namespace ShipIt.Label.Persistence;

public interface ILabelRepository
{
    Task WriteAsync(byte[] content, string trackingNumber);
    Task<byte[]> ReadAsync(string trackingNumber);
}
