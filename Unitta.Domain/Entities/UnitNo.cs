namespace Unitta.Domain.Entities;

public class UnitNo : BaseEntity
{
    public int UnitNumber { get; set; } // The actual unit number (e.g., "A101", "Villa-1")
    public int UnitId { get; set; } // Foreign key (nullable since unit number can exist without being assigned)
    public string? SpecialDetails { get; set; }

    // Navigation property
    public virtual Unit Unit { get; set; } // One-to-one (optional)
}

