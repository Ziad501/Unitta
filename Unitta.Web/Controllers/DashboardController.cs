using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unitta.Application.Interfaces;
using Unitta.Application.Utility;
using Unitta.Web.Models;

namespace Unitta.Web.Controllers;
[Authorize(Roles = SD.Role_Admin + "," + SD.Role_AdminView)]
public class DashboardController(IBookingRepository _booking, IUserService _userService, ILogger<DashboardController> _logger) : Controller
{
    public IActionResult Index()
    {
        ViewBag.IsAdminView = User.IsInRole(SD.Role_AdminView);
        return View();
    }

    public async Task<IActionResult> GetTotalBooking()
    {
        _logger.LogInformation("GetTotalBooking called");

        try
        {
            var today = DateTime.Today;
            var currentMonthStartDate = new DateTime(today.Year, today.Month, 1);
            var previousMonthStartDate = currentMonthStartDate.AddMonths(-1);
            var currentMonthEndDate = currentMonthStartDate.AddMonths(1);

            _logger.LogInformation($"Date ranges - Previous: {previousMonthStartDate:yyyy-MM-dd} to {currentMonthStartDate:yyyy-MM-dd}, Current: {currentMonthStartDate:yyyy-MM-dd} to {currentMonthEndDate:yyyy-MM-dd}");

            // First, get all bookings without filter to see if there's any data
            var allBookings = await _booking.GetAllAsync();
            _logger.LogInformation($"Total bookings in database: {allBookings?.Count() ?? 0}");

            if (allBookings == null || !allBookings.Any())
            {
                _logger.LogWarning("No bookings found in database");
                return Json(CreateRadialBarChartViewModel(0, 0, 0));
            }

            // Log booking statuses to understand the data
            var statusGroups = allBookings.GroupBy(b => b.Status).Select(g => new { Status = g.Key, Count = g.Count() });
            foreach (var group in statusGroups)
            {
                _logger.LogInformation($"Bookings with status '{group.Status}': {group.Count}");
            }

            // Now apply the filter
            var totalBooking = allBookings.Where(p => p.Status != SD.StatusPending && p.Status != SD.StatusCancelled).ToList();
            _logger.LogInformation($"Filtered bookings (excluding pending/cancelled): {totalBooking.Count}");

            if (!totalBooking.Any())
            {
                _logger.LogWarning("No bookings found after filtering out pending and cancelled statuses");
                return Json(CreateRadialBarChartViewModel(0, 0, 0));
            }

            var countByCurrentMonth = totalBooking.Count(p => p.BookingDate >= currentMonthStartDate && p.BookingDate < currentMonthEndDate);
            var countByPreviousMonth = totalBooking.Count(p => p.BookingDate >= previousMonthStartDate && p.BookingDate < currentMonthStartDate);

            _logger.LogInformation($"Current month bookings: {countByCurrentMonth}, Previous month: {countByPreviousMonth}");

            return Json(CreateRadialBarChartViewModel(totalBooking.Count, countByCurrentMonth, countByPreviousMonth));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting total bookings");
            return Json(CreateRadialBarChartViewModel(0, 0, 0));
        }
    }

