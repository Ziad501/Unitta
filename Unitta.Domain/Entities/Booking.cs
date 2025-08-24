namespace Unitta.Domain.Entities;

public class Booking : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public decimal TotalCost { get; set; }
    public int Nights { get; set; }
    public string? Status { get; set; }
    public DateTimeOffset BookingDate { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public bool IsPaymentSuccessful { get; set; } = false;
    public DateTimeOffset? PaymentDate { get; set; }
    public string? StripeSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public DateTimeOffset? ActualCheckInDate { get; set; }
    public DateTimeOffset? ActualCheckOutDate { get; set; }
    public int UnitNumber { get; set; }
    public List<UnitNo> UnitNoList { get; set; }
    public virtual Unit Unit { get; set; } = null!;
}
