namespace AuthService.Domain.Entities
{
    public interface IUser
    {
        string Id { get; }
        string Email { get; }
        string CPF { get; }
    }
}
