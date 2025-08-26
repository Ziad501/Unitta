using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Unitta.Application.DTOs;
using Unitta.Application.Interfaces;
using Unitta.Application.Mappers;
using Unitta.Application.Utility;
using Unitta.Domain.Entities;
using Unitta.Web.Models;

namespace Unitta.Web.Controllers;
public class FeaturesController(
    IFeatureRepository _featureRepository,
    IUnitRepository _unitRepository,
    ILogger<FeaturesController> _logger,
    IValidator<FeatureCreateDto> _createValidator,
    IValidator<FeatureUpdateDto> _updateValidator) : Controller
{
    [HttpGet]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_AdminView)]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Attempting to fetch all features.");
        var features = await _featureRepository.GetAllAsync();
        var featureDtos = features.ToFeatureDtoList();
        _logger.LogInformation("Successfully fetched {FeatureCount} features.", featureDtos.Count());
        return View(featureDtos);
    }

    [HttpGet]
    [Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> Create()
    {
        _logger.LogInformation("Rendering the Create Feature view.");
        var viewModel = new FeatureCreateViewModel
        {
            UnitList = await GetUnitsAsSelectList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> Create(FeatureCreateViewModel viewModel)
    {
        var validationResult = await _createValidator.ValidateAsync(viewModel.Feature);
        validationResult.AddToModelState(ModelState, nameof(viewModel.Feature));

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state is invalid for creating a new feature.");
            TempData["ErrorMessage"] = "Please correct the validation errors and try again.";
            viewModel.UnitList = await GetUnitsAsSelectList(viewModel.Feature.UnitId);
            return View(viewModel);
        }

        var feature = viewModel.Feature.ToFeature();
        await _featureRepository.CreateAsync(feature);
        _logger.LogInformation("Successfully created new feature with ID {FeatureId}.", feature.Id);
        TempData["SuccessMessage"] = $"Feature '{feature.Name}' was created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = SD.Role_Admin)]

    public async Task<IActionResult> Edit(int id)
    {
        _logger.LogInformation("Attempting to find feature with ID {FeatureId} for editing.", id);
        var feature = await _featureRepository.GetByIdAsync(id);
        if (feature == null)
        {
            _logger.LogWarning("Feature with ID {FeatureId} NOT FOUND for editing.", id);
            TempData["ErrorMessage"] = "The requested feature could not be found.";
            return RedirectToAction("Error", "Home");
        }

        var viewModel = new FeatureEditViewModel
        {
            Feature = feature.ToUpdateFeatureDto(),
            UnitList = await GetUnitsAsSelectList(feature.UnitId)
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> Edit(int id, FeatureEditViewModel viewModel)
    {
        var validationResult = await _updateValidator.ValidateAsync(viewModel.Feature);
        validationResult.AddToModelState(ModelState, nameof(viewModel.Feature));
        if (id != viewModel.Feature.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state is invalid for updating feature with ID {FeatureId}.", id);
            TempData["ErrorMessage"] = "Please correct the validation errors.";
            viewModel.UnitList = await GetUnitsAsSelectList(viewModel.Feature.UnitId);
            return View(viewModel);
        }

        var feature = await _featureRepository.GetByIdAsync(id);
        if (feature == null)
        {
            _logger.LogWarning("Feature with ID {FeatureId} NOT FOUND during update attempt.", id);
            TempData["ErrorMessage"] = "The feature could not be found.";
            return RedirectToAction("Error", "Home");
        }

        viewModel.Feature.UpdateFeatureFromDto(feature);
        await _featureRepository.UpdateAsync(feature);
        _logger.LogInformation("Successfully updated feature with ID {FeatureId}.", id);
        TempData["SuccessMessage"] = $"Feature '{feature.Name}' was updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = SD.Role_Admin)]

    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Attempting to delete feature with ID {FeatureId}.", id);

        var featureToDelete = await _featureRepository.GetByIdAsync(id);

        if (featureToDelete == null)
        {
            _logger.LogWarning("Feature with ID {FeatureId} NOT FOUND for deletion.", id);
            TempData["ErrorMessage"] = "The feature could not be found or was already deleted.";
            return RedirectToAction("Error", "Home");
        }

        await _featureRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task<IEnumerable<SelectListItem>> GetUnitsAsSelectList(object? selectedValue = null)
    {
        var units = await _unitRepository.GetAllAsync();

        return new SelectList(units, nameof(Unit.Id), nameof(Unit.Name), selectedValue);
    }
}