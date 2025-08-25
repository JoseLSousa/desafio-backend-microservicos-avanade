namespace Stock.Application.UseCases.Commands
{
    public sealed record CreateItemCommand(string Name, int Quantity, int Price);
}
