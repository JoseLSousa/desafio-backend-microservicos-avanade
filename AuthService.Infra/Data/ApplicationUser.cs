using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Infra.Data
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public string CPF { get; set; } = default!;
        string IUser.Email => Email!;
    }
}
