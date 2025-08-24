namespace Unitta.Domain.Entities;

public class UnitNo
{
    public int UnitNumber { get; set; }
    public int? UnitId { get; set; }
    public string? SpecialDetails { get; set; }

    public virtual Unit? Unit { get; set; }
}