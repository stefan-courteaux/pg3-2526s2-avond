using System;
using ShipIt.Label.Domain.DTO;

namespace ShipIt.Label.Domain;

public interface IDomainController
{
    Task<string> CreateLabelAsync(LabelCreationDTO labelCreation);

    Task<byte[]> GetLabelAsync(string trackingNumber);
}
