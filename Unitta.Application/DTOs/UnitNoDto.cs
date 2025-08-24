namespace Unitta.Application.DTOs;

public record UnitNoDto(
    int UnitNumber,
    int? UnitId,
    string? SpecialDetails
);