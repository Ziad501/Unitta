namespace Unitta.Application.DTOs;

public class FeatureUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UnitId { get; set; }
}