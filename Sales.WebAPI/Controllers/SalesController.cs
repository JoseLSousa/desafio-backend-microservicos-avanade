using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Commands;
using Sales.Application.DTOs;
using Sales.Domain.Interfaces;

namespace Sales.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SalesController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] List<SaleItemDto> items)
        {
            try
            {
                if (items == null || items.Count == 0)
                    return BadRequest("Sale must contain at least one item.");
                var saleId = Guid.NewGuid();

                await mediator.Send(new CreateSaleCommand(saleId, items));

                return Accepted(new { SaleId = saleId, Status = "Pending" });
            }
            catch (Exception)
            {
                return BadRequest("An error ocurred");
                throw;
            }
        }

        [HttpGet("{saleId:guid}")]
        public async Task<IActionResult> getStatus(Guid saleId, [FromServices] ISaleRepository saleRepository)
        {
            var sale = await saleRepository.GetByIdAsync(saleId, CancellationToken.None);
            if (sale == null)
                return NotFound("Sale not found");

            return Ok(new { SaleId = sale.Id, Status = sale.Status.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(ISaleRepository sale, CancellationToken cancellationToken)
        {
            return Ok(await sale.GetAll(cancellationToken));
        }
    }
}
