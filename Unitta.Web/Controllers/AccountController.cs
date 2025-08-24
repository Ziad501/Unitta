using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Unitta.Application.DTOs;
using Unitta.Application.Interfaces;
using Unitta.Application.Utility;
using Unitta.Infrastructure.Identity;
using Unitta.Web.Models;

namespace Unitta.Web.Controllers;

public class AccountController(
    UserManager<ApplicationUser> _userManager,
    SignInManager<ApplicationUser> _signInManager,
    RoleManager<IdentityRole> _roleManager,
    ILogger<AccountController> _logger,
    IEmailSender _emailSender) : Controller
{
    public IActionResult Login(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        LoginDto loginDto = new()
        {
            RedirectUrl = returnUrl
        };
        return View(loginDto);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        _logger.LogInformation("Login attempt started for email: {Email}", loginDto.Email);
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User with email {Email} logged in successfully.", user.Email);
                    if (await _userManager.IsInRoleAsync(user, SD.Role_Admin) ||
                        await _userManager.IsInRoleAsync(user, SD.Role_AdminView))
                    {
                        _logger.LogInformation("User {Email} is an admin or admin viewer. Redirecting to Admin Dashboard.", user.Email);
                        return RedirectToAction("Index", "Dashboard");
                    }
                    if (string.IsNullOrEmpty(loginDto.RedirectUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    return LocalRedirect(loginDto.RedirectUrl);
                }
                else
                {
                    _logger.LogWarning("Invalid password for user with email {Email}.", loginDto.Email);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            else
            {
                _logger.LogWarning("No user found with email {Email}.", loginDto.Email);
                ModelState.AddModelError(string.Empty, "User not found.");
            }
        }
        else
        {
            _logger.LogWarning("Invalid login attempt for email: {Email}. Model state is invalid.", loginDto.Email);
        }
        return View(loginDto);
    }

    public async Task<IActionResult> Register(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        _logger.LogInformation("Rendering the Register view with return URL: {ReturnUrl}", returnUrl);
        var roleList = (await _roleManager.Roles.ToListAsync())
            .Where(r => r.Name == SD.Role_Client || r.Name == SD.Role_AdminView)
            .Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            }).ToList();
        RegisterViewModel registerViewModel = new()
        {
            Roles = roleList,
            RegisterDto = new RegisterDto
            {
                RedirectUrl = returnUrl
            }
        };
        return View(registerViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        _logger.LogInformation("Registration attempt started for email: {Email}", viewModel.RegisterDto.Email);
        if (ModelState.IsValid)
        {
            ApplicationUser user = new()
            {
                Name = viewModel.RegisterDto.Name,
                UserName = viewModel.RegisterDto.Email,
                Email = viewModel.RegisterDto.Email,
                PhoneNumber = viewModel.RegisterDto.PhoneNumber,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, viewModel.RegisterDto.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with email {Email} created successfully.", user.Email);
                var roleToAssign = !string.IsNullOrEmpty(viewModel.RegisterDto.Role) ? viewModel.RegisterDto.Role : SD.Role_Client;
                _logger.LogInformation("Assigning role '{Role}' to user {Email}.", roleToAssign, user.Email);
                await _userManager.AddToRoleAsync(user, roleToAssign);

                // ADDED: Send welcome email to new user
                try
                {
                    var subject = "Welcome to Unitta!";
                    var body = $"Hi {user.Name},<br/><br/>Thank you for registering with Unitta. We're excited to have you!";
                    await _emailSender.SendEmailAsync(user.Email, subject, body);
                    _logger.LogInformation("Welcome email sent to {Email}", user.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send welcome email to {Email}", user.Email);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                if (roleToAssign == SD.Role_AdminView)
                {
                    _logger.LogInformation("User {Email} registered as AdminView. Redirecting to Dashboard.", user.Email);
                    return RedirectToAction("Index", "Dashboard");
                }
                var redirectUrl = string.IsNullOrEmpty(viewModel.RegisterDto.RedirectUrl) ? Url.Action("Index", "Home") : viewModel.RegisterDto.RedirectUrl;
                _logger.LogInformation("User {Email} signed in successfully. Redirecting to {RedirectUrl}.", user.Email, redirectUrl);
                return LocalRedirect(redirectUrl);
            }
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Failed to create user with email {Email}. Errors: {Errors}", user.Email, errors);
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        else
        {
            _logger.LogWarning("Invalid registration attempt for email: {Email}. Model state is invalid.", viewModel.RegisterDto.Email);
        }
        viewModel.Roles = (await _roleManager.Roles.ToListAsync())
            .Where(r => r.Name == SD.Role_Client || r.Name == SD.Role_AdminView)
            .Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            }).ToList();
        return View(viewModel);
    }

    public IActionResult AccessDenied()
    {
        _logger.LogWarning("Access denied page accessed.");
        return View();
    }
}