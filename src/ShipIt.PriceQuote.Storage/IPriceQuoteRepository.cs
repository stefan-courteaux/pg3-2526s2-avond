using ShipIt.PriceQuote.Storage.DataModel;

namespace ShipIt.PriceQuote.Storage;

public interface IPriceQuoteRepository
{
    Task<PriceQuoteDataModel> CreateAsync(PriceQuoteDataModel dataModel);
    Task<PriceQuoteDataModel> GetAsync(string Id);
}
