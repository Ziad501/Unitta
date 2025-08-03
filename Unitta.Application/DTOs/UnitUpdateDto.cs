namespace Unitta.Application.DTOs;

public record UnitUpdateDto(
int Id,
string Name,
double Price,
int Sqft,
int Occupancy,
string Description,
string ImageUrl
);