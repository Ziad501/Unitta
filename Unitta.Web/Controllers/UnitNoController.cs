using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Unitta.Application.Interfaces;
using Unitta.Application.Mappers;
using Unitta.Application.Utility;
using Unitta.Web.Models;

namespace Unitta.Web.Controllers;
public class UnitNoController(IUnitNoRepository _repo,
    IUnitRepository unitRepo, ILogger<UnitNoController> _logger) : Controller
{
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_AdminView)]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Fetching all unit numbers from the database.");
        var unitNumbers = await _repo.GetAllAsync();
        var unitNumbersDto = unitNumbers.ToUnitNoDtoList();
        return View(unitNumbersDto);
    }
    [HttpGet]
    [Authorize(Roles = SD.Role_Admin)]

    public async Task<IActionResult> Create()
    {
        _logger.LogInformation("Rendering the Create Unit Number view.");
        var viewModel = new UnitNoCreateViewModel();
        viewModel.UnitList = await GetUnitSelectListAsync(onlyUnassigned: true);
        return View(viewModel);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = SD.Role_Admin)]

    public async Task<IActionResult> Create(UnitNoCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Validation failed when creating a unit number.");
            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            viewModel.UnitList = await GetUnitSelectListAsync(viewModel.UnitNo.UnitId, onlyUnassigned: true);
            return View(viewModel);
        }

        var newUnitNo = viewModel.UnitNo.ToUnitNo();
        await _repo.CreateAsync(newUnitNo);

        TempData["SuccessMessage"] = $"Unit Number '{newUnitNo.UnitNumber}' created successfully.";
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    [Authorize(Roles = SD.Role_Admin)]

    public async Task<IActionResult> Edit(int id)
    {
        _logger.LogInformation("Attempting to find unit number with ID {UnitId} for editing.", id);
        var unitNo = await _repo.GetByUnitNumberAsync(id);
        if (unitNo == null)
        {
            _logger.LogWarning("Unit number with ID {UnitId} NOT FOUND for editing.", id);
            TempData["ErrorMessage"] = $"Unit Number with ID {id} not found.";
            return RedirectToAction("Error", "Home");
        }
        var updateViewModel = new UnitNoUpdateViewModel
        {
            UnitNo = unitNo.ToUnitNoUpdateDto()
        };
        updateViewModel.UnitList = await GetUnitSelectListAsync(unitNo.UnitId);
        return View(updateViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = SD.Role_Admin)]

    public async Task<IActionResult> Edit(UnitNoUpdateViewModel viewmodel)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state is invalid for unit number update.");
            TempData["ErrorMessage"] = "The information submitted is invalid. Please correct the errors.";
            viewmodel.UnitList = await GetUnitSelectListAsync(viewmodel.UnitNo.UnitId);
            return View(viewmodel);
        }

        var unitNoToUpdate = await _repo.GetByUnitNumberAsync(viewmodel.UnitNo.UnitNumber);
        if (unitNoToUpdate == null)
        {
            _logger.LogWarning("Unit number with ID {UnitId} NOT FOUND for updating.", viewmodel.UnitNo.UnitNumber);
            TempData["ErrorMessage"] = $"Unit Number with ID {viewmodel.UnitNo.UnitNumber} not found.";
            return RedirectToAction("Error", "Home");
        }

        viewmodel.UnitNo.UpdateUnitNoFromDto(unitNoToUpdate);

        await _repo.UpdateAsync(unitNoToUpdate);

        TempData["SuccessMessage"] = $"Unit Number '{unitNoToUpdate.UnitNumber}' was updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]

    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Attempting to delete unit number with ID {UnitId}.", id);
        var unitNoToDelete = await _repo.GetByUnitNumberAsync(id);
        if (unitNoToDelete == null)
        {
            _logger.LogWarning("Unit number with ID {UnitId} NOT FOUND for deletion.", id);
            TempData["ErrorMessage"] = $"Unit Number with ID {id} not found.";
            return RedirectToAction("Error", "Home");
        }
        await _repo.DeleteAsync(unitNoToDelete);
        TempData["SuccessMessage"] = $"Unit Number '{unitNoToDelete.UnitNumber}' deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IEnumerable<SelectListItem>> GetUnitSelectListAsync(int? selectedUnitId = null, bool onlyUnassigned = false)
    {
        var units = onlyUnassigned
            ? await unitRepo.GetAllAsync(u => u.UnitNo == null)
            : await unitRepo.GetAllAsync();

        return units.Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString(),
            Selected = u.Id == selectedUnitId
        });
    }
}

