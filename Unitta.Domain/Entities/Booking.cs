namespace Unitta.Domain.Entities;

public class Booking
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public double TotalCost { get; set; }
    public int Nights { get; set; }
    public string? Status { get; set; }
    public DateTime BookingDate { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public bool IsPaymentSuccessful { get; set; } = false;
    public DateTime? PaymentDate { get; set; }
    public string? StripeSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public DateTime? ActualCheckInDate { get; set; }
    public DateTime? ActualCheckOutDate { get; set; }
    public int UnitNumber { get; set; }

    public virtual Unit Unit { get; set; } = null!;

}
