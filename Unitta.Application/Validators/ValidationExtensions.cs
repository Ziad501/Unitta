using FluentValidation;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
namespace Unitta.Application.Validators;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, IFormFile> IsValidImage<T>(this IRuleBuilder<T, IFormFile> ruleBuilder)
    {
        const int maxFileSize = 5 * 1024 * 1024;

        return ruleBuilder
            .Must(file => file == null || file.Length <= maxFileSize)
            .WithMessage($"Image size must not exceed {maxFileSize / 1024 / 1024}MB.")
            .Must(BeAValidImageFile)
            .WithMessage("Please upload a valid image file (JPG, PNG, GIF, BMP, WebP, TIFF).");
    }

    private static bool BeAValidImageFile(IFormFile file)
    {
        if (file == null)
        {
            return true;
        }

        try
        {
            using var stream = file.OpenReadStream();

            stream.Position = 0;

            var info = Image.Identify(stream);

            var allowedFormats = new[] { "Jpeg", "jpg", "Png", "Gif", "Bmp", "WebP", "Tiff" };

            return allowedFormats.Contains(info.Metadata.DecodedImageFormat.Name, StringComparer.OrdinalIgnoreCase);
        }
        catch (Exception)
        {
            return false;
        }
    }
}