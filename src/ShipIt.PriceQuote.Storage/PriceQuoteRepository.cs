using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using ShipIt.PriceQuote.Storage.DataModel;

namespace ShipIt.PriceQuote.Storage;

public class PriceQuoteRepository(IOptions<PriceQuoteRepositoryOptions> options) : IPriceQuoteRepository
{
    public async Task<PriceQuoteDataModel> CreateAsync(PriceQuoteDataModel dataModel)
    {
        dataModel.id = Guid.NewGuid().ToString();

        var createdQuote = await GetCosmosContainer().CreateItemAsync(
            item: dataModel,
            partitionKey: new PartitionKey(dataModel.id)
        );

        return createdQuote.Resource;
    }

    public async Task<PriceQuoteDataModel> GetAsync(string Id)
    {
        var existing = await GetCosmosContainer().ReadItemAsync<PriceQuoteDataModel>(
            id: Id,
            partitionKey: new PartitionKey(Id)
        );

        return existing.Resource; 
    }

    private Container GetCosmosContainer() 
    {
        var client = new CosmosClient(options.Value.Connectionstring);
        var database = client.GetDatabase(options.Value.DatabaseName);
        var container = database.GetContainer(options.Value.ContainerName);

        if (container is null)
        {
            throw new Exception("Could not obtain cosmos container.");
        }

        return container;
    }
}
