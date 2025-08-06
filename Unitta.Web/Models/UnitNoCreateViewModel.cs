using Microsoft.AspNetCore.Mvc.Rendering;
using Unitta.Application.DTOs;

namespace Unitta.Web.Models;

public class UnitNoCreateViewModel
{
    // The DTO containing the data to be submitted
    public UnitNoCreateDto? UnitNo { get; set; }

    // The list of options for the <select> dropdown
    public IEnumerable<SelectListItem>? UnitList { get; set; }
}