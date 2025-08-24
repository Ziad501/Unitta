using Microsoft.AspNetCore.Mvc.Rendering;
using Unitta.Application.DTOs;

namespace Unitta.Web.Models
{
    public class RegisterViewModel
    {
        public RegisterDto RegisterDto { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}
