using System;
using QuestPDF.Fluent;
using ShipIt.Label.Domain.DTO;
using ShipIt.Label.Domain.Model;
using ShipIt.Label.Persistence;

namespace ShipIt.Label.Domain;

public class DomainController(ILabelRepository _repo) : IDomainController
{
    public async Task<string> CreateLabelAsync(LabelCreationDTO labelCreation)
    {
        var addresModel = new Address(labelCreation);
        var label = new ShipmentLabel(addresModel, labelCreation.ShipmentPrice);

        await _repo.WriteAsync(label.GeneratePdf(), label.TrackingNumber);

        return label.TrackingNumber;
    }

    public Task<byte[]> GetLabelAsync(string trackingNumber)
    {
        return _repo.ReadAsync(trackingNumber);
    }
}
