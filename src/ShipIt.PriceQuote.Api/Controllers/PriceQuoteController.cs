using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShipIt.PriceQuote.Api.Contracts;
using ShipIt.PriceQuote.Service;

namespace ShipIt.PriceQuote.Api.Controllers;

[ApiController]
[Authorize]
[Route("pricequotes")]
public class PriceQuoteController(IPriceQuoteService quoteService) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("quote-creation-limiter")]
    [Authorize(Policy = "QuoteWritePolicy")]
    public async Task<ActionResult<PriceQuoteResponseContract>> CalculatePriceQuoteAsync([FromBody] PriceQuoteRequestContract priceQuoteRequest)
    {
        try
        {
            var created = await quoteService.CreateAsync(priceQuoteRequest);
            return CreatedAtAction(nameof(GetByIdAsync), new { Id = created.Id }, created);
        }
        catch (PriceQuoteExceptionNLWeightExceeded e)
        {
            return Problem(title: e.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "QuoteReadPolicy")]
    public async Task<ActionResult<PriceQuoteResponseContract>> GetByIdAsync([FromRoute] string Id)
    {
        return Ok(await quoteService.GetAsync(Id));
    }
}
