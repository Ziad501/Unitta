using Microsoft.EntityFrameworkCore;
using Unitta.Application.Interfaces;
using Unitta.Domain.Entities;
using Unitta.Infrastructure.Persistence;

namespace Unitta.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.Unit)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.Unit)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(string userId)
    {
        return await _context.Bookings
            .Include(b => b.Unit)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByUnitIdAsync(int unitId)
    {
        return await _context.Bookings
            .Where(b => b.UnitId == unitId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _context.Bookings
            .Include(b => b.Unit)
            .Where(b => b.CheckInDate <= endDate && b.CheckOutDate >= startDate)
            .OrderBy(b => b.CheckInDate)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(Booking booking)
    {
        try
        {
            _context.Bookings.Add(booking);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Booking booking)
    {
        try
        {
            _context.Bookings.Update(booking);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Bookings.AnyAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetConflictingBookingsAsync(int unitId, DateOnly checkIn, DateOnly checkOut, int? excludeBookingId = null)
    {
        var query = _context.Bookings
            .Where(b => b.UnitId == unitId &&
                       b.CheckInDate < checkOut &&
                       b.CheckOutDate > checkIn);

        if (excludeBookingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookingId.Value);
        }

        return await query.ToListAsync();
    }
}