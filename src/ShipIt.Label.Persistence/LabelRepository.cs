using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace ShipIt.Label.Persistence;

public class LabelRepository(IOptions<LabelRepositoryOptions> _options) : ILabelRepository
{
    public async Task<byte[]> ReadAsync(string trackingNumber)
    {
        var blob = await GetBlobClient(trackingNumber).DownloadContentAsync();
        return blob.Value.Content.ToArray();
    }

    public Task WriteAsync(byte[] content, string trackingNumber)
    {
        return GetBlobClient(trackingNumber).UploadAsync(new BinaryData(content));
    }

    private BlobClient GetBlobClient(string trackingNumber)
    {
        var client = new BlobServiceClient(_options.Value.ConnectionString);
        var containerClient = client.GetBlobContainerClient(_options.Value.ContainerName);
        return containerClient.GetBlobClient(ToFileName(trackingNumber));
    }

    private static string ToFileName(string trackingNumber)
    {
        return $"label-for-{trackingNumber}.pdf";
    }

}

