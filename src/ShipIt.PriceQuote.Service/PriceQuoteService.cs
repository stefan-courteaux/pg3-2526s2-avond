using System;
using ShipIt.PriceQuote.Api.Contracts;
using ShipIt.PriceQuote.Service.Model;
using ShipIt.PriceQuote.Storage;
using ShipIt.PriceQuote.Storage.DataModel;
using ShipIt.Shared;

namespace ShipIt.PriceQuote.Service;

public class PriceQuoteService(IPriceQuoteRepository priceQuoteRepository) : IPriceQuoteService
{
    //TODO Factor out mapping logic.
    public async Task<PriceQuoteResponseContract> CreateAsync(PriceQuoteRequestContract requestContract)
    {
        // Map to Model
        var model = new PriceQuoteModel
        {
            PriceArguments = new PriceQuoteModelArguments
            {
                WidthCm = requestContract.WidthCm,
                HeightCm = requestContract.HeightCm,
                LengthCm = requestContract.LengthCm,
                WeightKg = requestContract.WeightKg,
                FromCountry = requestContract.FromCountry,
                ToCountry = requestContract.ToCountry
            }
        };

        // Business Logic
        if (model.PriceArguments.FromCountry == CountryEnum.NL && model.PriceArguments.WeightKg > 10)
        {
            throw new PriceQuoteExceptionNLWeightExceeded(
                $"For shipments from NL, the maximum weight is 10kg. You submitted {model.PriceArguments.WeightKg}");
        }

        var price = 2.5 + model.PriceArguments.LengthCm * model.PriceArguments.WidthCm * model.PriceArguments.HeightCm * 0.0001 + model.PriceArguments.WeightKg * 1.15;
        if (model.PriceArguments.FromCountry != model.PriceArguments.ToCountry)
        {
            price += 3.0;
        }

        model.PriceResult = new PriceQuoteModelResult
        {
            Price = decimal.Round((decimal)price, 1),
            CreatedOn = DateTime.UtcNow,
            ValidUntil = DateTime.UtcNow.AddHours(48)
        };

        // Stel u voor: nog meer logica enzo op model


        // Map to DataModel
        var dbQuote = new PriceQuoteDataModel
        {
            widthCm = model.PriceArguments.WidthCm,
            heightCm = model.PriceArguments.HeightCm,
            lengthCm = model.PriceArguments.LengthCm,
            weightKg = model.PriceArguments.WeightKg,
            fromCountry = model.PriceArguments.FromCountry,
            toCountry = model.PriceArguments.ToCountry,
            price = model.PriceResult.Price,
            createdOn = model.PriceResult.CreatedOn,
            validUntil = model.PriceResult.ValidUntil
        };

        // Create in DB
        var created = await priceQuoteRepository.CreateAsync(dbQuote);

        // Map to Model, then Contract...

        // Map to Contract
        var responseContract = new PriceQuoteResponseContract
        {
            Id = created.id,
            CreatedOn = created.createdOn,
            ValidUntil = created.validUntil,
            Price = created.price,
            CountryFrom = created.fromCountry
        };

        return responseContract;
    }

    //TODO refactor duplicate logic
    public async Task<PriceQuoteResponseContract> GetAsync(string Id)
    {
        var existing = await priceQuoteRepository.GetAsync(Id);

        var responseContract = new PriceQuoteResponseContract
        {
            Id = existing.id,
            CreatedOn = existing.createdOn,
            ValidUntil = existing.validUntil,
            Price = existing.price
        };

        return responseContract;
    }
}
