using System.Linq.Expressions;
using Unitta.Domain.Entities;

namespace Unitta.Application.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id);
    Task<IEnumerable<Booking>> GetAllAsync(string? status = null, Expression<Func<Booking, bool>>? filter = null);
    Task<IEnumerable<Booking>> GetByUserIdAsync(string userId, string? status = null);
    Task<IEnumerable<Booking>> GetByUnitIdAsync(int unitId);
    Task<IEnumerable<Booking>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task CreateAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Booking>> GetConflictingBookingsAsync(int unitId, DateOnly checkIn, DateOnly checkOut, int? excludeBookingId = null);
    Task UpdateStatusAsync(int bookingId, string bookingStatus, int unitNumber = 0);
    Task UpdatePaymentIdAsync(int bookingId, string sessionId, string paymentIntentId);
    Task<Booking?> GetAsync(Expression<Func<Booking, bool>> filter);

}