namespace Unitta.Domain.Entities;

public class Feature : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UnitId { get; set; }

    public virtual Unit Unit { get; set; } = null!;
}