    public async Task<IActionResult> GetTotalUsers()
    {
        _logger.LogInformation("GetTotalUsers called");

        try
        {
            var today = DateTime.Today;
            var currentMonthStartDate = new DateTime(today.Year, today.Month, 1);
            var previousMonthStartDate = currentMonthStartDate.AddMonths(-1);
            var currentMonthEndDate = currentMonthStartDate.AddMonths(1);

            var totalUsers = await _userService.GetAllUsersAsync();
            _logger.LogInformation($"Total users found: {totalUsers?.Count() ?? 0}");

            if (totalUsers == null || !totalUsers.Any())
            {
                _logger.LogWarning("No users found in database");
                return Json(CreateRadialBarChartViewModel(0, 0, 0));
            }

            var countByCurrentMonth = totalUsers.Count(p => p.CreatedAt >= currentMonthStartDate && p.CreatedAt < currentMonthEndDate);
            var countByPreviousMonth = totalUsers.Count(p => p.CreatedAt >= previousMonthStartDate && p.CreatedAt < currentMonthStartDate);

            _logger.LogInformation($"Current month users: {countByCurrentMonth}, Previous month: {countByPreviousMonth}");

            return Json(CreateRadialBarChartViewModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting total users");
            return Json(CreateRadialBarChartViewModel(0, 0, 0));
        }
    }

    public async Task<IActionResult> GetTotalRevenue()
    {
        _logger.LogInformation("GetTotalRevenue called");

        try
        {
            var today = DateTime.Today;
            var currentMonthStartDate = new DateTime(today.Year, today.Month, 1);
            var previousMonthStartDate = currentMonthStartDate.AddMonths(-1);
            var currentMonthEndDate = currentMonthStartDate.AddMonths(1);

            var totalRevenue = await _booking.GetAllAsync(
                filter: p => p.Status != SD.StatusPending && p.Status != SD.StatusCancelled);

            _logger.LogInformation($"Revenue bookings found: {totalRevenue?.Count() ?? 0}");

            if (totalRevenue == null || !totalRevenue.Any())
            {
                _logger.LogWarning("No revenue bookings found");
                return Json(CreateRadialBarChartViewModel(0, 0, 0));
            }

            var totalRevenueAmount = Convert.ToInt32(totalRevenue.Sum(p => p.TotalCost));
            var countByCurrentMonth = totalRevenue.Where(p => p.BookingDate >= currentMonthStartDate && p.BookingDate < currentMonthEndDate).Sum(u => u.TotalCost);
            var countByPreviousMonth = totalRevenue.Where(p => p.BookingDate >= previousMonthStartDate && p.BookingDate < currentMonthStartDate).Sum(u => u.TotalCost);

            _logger.LogInformation($"Total revenue: {totalRevenueAmount}, Current month: {countByCurrentMonth}, Previous month: {countByPreviousMonth}");

            return Json(CreateRadialBarChartViewModel(totalRevenueAmount, Convert.ToInt32(countByCurrentMonth), Convert.ToInt32(countByPreviousMonth)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting total revenue");
            return Json(CreateRadialBarChartViewModel(0, 0, 0));
        }
    }

    public async Task<IActionResult> GetBookingPieChart()
    {
        _logger.LogInformation("GetBookingPieChart called");


        var thirtyDaysAgo = DateTime.Now.AddDays(-30);
        _logger.LogInformation($"Fetching bookings from {thirtyDaysAgo:yyyy-MM-dd HH:mm:ss} onwards");

        var allBookings = await _booking.GetAllAsync();
        _logger.LogInformation($"Total bookings in database: {allBookings?.Count() ?? 0}");

        if (allBookings == null || !allBookings.Any())
        {
            _logger.LogWarning("No bookings found in database at all");
            return Json(new PieChartViewModel
            {
                Labels = new[] { "No Data Available" },
                Series = new decimal[] { 1 }
            });
        }

        var recentBookings = allBookings.Where(p => p.BookingDate >= thirtyDaysAgo).ToList();
        _logger.LogInformation($"Bookings in last 30 days: {recentBookings.Count}");

        var totalBooking = recentBookings.Where(p => p.Status != SD.StatusPending && p.Status != SD.StatusCancelled).ToList();
        _logger.LogInformation($"Filtered bookings (last 30 days, excluding pending/cancelled): {totalBooking.Count}");

        if (!totalBooking.Any())
        {
            _logger.LogWarning("No valid bookings found for pie chart");
            return Json(new PieChartViewModel
            {
                Labels = new[] { "No Valid Bookings" },
                Series = new decimal[] { 1 }
            });
        }

        var clientWithOneBooking = totalBooking
            .GroupBy(p => p.UserId)
            .Where(g => g.Count() == 1)
            .Select(g => g.Key)
            .ToList();

        int bookingByNewClients = clientWithOneBooking.Count;
        int bookingByReturningClients = totalBooking.Count() - bookingByNewClients;

        _logger.LogInformation($"New clients: {bookingByNewClients}, Returning clients: {bookingByReturningClients}");

        PieChartViewModel pieChartViewModel = new()
        {
            Labels = new[] { "New Clients Bookings", "Returning Clients Bookings" },
            Series = new decimal[] { bookingByNewClients, bookingByReturningClients }
        };

        return Json(pieChartViewModel);

    }
    public async Task<IActionResult> GetMemberAndBookingPieChart()
    {
        _logger.LogInformation("GetMemberAndBookingPieChart called");
        var booking = await _booking.GetAllAsync(filter: u => u.BookingDate >= DateTime.UtcNow.AddDays(-30) && u.BookingDate.Date <= DateTime.UtcNow);
        var totalBooking = booking.GroupBy(p => p.BookingDate.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToList();

        _logger.LogInformation($"Total bookings in last 30 days: {totalBooking.Count}");

        if (totalBooking == null || !totalBooking.Any())
        {
            _logger.LogWarning("No bookings found in the last 30 days");
            return Json(new PieChartViewModel
            {
                Labels = new[] { "No Data Available" },
                Series = new decimal[] { 1 }
            });
        }
        var Users = await _userService.GetAllUsersAsync(p => p.CreatedAt >= DateTime.UtcNow.AddDays(-30) && p.CreatedAt <= DateTime.UtcNow);

        var UserData = Users.GroupBy(p => p.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToList();
        _logger.LogInformation($"Total users found: {UserData?.Count() ?? 0}");
        if (UserData == null || !UserData.Any())
        {
            _logger.LogWarning("No users found in database");
            return Json(new PieChartViewModel
            {
                Labels = new[] { "No Data Available" },
                Series = new decimal[] { 1 }
            });
        }

        var leftJoin = totalBooking.GroupJoin(UserData,
                booking => booking.Date,
                user => user.Date,
                (booking, users) => new
                {
                    booking.Date,
                    bookingData = booking.Count,
                    UserCount = users.Select(x => x.Count).FirstOrDefault()
                });

        var rightJoin = UserData.GroupJoin(totalBooking,
                user => user.Date,
                booking => booking.Date,
                (users, booking) => new
                {
                    users.Date,
                    bookingData = booking.Select(x => x.Count).FirstOrDefault(),
                    UserCount = users.Count
                });
        var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.Date).ToList();

        var bookingSeries = mergedData.Select(x => x.bookingData).ToArray();
        var userSeries = mergedData.Select(x => x.UserCount).ToArray();
        var categories = mergedData.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray();

        LineChartViewModel lineChart = new()
        {
            Categories = categories,
        };
        lineChart.Series = new List<ChartData>
        {
        new ChartData { Name = "New Bookings", Data = bookingSeries },
        new ChartData { Name = "New Members", Data = userSeries }
        };

        return Json(lineChart);

    }

    private static RadialBarChartViewModel CreateRadialBarChartViewModel(int totalCount, int countInCurrentMonth, int countInPreviousMonth)
    {
        var rBC = new RadialBarChartViewModel
        {
            TotalCount = totalCount,
            CountInCurrentMonth = countInCurrentMonth,
            IsIncrease = countInCurrentMonth > countInPreviousMonth
        };

        int increaseDecreaseRatio;
        if (countInPreviousMonth == 0)
        {
            increaseDecreaseRatio = (countInCurrentMonth > 0) ? 100 : 0;
        }
        else
        {
            double rawRatio = (double)(countInCurrentMonth - countInPreviousMonth) / countInPreviousMonth;
            increaseDecreaseRatio = Convert.ToInt32(rawRatio * 100);
        }

        rBC.Series = new[] { increaseDecreaseRatio };
        return rBC;
    }
}