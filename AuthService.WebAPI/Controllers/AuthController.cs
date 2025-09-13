using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;
using AuthService.Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService
        ) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            try
            {
                var userExists = await userManager.FindByEmailAsync(userRegisterDto.Email);

                if (userExists != null) return Conflict(new { Message = "User already exists!" });

                ApplicationUser newUser = new()
                {
                    Email = userRegisterDto.Email,
                    UserName = userRegisterDto.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CPF = userRegisterDto.CPF
                };

                var result = await userManager.CreateAsync(newUser, userRegisterDto.Password);

                if (!result.Succeeded) return BadRequest(new
                {
                    Message = "User creation failed! Please check user details and try again.",
                    Errors = result.Errors.Select(e => e.Description)
                });
                return Ok(new { Message = "User created successfully!" });
            }
            catch (Exception)
            {

                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(userLoginDto.Email);

                if (user == null) return Unauthorized(new
                {
                    Message = "Invalid email or password"
                });

                var result = await signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, false);

                if (!result.Succeeded) return Unauthorized(new
                {
                    Message = "Invalid email or password"
                });
                var token = jwtService.GenerateToken(user);
                return Ok(new
                {
                    Token = token
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
