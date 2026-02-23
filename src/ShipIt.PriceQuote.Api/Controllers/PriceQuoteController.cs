using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShipIt.PriceQuote.Api.Contracts;

namespace ShipIt.PriceQuote.Api.Controllers;

[ApiController]
[Route("pricequotes")]
public class PriceQuoteController : ControllerBase
{
    [HttpPost]
    public ActionResult<PriceQuoteResponseContract> CalculatePriceQuote([FromBody]PriceQuoteRequestContract priceQuoteRequest)
    {
        if(priceQuoteRequest.FromCountry == CountryEnum.NL && priceQuoteRequest.WeightKg > 10)
        {
            return Problem(title: "For shipments from the Netherlands, the maximum weight is 10 kg.", statusCode: StatusCodes.Status400BadRequest);
        }

        var price = 2.5 + priceQuoteRequest.LengthCm * priceQuoteRequest.WidthCm * priceQuoteRequest.HeightCm * 0.0001 + priceQuoteRequest.WeightKg * 1.15;
        if(priceQuoteRequest.FromCountry != priceQuoteRequest.ToCountry)
        {
            price += 3.0;
        }

        return Ok(new PriceQuoteResponseContract
        {
            CreatedOn = DateTime.UtcNow,
            ValidUntil = DateTime.UtcNow.AddHours(48),
            Price = decimal.Round((decimal)price, 1)
        });
    }
}
