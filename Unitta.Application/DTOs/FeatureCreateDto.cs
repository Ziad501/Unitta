namespace Unitta.Application.DTOs;

public class FeatureCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UnitId { get; set; }
}