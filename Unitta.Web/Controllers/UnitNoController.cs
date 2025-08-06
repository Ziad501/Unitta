using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Unitta.Application.Interfaces;
using Unitta.Application.Mappers;
using Unitta.Web.Models;

namespace Unitta.Web.Controllers
{
    public class UnitNoController(IUnitNoRepository _repo,
        IUnitRepository unitRepo, ILogger<UnitNoController> _logger) : Controller
    {
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching all unit numbers from the database.");
            var unitNumbers = await _repo.GetAllAsync();
            var unitNumbersDto = unitNumbers.ToUnitNoDtoList();
            return View(unitNumbersDto);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Rendering the Create Unit Number view.");
            var viewModel = new UnitNoCreateViewModel();
            await PopulateUnitListAsync(viewModel);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitNoCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed when creating a unit number.");
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                await PopulateUnitListAsync(viewModel);
                return View(viewModel);
            }

            var newUnitNo = viewModel.UnitNo.ToUnitNo();
            await _repo.CreateAsync(newUnitNo);

            TempData["SuccessMessage"] = $"Unit Number '{newUnitNo.UnitNumber}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateUnitListAsync(UnitNoCreateViewModel viewModel)
        {
            // Fetch from the correct repository (_unitRepo)
            var units = await unitRepo.GetAllAsync(u => u.UnitNo == null);

            viewModel.UnitList = units.Select(u => new SelectListItem
            {
                // Now 'u' is a Unit object directly, so no need for 'u.Unit.Name'
                Text = u.Name,
                Value = u.Id.ToString()
            });
        }

    }
}
