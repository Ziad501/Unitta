using Microsoft.AspNetCore.Mvc;
using Unitta.Application.Interfaces;
using Unitta.Application.Mappers;
using Unitta.Web.Models;

namespace Unitta.Web.Controllers;

public class HomeController(
    IUnitRepository _repo,
    ILogger<HomeController> _logger) : Controller
{
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Attempting to fetch all units from database");
        var unitsFromDb = await _repo.GetAllAsync();
        var unitsDto = unitsFromDb.ToUnitDtoList();

        HomeViewModel home = new()
        {
            Units = unitsDto,
            CheckInDate = DateOnly.FromDateTime(DateTime.Now),
            NumberOfNights = 1
        };
        return View(home);
    }

    [HttpPost]
    public async Task<IActionResult> GetUnitsByDate(DateOnly checkInDate, int numberOfNights)
    {
        _logger.LogInformation("Attempting to filter units by date.");

        var checkOutDate = checkInDate.AddDays(numberOfNights);

        var allUnitsFromDb = await _repo.GetAllAsync();
        var availableUnitsFromDb = await _repo.GetAvailableUnitsAsync(checkInDate, checkOutDate);

        var allUnitsDto = allUnitsFromDb.ToUnitDtoList();
        var availableUnitsDto = availableUnitsFromDb.ToUnitDtoList();

        var availableUnitIds = availableUnitsDto.Select(u => u.Id).ToHashSet();

        var homeViewModel = new HomeViewModel
        {
            Units = allUnitsDto,
            CheckInDate = checkInDate,
            NumberOfNights = numberOfNights,
            AvailableUnitIds = availableUnitIds
        };

        return PartialView("_UnitList", homeViewModel);
    }

    public IActionResult Error()
    {
        _logger.LogError("An error occurred while processing your request.");
        return View();
    }
}