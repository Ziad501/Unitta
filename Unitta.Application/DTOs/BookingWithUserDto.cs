namespace Unitta.Application.DTOs;

public record BookingWithUserDto
(
    BookingDto Booking,
    UserDto User
);

