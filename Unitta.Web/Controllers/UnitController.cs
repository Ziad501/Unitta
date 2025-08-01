using Microsoft.AspNetCore.Mvc;
using Unitta.Application.Interfaces;

namespace Unitta.Web.Controllers;

public class UnitController(IUnitRepository _unit) : Controller
{
    public IActionResult Index()
    {
        var units = _unit.GetAllAsync();
        return View();
    }
}
