namespace Unitta.Web.Models;
public class RadialBarChartViewModel
{
    public decimal TotalCount { get; set; }
    public decimal CountInCurrentMonth { get; set; }
    public bool IsIncrease { get; set; }
    public int[] Series { get; set; }
}
