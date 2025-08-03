namespace Unitta.Application.DTOs;

public record UnitDto(
int Id,
string Name,
double Price,
int Sqft,
int Occupancy,
string? Description,
string? ImageUrl
);