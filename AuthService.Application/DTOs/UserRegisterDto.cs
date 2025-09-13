namespace AuthService.Application.DTOs
{
    public record UserRegisterDto(string Email, string Password, string CPF)
    {
    }
}
