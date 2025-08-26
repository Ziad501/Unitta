using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using Unitta.Application.DTOs;
using Unitta.Application.Interfaces;
using Unitta.Application.Mappers;
using Unitta.Application.Utility;
using Unitta.Domain.Entities;

namespace Unitta.Web.Controllers;

public class BookingController(IBookingRepository _repo,
    IUnitRepository _unit,
    IUserService _userService,
    IEmailSender _emailSender,
    ILogger<BookingController> _logger) : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        _logger.LogInformation("Accessing Booking Index page.");
        return View();
    }
    [Authorize]
    public async Task<IActionResult> FinalizeBooking(int UnitId, DateOnly CheckInDate, int nights)
    {
        _logger.LogInformation("Finalizing booking for Unit ID {UnitId} with Check-In Date {CheckInDate} for {Nights} nights.", UnitId, CheckInDate, nights);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User is not authenticated, despite [Authorize] attribute.");
            return Unauthorized();
        }

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found in database.", userId);

            TempData["ErrorMessage"] = "Your user account could not be found.";

            return RedirectToAction("Login", "Account");
        }

        var unit = await _unit.GetByIdAsync(UnitId);
        if (unit == null)
        {
            _logger.LogWarning("Unit with ID {UnitId} not found.", UnitId);
            TempData["ErrorMessage"] = "The selected unit does not exist.";
            return RedirectToAction("Error", "Home");
        }

        BookingDto bookingDto = new()
        {
            UnitId = UnitId,
            Unit = unit.ToUnitDto(),
            CheckInDate = CheckInDate,
            Nights = nights,
            CheckOutDate = CheckInDate.AddDays(nights),
            TotalCost = unit.Price * nights,
            UserId = userId,
            Email = user.Email ?? string.Empty,
            Name = user.Name ?? string.Empty,
            Phone = user.Phone ?? string.Empty,
        };

        _logger.LogInformation("Booking details prepared for Unit ID {UnitId} by User ID {UserId}.", UnitId, userId);
        return View(bookingDto);
    }

    // In BookingController.cs

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Client)]
    public async Task<IActionResult> FinalizeBooking(BookingDto bookingDto)
    {
        _logger.LogInformation("Processing booking for Unit ID {UnitId}.", bookingDto.UnitId);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID not found in claims during POST.");
            return Unauthorized();
        }

        var unit = await _unit.GetByIdAsync(bookingDto.UnitId);
        if (unit == null)
        {
            _logger.LogWarning("Unit with ID {UnitId} not found during POST.", bookingDto.UnitId);
            TempData["ErrorMessage"] = "The selected unit does not exist.";
            return RedirectToAction("Error", "Home");
        }

        bookingDto.UserId = userId;
        bookingDto.TotalCost = unit.Price * bookingDto.Nights;
        bookingDto.CheckOutDate = bookingDto.CheckInDate.AddDays(bookingDto.Nights);
        bookingDto.Unit = unit.ToUnitDto();

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid booking model state for Unit ID {UnitId}.", bookingDto.UnitId);
            return View(bookingDto);
        }


        // First, check if this user already has an abandoned PENDING booking for this unit.
        var existingPendingBooking = await _repo.GetAsync(
            b => b.UnitId == bookingDto.UnitId &&
                 b.UserId == userId &&
                 b.Status == SD.StatusPending);

        Booking bookingToProcess;

        if (existingPendingBooking != null)
        {
            // If a pending booking exists, we will reuse it for this new payment attempt.
            _logger.LogInformation("Found existing pending booking (ID: {BookingId}). Reusing it.", existingPendingBooking.Id);
            bookingToProcess = existingPendingBooking;

            // Update details in case they changed (e.g., number of nights)
            bookingToProcess.Nights = bookingDto.Nights;
            bookingToProcess.TotalCost = bookingDto.TotalCost;
            bookingToProcess.CheckInDate = bookingDto.CheckInDate;
            bookingToProcess.CheckOutDate = bookingDto.CheckOutDate;
        }
        else
        {
            // If no pending booking exists, check for CONFIRMED conflicts.
            var conflictingBookings = await _repo.GetConflictingBookingsAsync(
                bookingDto.UnitId,
                bookingDto.CheckInDate,
                bookingDto.CheckOutDate);

            if (conflictingBookings.Any())
            {
                _logger.LogWarning("Conflicting bookings found for Unit ID {UnitId} between {CheckInDate} and {CheckOutDate}.",
                    bookingDto.UnitId, bookingDto.CheckInDate, bookingDto.CheckOutDate);
                TempData["ErrorMessage"] = "This unit is already booked for the selected dates.";
                return RedirectToAction("Error", "Home");
            }

            // No conflicts, so create a new booking record.
            _logger.LogInformation("No existing pending booking found. Creating a new one.");
            bookingToProcess = bookingDto.ToBooking();
            bookingToProcess.Unit = unit;
            bookingToProcess.Status = SD.StatusPending;
            bookingToProcess.BookingDate = DateTime.UtcNow;
            bookingToProcess.IsPaymentSuccessful = false;
            bookingToProcess.UnitNumber = unit.UnitNo?.UnitNumber ?? 0;
            await _repo.CreateAsync(bookingToProcess);
        }



        var domain = Request.Scheme + "://" + Request.Host.Value + "/";
        var options = new SessionCreateOptions
        {
            SuccessUrl = domain + "Booking/BookingConfirmation?bookingId=" + bookingToProcess.Id,
            CancelUrl = domain + "Booking/FinalizeBooking?UnitId=" + bookingToProcess.UnitId + "&CheckInDate=" + bookingToProcess.CheckInDate + "&nights=" + bookingToProcess.Nights,
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
        {
            new() {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = unit.Name,
                    },
                    UnitAmountDecimal = (decimal)(bookingToProcess.TotalCost * 100),
                },
                Quantity = 1,
            },
        },
            Mode = "payment",
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        if (session == null)
        {
            _logger.LogError("Stripe session creation failed for Booking ID {BookingId}.", bookingToProcess.Id);
            TempData["ErrorMessage"] = "An error occurred while processing your payment. Please try again later.";
            return RedirectToAction("Error", "Home");
        }

        Response.Headers.Add("Location", session.Url);
        _logger.LogInformation("Stripe session created successfully for Booking ID {BookingId}.", bookingToProcess.Id);

        await _repo.UpdatePaymentIdAsync(bookingToProcess.Id, session.Id, session.PaymentIntentId);

        _logger.LogInformation("Redirecting to Stripe payment session for Booking ID {BookingId}.", bookingToProcess.Id);


        return new StatusCodeResult(StatusCodes.Status303SeeOther);
    }
    [Authorize]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Client)]
    public async Task<IActionResult> BookingConfirmation(int bookingId)
    {
        _logger.LogInformation("Fetching booking confirmation for Booking ID {BookingId}.", bookingId);
        var booking = await _repo.GetByIdAsync(bookingId);
        if (booking == null)
        {
            _logger.LogWarning("Booking with ID {BookingId} not found.", bookingId);
            TempData["ErrorMessage"] = "The requested booking does not exist.";
            return RedirectToAction("Error", "Home");
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (booking.UserId != userId)
        {
            _logger.LogWarning("User ID {UserId} does not match Booking User ID {BookingUserId}.", userId, booking.UserId);
            return Unauthorized();
        }
        if (booking.Status == SD.StatusPending)
        {
            var service = new SessionService();
            Session session = await service.GetAsync(booking.StripeSessionId);
            if (session == null)
            {
                _logger.LogWarning("Stripe session with ID {SessionId} not found for Booking ID {BookingId}.", booking.StripeSessionId, booking.Id);
                TempData["ErrorMessage"] = "The payment session could not be found. Please try again.";
                return RedirectToAction("Error", "Home");
            }
            else
            {
                if (session.PaymentStatus == SD.StatusPaid)
                {
                    booking.IsPaymentSuccessful = true;
                    booking.Status = SD.StatusApproved;
                    booking.PaymentDate = DateTime.UtcNow;
                    await _repo.UpdatePaymentIdAsync(booking.Id, session.Id, session.PaymentIntentId);
                    await _repo.UpdateStatusAsync(booking.Id, SD.StatusApproved, 0);
                    _logger.LogInformation("Booking ID {BookingId} payment confirmed successfully.", booking.Id);
                    try
                    {
                        var subject = $"Your Booking is Confirmed! - Booking #{booking.Id}";
                        var body = $@"
                            <h1>Booking Confirmation</h1>
                            <p>Hi {booking.Name},</p>
                            <p>Your payment was successful and your booking is confirmed. Here are the details:</p>
                            <ul>
                                <li><strong>Unit:</strong> {booking.Unit.Name}</li>
                                <li><strong>Check-in Date:</strong> {booking.CheckInDate:yyyy-MM-dd}</li>
                                <li><strong>Check-out Date:</strong> {booking.CheckOutDate:yyyy-MM-dd}</li>
                                <li><strong>Total Cost:</strong> {booking.TotalCost:C}</li>
                            </ul>
                            <p>Thank you for choosing Unitta!</p>";
                        await _emailSender.SendEmailAsync(booking.Email, subject, body);
                        _logger.LogInformation("Booking confirmation email sent to {Email} for Booking ID {BookingId}", booking.Email, booking.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send booking confirmation email for Booking ID {BookingId}", booking.Id);
                    }
                }
            }
        }
        var updatedBooking = await _repo.GetByIdAsync(booking.Id);
        if (updatedBooking == null)
        {
            _logger.LogWarning("Updated booking with ID {BookingId} not found after payment confirmation.", booking.Id);
            TempData["ErrorMessage"] = "The booking could not be retrieved after payment confirmation.";
            return RedirectToAction("Error", "Home");
        }
        var bookingDto = updatedBooking.ToBookingDto();
        _logger.LogInformation("Booking confirmation retrieved successfully for Booking ID {BookingId}.", booking.Id);
        return View(bookingDto);
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllBookings(string? status)
    {
        IEnumerable<Booking> bookings;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Fetching all bookings with status filter: {Status}", status ?? "All");
        if (string.IsNullOrEmpty(status) || status.Trim() == "")
        {
            status = null;
        }

        if (User.IsInRole(SD.Role_Admin))
        {
            _logger.LogInformation("Fetching bookings for admin with status filter: {Status}", status ?? "All");
            bookings = await _repo.GetAllAsync(status);
        }
        else
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims.");
                return Unauthorized();
            }
            _logger.LogInformation("Fetching bookings for User ID {UserId} with status filter: {Status}", userId, status ?? "All");
            bookings = await _repo.GetByUserIdAsync(userId, status);
        }

        return Json(new { data = bookings.ToBookingDtos() });
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        _logger.LogInformation("Fetching booking details for Booking ID {BookingId}.", id);
        var booking = await _repo.GetByIdAsync(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking with ID {BookingId} not found.", id);
            return NotFound();
        }
        var bookingDto = booking.ToBookingDto();
        return View(bookingDto);
    }
    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> CheckIn(BookingDto bookingDto)
    {
        _logger.LogInformation("Processing check-in for Booking ID {BookingId} with Unit Number {UnitNumber}.", bookingDto.Id, bookingDto.UnitNumber);
        var booking = bookingDto.ToBooking();
        await _repo.UpdateStatusAsync(booking.Id, SD.StatusCheckIn, booking.UnitNumber);
        TempData["SuccessMessage"] = "Check-in successful.";
        _logger.LogInformation("Check-in successful for Booking ID {BookingId}.", booking.Id);
        return RedirectToAction(nameof(Details), new { id = booking.Id });
    }
    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> CheckOut(BookingDto bookingDto)
    {
        _logger.LogInformation("Processing check-out for Booking ID {BookingId} with Unit Number {UnitNumber}.", bookingDto.Id, bookingDto.UnitNumber);
        var booking = bookingDto.ToBooking();
        await _repo.UpdateStatusAsync(booking.Id, SD.StatusCompleted, booking.UnitNumber);
        TempData["SuccessMessage"] = "Check-out successful.";
        _logger.LogInformation("Check-out successful for Booking ID {BookingId}.", booking.Id);
        return RedirectToAction(nameof(Details), new { id = booking.Id });
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CancelBooking(BookingDto bookingDto)
    {
        _logger.LogInformation("Processing cancellation for Booking ID {BookingId} with Unit Number {UnitNumber}.", bookingDto.Id, bookingDto.UnitNumber);
        var booking = bookingDto.ToBooking();
        await _repo.UpdateStatusAsync(booking.Id, SD.StatusCancelled, 0);
        TempData["SuccessMessage"] = "Cancellation successful.";
        _logger.LogInformation("Cancellation successful for Booking ID {BookingId}.", booking.Id);
        return RedirectToAction(nameof(Details), new { id = booking.Id });
    }


    [HttpGet]
    [Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> DownloadBookingDetails(int bookingId)
    {
        _logger.LogInformation("Admin downloading booking details for Booking ID {BookingId}.", bookingId);

        var booking = await _repo.GetByIdAsync(bookingId);
        if (booking == null)
        {
            _logger.LogWarning("Booking with ID {BookingId} not found.", bookingId);
            return NotFound();
        }

        try
        {
            // Path to your Word template
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "BookingDetails.docx");

            if (!System.IO.File.Exists(templatePath))
            {
                _logger.LogError("Template file not found at path: {TemplatePath}", templatePath);
                TempData["ErrorMessage"] = "Booking details template not found.";
                return RedirectToAction("Details", new { id = bookingId });
            }

            // Create a memory stream to work with the document
            using var memoryStream = new MemoryStream();

            // Copy template to memory stream
            using (var fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(memoryStream);
            }

            // Reset position to beginning
            memoryStream.Position = 0;

            // Process the Word document
            using (var wordDocument = WordprocessingDocument.Open(memoryStream, true))
            {
                // Get the main document part
                var mainPart = wordDocument.MainDocumentPart;
                if (mainPart != null)
                {
                    // Replace placeholders in the document
                    var body = mainPart.Document.Body;
                    if (body != null)
                    {
                        ReplaceTextInDocument(body, new Dictionary<string, string>
                    {
                        { "xx_booking_Number", booking.Id.ToString() },
                        { "xx_BOOKING_Date", booking.BookingDate.ToString("yyyy-MM-dd") },
                        { "xx_client_name", booking.Name ?? "N/A" },
                        { "xx_client_phone", booking.Phone ?? "N/A" },
                        { "xx_client_email", booking.Email ?? "N/A" },
                        { "xx_payment_date", booking.PaymentDate?.ToString("yyyy-MM-dd") ?? "Pending" },
                        { "xx_checkin_date", booking.CheckInDate.ToString("yyyy-MM-dd") },
                        { "xx_checkout_date", booking.CheckOutDate.ToString("yyyy-MM-dd") },
                        { "xx_booking_total", booking.TotalCost.ToString("C") }
                    });
                    }

                    // Save changes
                    mainPart.Document.Save();
                }
            }

            // Generate filename
            string fileName = $"BookingDetails_{booking.Id}_{DateTime.Now:yyyyMMdd}.docx";

            _logger.LogInformation("Booking details document generated successfully for Booking ID {BookingId}.", bookingId);

            // Return the file for download
            return File(memoryStream.ToArray(),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating booking details document for Booking ID {BookingId}.", bookingId);
            TempData["ErrorMessage"] = "An error occurred while generating the booking details document.";
            return RedirectToAction("Details", new { id = bookingId });
        }
    }

    private void ReplaceTextInDocument(Body body, Dictionary<string, string> replacements)
    {
        foreach (var replacement in replacements)
        {
            var textElements = body.Descendants<Text>().ToList();
            foreach (var textElement in textElements)
            {
                if (textElement.Text.Contains(replacement.Key))
                {
                    textElement.Text = textElement.Text.Replace(replacement.Key, replacement.Value);
                }
            }

            var paragraphs = body.Descendants<Paragraph>().ToList();
            foreach (var paragraph in paragraphs)
            {
                ReplaceTextInParagraph(paragraph, replacement.Key, replacement.Value);
            }
        }
    }

    private void ReplaceTextInParagraph(Paragraph paragraph, string searchText, string replaceText)
    {
        var fullText = paragraph.InnerText;
        if (!fullText.Contains(searchText))
            return;

        // Get all runs in the paragraph
        var runs = paragraph.Elements<Run>().ToList();
        if (runs.Count == 0)
            return;

        // Build the full text from all runs and find replacement positions
        var runTexts = new List<string>();
        var textElements = new List<Text>();

        foreach (var run in runs)
        {
            var textElement = run.GetFirstChild<Text>();
            if (textElement != null)
            {
                runTexts.Add(textElement.Text);
                textElements.Add(textElement);
            }
            else
            {
                runTexts.Add("");
                textElements.Add(null);
            }
        }

        var combinedText = string.Join("", runTexts);
        if (!combinedText.Contains(searchText))
            return;

        // Replace the text
        var replacedText = combinedText.Replace(searchText, replaceText);

        // Clear all existing text elements
        foreach (var textElement in textElements)
        {
            if (textElement != null)
            {
                textElement.Text = "";
            }
        }

        // Put the replaced text in the first available text element
        var firstTextElement = textElements.FirstOrDefault(t => t != null);
        if (firstTextElement != null)
        {
            firstTextElement.Text = replacedText;
        }
        else if (runs.Count > 0)
        {
            // Create a new text element in the first run if none exists
            var newText = new Text(replacedText);
            runs[0].AppendChild(newText);
        }
    }
}