using Unitta.Application.DTOs;
using Unitta.Application.Interfaces;
using Unitta.Application.Mappers;
using Unitta.Domain.Entities;

namespace Unitta.Application.Services;

public class BookingService(
    IBookingRepository _bookingRepository,
    IUserService _userService)
{
    public async Task CreateBookingAsync(Booking booking)
    {
        var userExists = await _userService.GetUserByIdAsync(booking.UserId);
        if (userExists == null)
            return;
        await _bookingRepository.CreateAsync(booking);
    }

    public async Task<BookingWithUserDto?> GetBookingWithUserAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null) return null;

        var user = await _userService.GetUserByIdAsync(booking.UserId);
        if (user == null) return null;

        var bookingDto = booking.ToBookingDto();
        var userDto = user.ToUserDto();

        return new BookingWithUserDto
        (
             Booking: bookingDto,
             User: userDto
        );
    }
}
