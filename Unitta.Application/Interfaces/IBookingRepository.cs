using Unitta.Domain.Entities;

namespace Unitta.Application.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id);
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<IEnumerable<Booking>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Booking>> GetByUnitIdAsync(int unitId);
    Task<IEnumerable<Booking>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<bool> CreateAsync(Booking booking);
    Task<bool> UpdateAsync(Booking booking);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Booking>> GetConflictingBookingsAsync(int unitId, DateOnly checkIn, DateOnly checkOut, int? excludeBookingId = null);
}