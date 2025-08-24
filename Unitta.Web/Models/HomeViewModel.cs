using Unitta.Application.DTOs;

namespace Unitta.Web.Models;

public class HomeViewModel
{
    public IEnumerable<UnitDto>? Units { get; set; }
    public DateOnly? CheckInDate { get; set; }
    public DateOnly? CheckOutDate { get; set; }
    public int NumberOfNights { get; set; }
    public HashSet<int>? AvailableUnitIds { get; set; }
}