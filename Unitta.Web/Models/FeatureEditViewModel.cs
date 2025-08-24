using Microsoft.AspNetCore.Mvc.Rendering;
using Unitta.Application.DTOs;

namespace Unitta.Web.Models;

public class FeatureEditViewModel
{
    public FeatureUpdateDto Feature { get; set; } = new();

    public IEnumerable<SelectListItem>? UnitList { get; set; }
}