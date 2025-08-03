using Microsoft.AspNetCore.Mvc;
using Unitta.Application.DTOs;
using Unitta.Application.Interfaces;
using Unitta.Application.Mappers;

namespace Unitta.Web.Controllers;

public class UnitController(IUnitRepository _unit, ILogger<UnitController> _logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Fetching all units from the database.");
        var units = await _unit.GetAllAsync();
        var unitDtos = units.ToUnitDtoList();
        return View(unitDtos);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UnitCreateDto unitDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Create validation failed for unit with name: {UnitName}", unitDto.Name);
            return View(unitDto);
        }
        var newUnit = unitDto.ToUnit();

        await _unit.CreateAsync(newUnit);
        _logger.LogInformation("Unit '{UnitName}' created successfully.", newUnit.Name);
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        _logger.LogInformation("Attempting to find unit with ID {UnitId} for editing.", id);
        var unit = await _unit.GetByIdAsync(id);
        if (unit == null)
        {
            _logger.LogWarning("Unit with ID {UnitId} NOT FOUND for editing.", id);
            return RedirectToAction("Error", "Home");
        }
        var unitDto = unit.ToUnitUpdateDto();
        return View(unitDto);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UnitUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Update validation failed for unit with ID {UnitId}.", dto.Id);
            return View(dto);
        }

        var unitToUpdate = await _unit.GetByIdAsync(dto.Id);
        if (unitToUpdate == null)
        {
            _logger.LogWarning("Unit with ID {UnitId} NOT FOUND during update attempt.", dto.Id);
            return RedirectToAction("Error", "Home");
        }
        UnitMapper.UpdateUnitFromDto(dto, unitToUpdate); //(maps the DTO onto the existing entity)
        unitToUpdate.UpdatedDate = DateTime.UtcNow;
        await _unit.UpdateAsync(unitToUpdate);
        _logger.LogInformation("Unit with ID {UnitId} updated successfully.", dto.Id);
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Attempting to delete unit with ID {UnitId}.", id);
        if (!await _unit.ExistsAsync(id))
        {
            _logger.LogWarning("Unit with ID {UnitId} NOT FOUND for deletion.", id);
            return RedirectToAction("Error", "Home");
        }
        await _unit.DeleteAsync(id);
        _logger.LogInformation("Unit with ID {UnitId} deleted successfully.", id);
        return RedirectToAction(nameof(Index));
    }
}
