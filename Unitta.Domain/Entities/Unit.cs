namespace Unitta.Domain.Entities;

public class Unit : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Sqft { get; set; }
    public int Occupancy { get; set; }
    public string? ImageUrl { get; set; }
    public DateTimeOffset? CreatedDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public bool IsAvailable { get; set; } = true;
    public virtual ICollection<Feature> Features { get; set; } = new HashSet<Feature>();
    public virtual UnitNo? UnitNo { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
}