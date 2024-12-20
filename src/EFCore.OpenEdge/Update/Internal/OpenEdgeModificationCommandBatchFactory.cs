using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkCore.OpenEdge.Update.Internal;

public class OpenEdgeModificationCommandBatchFactory(ModificationCommandBatchFactoryDependencies dependencies) : IModificationCommandBatchFactory
{
    private readonly ModificationCommandBatchFactoryDependencies _dependencies = dependencies;

    public virtual ModificationCommandBatch Create() => new OpenEdgeSingularModificationCommandBatch(_dependencies);
}