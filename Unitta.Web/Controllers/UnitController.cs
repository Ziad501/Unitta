using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unitta.Application.DTOs;
using Unitta.Application.Interfaces;
using Unitta.Application.Mappers;
using Unitta.Application.Utility;
using Unitta.Web.Extensions;

namespace Unitta.Web.Controllers;
[Authorize(Roles = SD.Role_Admin + "," + SD.Role_AdminView)]
public class UnitController(
    IUnitRepository _unit,
    ILogger<UnitController> _logger,
    IWebHostEnvironment _hostEnvironment,
    IValidator<UnitCreateDto> _createValidator,
    IValidator<UnitUpdateDto> _updateValidator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Fetching all units from the database.");
        var units = await _unit.GetAllAsync();
        var unitDtos = units.ToUnitDtoList();
        ViewBag.IsAdminView = User.IsInRole(SD.Role_AdminView);
        return View(unitDtos);
    }
    [HttpGet]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> Create(UnitCreateDto unitDto)
    {
        /*
        If your action takes the DTO directly (e.g., 
        public IActionResult Action(MyDto dto)), 
        do not use a prefix: result.AddToModelState(ModelState);

        If your action takes a ViewModel that contains the DTO 
        (e.g., public IActionResult Action(MyViewModel vm) where vm has a property public MyDto Dto { get; set; }), 
        you must use the prefix:result.AddToModelState(ModelState, nameof(vm.Dto));*/
        var validationResult = await _createValidator.ValidateAsync(unitDto);
        validationResult.AddToModelState(ModelState);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Create validation failed for unit with name: {UnitName}", unitDto.Name);
            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            return View(unitDto);
        }

        var newUnit = unitDto.ToUnit();

        if (unitDto.Image != null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(unitDto.Image.FileName);
            string imagePath = Path.Combine(_hostEnvironment.WebRootPath, @"images/units");

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            await using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
            {
                await unitDto.Image.CopyToAsync(fileStream);
            }

            newUnit.ImageUrl = @"/images/units/" + fileName;
        }

        newUnit.CreatedDate = DateTime.UtcNow;

        await _unit.CreateAsync(newUnit);
        TempData["SuccessMessage"] = $"Unit '{newUnit.Name}' created successfully.";
        _logger.LogInformation("Unit '{UnitName}' created successfully.", newUnit.Name);
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    [Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> Edit(int id)
    {
        _logger.LogInformation("Attempting to find unit with ID {UnitId} for editing.", id);
        var unit = await _unit.GetByIdAsync(id);
        if (unit == null)
        {
            _logger.LogWarning("Unit with ID {UnitId} NOT FOUND for editing.", id);
            TempData["ErrorMessage"] = $"Unit with ID {id} not found.";
            return RedirectToAction("Error", "Home");
        }
        var unitDto = unit.ToUnitUpdateDto();
        return View(unitDto);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> Edit(UnitUpdateDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        validationResult.AddToModelState(ModelState);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Update validation failed for unit with ID {UnitId}.", dto.Id);
            dto.CurrentImageUrl = (await _unit.GetByIdAsync(dto.Id))?.ImageUrl;
            return View(dto);
        }

        var unitToUpdate = await _unit.GetByIdAsync(dto.Id);
        if (unitToUpdate == null)
        {
            _logger.LogWarning("Unit with ID {UnitId} NOT FOUND during update attempt.", dto.Id);

            TempData["ErrorMessage"] = $"Unit with ID {dto.Id} not found.";

            return RedirectToAction("Error", "Home");
        }
        UnitMapper.UpdateUnitFromDto(dto, unitToUpdate);

        if (dto.NewImage != null)
        {
            if (!string.IsNullOrEmpty(unitToUpdate.ImageUrl))
            {
                var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, unitToUpdate.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.NewImage.FileName);
            string imagePath = Path.Combine(_hostEnvironment.WebRootPath, @"images/units");

            await using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
            {
                await dto.NewImage.CopyToAsync(fileStream);
            }

            unitToUpdate.ImageUrl = @"/images/units/" + fileName;
        }
        unitToUpdate.UpdatedDate = DateTime.UtcNow;

        await _unit.UpdateAsync(unitToUpdate);
        TempData["SuccessMessage"] = $"Unit '{unitToUpdate.Name}' updated successfully.";
        _logger.LogInformation("Unit with ID {UnitId} updated successfully.", dto.Id);
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = SD.Role_Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Attempting to delete unit with ID {UnitId}.", id);
        var unitToDelete = await _unit.GetByIdAsync(id);

        if (unitToDelete == null)
        {
            _logger.LogWarning("Unit with ID {UnitId} NOT FOUND for deletion.", id);
            TempData["ErrorMessage"] = $"Unit with ID {id} not found.";
            return RedirectToAction("Error", "Home");
        }

        if (!string.IsNullOrEmpty(unitToDelete.ImageUrl))
        {
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, unitToDelete.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }

        await _unit.DeleteAsync(id);
        TempData["SuccessMessage"] = $"Unit with ID {id} deleted successfully.";
        _logger.LogInformation("Unit with ID {UnitId} deleted successfully.", id);
        return RedirectToAction(nameof(Index));
    }
}
