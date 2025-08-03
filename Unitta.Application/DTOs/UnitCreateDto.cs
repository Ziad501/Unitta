namespace Unitta.Application.DTOs;

public record UnitCreateDto(
    string Name,
    double Price,
    int Sqft,
    int Occupancy,
    string? Description,
    string? ImageUrl
);