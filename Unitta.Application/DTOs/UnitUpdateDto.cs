using Microsoft.AspNetCore.Http;

namespace Unitta.Application.DTOs;

public class UnitUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Sqft { get; set; }
    public int Occupancy { get; set; }
    public string Description { get; set; }

    public string? CurrentImageUrl { get; set; }

    public IFormFile? NewImage { get; set; }
}