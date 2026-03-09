using System;

namespace ShipIt.Label.Persistence;

public class LabelRepositoryOptions
{
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
}
