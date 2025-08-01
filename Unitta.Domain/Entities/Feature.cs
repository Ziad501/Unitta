namespace Unitta.Domain.Entities;

public class Feature
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UnitId { get; set; }

    public virtual Unit Unit { get; set; } = null!;
}