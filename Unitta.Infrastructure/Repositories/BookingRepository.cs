using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Unitta.Application.Interfaces;
using Unitta.Application.Utility;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Persistence;

namespace Unitta.Infrastructure.Repositories;

public class BookingRepository(ApplicationDbContext _context) : IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.Unit)
            .AsSplitQuery()
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetAllAsync(string? status = null, Expression<Func<Booking, bool>>? filter = null)
    {
        var query = _context.Bookings.AsNoTracking().Include(b => b.Unit).AsSplitQuery();
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(b => b.Status == status);
        }

        return await query.OrderByDescending(b => b.BookingDate).ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(string userId, string? status = null)
    {
        var query = _context.Bookings
            .AsNoTracking()
            .Include(b => b.Unit)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .AsSplitQuery();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(b => b.Status == status);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByUnitIdAsync(int unitId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.UnitId == unitId)
            .OrderByDescending(b => b.BookingDate)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Unit)
            .Where(b => b.CheckInDate <= endDate && b.CheckOutDate >= startDate)
            .OrderBy(b => b.CheckInDate)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task CreateAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
    }


    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Bookings.AnyAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetConflictingBookingsAsync(int unitId, DateOnly checkIn, DateOnly checkOut, int? excludeBookingId = null)
    {
        var query = _context.Bookings.AsNoTracking()
            .Where(b => b.UnitId == unitId &&
                       b.CheckInDate < checkOut &&
                       b.CheckOutDate > checkIn);

        if (excludeBookingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookingId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task UpdateStatusAsync(int bookingId, string bookingStatus, int unitNumber = 0)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking != null)
        {
            booking.Status = bookingStatus;
            if (bookingStatus == SD.StatusCheckIn)
            {
                booking.UnitNumber = unitNumber;
                booking.ActualCheckInDate = DateTime.UtcNow;
            }
            else if (bookingStatus == SD.StatusCheckOut)
            {
                booking.ActualCheckOutDate = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePaymentIdAsync(int bookingId, string sessionId, string paymentIntentId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking != null)
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                booking.StripeSessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                booking.StripePaymentIntentId = paymentIntentId;
                booking.PaymentDate = DateTime.UtcNow;
                booking.IsPaymentSuccessful = true;
            }
            await _context.SaveChangesAsync();
        }
    }
    public async Task<Booking?> GetAsync(Expression<Func<Booking, bool>> filter)
    {
        IQueryable<Booking> query = _context.Bookings;
        return await query.FirstOrDefaultAsync(filter);
    }
}