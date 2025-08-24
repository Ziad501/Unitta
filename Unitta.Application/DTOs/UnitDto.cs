namespace Unitta.Application.DTOs;

public record UnitDto(
    int Id,
    string Name,
    decimal Price,
    int Sqft,
    int Occupancy,
    string? Description,
    string? ImageUrl
);