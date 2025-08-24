using Microsoft.AspNetCore.Mvc.Rendering;
using Unitta.Application.DTOs;

namespace Unitta.Web.Models;

public class FeatureCreateViewModel
{
    public FeatureCreateDto Feature { get; set; } = new();

    public IEnumerable<SelectListItem>? UnitList { get; set; }
}