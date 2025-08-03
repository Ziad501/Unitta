namespace Unitta.Domain.Entities;

public class Unit : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Price { get; set; }
    public int Sqft { get; set; }
    public int Occupancy { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Feature> Features { get; set; } = new List<Feature>();
    public virtual ICollection<UnitNo> UnitNumbers { get; set; } = new List<UnitNo>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}