using System;
using ShipIt.PriceQuote.Api.Contracts;

namespace ShipIt.PriceQuote.Service;

public interface IPriceQuoteService
{
    Task<PriceQuoteResponseContract> CreateAsync(PriceQuoteRequestContract requestContract);
    Task<PriceQuoteResponseContract> GetAsync(string Id);
}
