namespace Unitta.Application.DTOs;

public record UpdateBookingDto(
     string Name,
     string Email,
     string? Phone,
     string Status
 );