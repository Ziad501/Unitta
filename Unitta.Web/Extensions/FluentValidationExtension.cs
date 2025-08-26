using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Unitta.Web.Extensions;

public static class FluentValidationExtensions
{
    /// <summary>
    /// Adds validation errors from a FluentValidation result to the ASP.NET Core ModelState.
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <param name="modelState">The ModelStateDictionary to add errors to.</param>
    /// <param name="prefix">An optional prefix for the model state keys.</param>
    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState, string? prefix = null)
    {
        if (result.IsValid)
        {
            return;
        }

        foreach (var error in result.Errors)
        {
            var key = string.IsNullOrEmpty(prefix)
                ? error.PropertyName
                : $"{prefix}.{error.PropertyName}";

            modelState.AddModelError(key, error.ErrorMessage);
        }
    }
}