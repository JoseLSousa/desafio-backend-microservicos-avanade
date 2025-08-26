using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using Stock.Application.UseCases.Commands;
using Stock.Application.UseCases.Queries;
using Stock.Domain.Entitites;
using Stock.Domain.Interfaces;

namespace Stock.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StockController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await mediator.Send(command, cancellationToken);
                return Ok("Item created successfully.");
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpGet("{itemId:guid}")]
        public async Task<IActionResult> GetItemById(Guid itemId, CancellationToken cancellationToken)
        {
            var requestClient = mediator.CreateRequestClient<GetItemByIdQuery>();

            var response = await requestClient.GetResponse<StockItem>(new GetItemByIdQuery(itemId), cancellationToken);

            if (response == null)
                return NotFound(new { Message = $"Item with ID {itemId} not found." });

            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(IStockRepository repository, CancellationToken cancellation)
        {
            return Ok(await repository.GetAll(cancellation));
        }

    }
}
