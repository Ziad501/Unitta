using Microsoft.AspNetCore.Http;

namespace Unitta.Application.DTOs;

public class UnitCreateDto
{
    public string Name { get; set; }
    public double Price { get; set; }
    public int Sqft { get; set; }
    public int Occupancy { get; set; }
    public string? Description { get; set; }
    public IFormFile Image { get; set; }
}