namespace Unitta.Application.DTOs;
public record UnitNoUpdateDto(
    int UnitNumber,
    int? UnitId,
    string? SpecialDetails
);