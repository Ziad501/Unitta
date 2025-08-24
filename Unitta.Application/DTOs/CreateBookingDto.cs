namespace Unitta.Application.DTOs;

public record CreateBookingDto(
    int UnitId,
    string Name,
    string Email,
    string? Phone,
    int Nights,
    DateOnly CheckInDate,
    int UnitNumber
);