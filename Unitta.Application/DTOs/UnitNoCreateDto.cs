namespace Unitta.Application.DTOs;

public record UnitNoCreateDto(
    int UnitNumber,
    int UnitId,
    string? SpecialDetails
);