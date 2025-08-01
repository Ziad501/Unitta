using Unitta.Domain.Entities;

namespace Unitta.Application.DTOs;

public record BookingWithUserDto
(
    Booking Booking,
    User User
);

