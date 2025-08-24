using Microsoft.AspNetCore.Mvc.Rendering;
using Unitta.Application.DTOs;

namespace Unitta.Web.Models;

public class UnitNoCreateViewModel
{
    public UnitNoCreateDto? UnitNo { get; set; }
    public IEnumerable<SelectListItem>? UnitList { get; set; }
}