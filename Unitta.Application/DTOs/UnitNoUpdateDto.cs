namespace Unitta.Application.DTOs;
public record UnitNoUpdateDto(
    int Id,
    int UnitNumber,
    int UnitId,
    string? SpecialDetails
);