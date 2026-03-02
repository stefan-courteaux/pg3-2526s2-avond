using Microsoft.AspNetCore.Mvc;
using ShipIt.PriceQuote.Api.Contracts;
using ShipIt.PriceQuote.Service;

namespace ShipIt.PriceQuote.Api.Controllers;

[ApiController]
[Route("pricequotes")]
public class PriceQuoteController(IPriceQuoteService quoteService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PriceQuoteResponseContract>> CalculatePriceQuoteAsync([FromBody]PriceQuoteRequestContract priceQuoteRequest)
    {
        try
        {
            var created = await quoteService.CreateAsync(priceQuoteRequest);
            return CreatedAtAction(nameof(GetByIdAsync), new {Id = created.Id}, created);
        }
        catch(PriceQuoteExceptionNLWeightExceeded e)
        {
            return Problem(title: e.Message, statusCode: StatusCodes.Status400BadRequest);   
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PriceQuoteResponseContract>> GetByIdAsync([FromRoute]string Id)
    {
        return Ok(await quoteService.GetAsync(Id));
    }
}
