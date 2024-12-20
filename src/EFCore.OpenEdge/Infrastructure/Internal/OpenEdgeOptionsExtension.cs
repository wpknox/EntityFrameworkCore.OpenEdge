using EntityFrameworkCore.OpenEdge.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace EntityFrameworkCore.OpenEdge.Infrastructure.Internal;

public class OpenEdgeOptionsExtension : RelationalOptionsExtension
{
    private DbContextOptionsExtensionInfo _info;
    public override DbContextOptionsExtensionInfo Info => _info ??= (new OpenEdgeDbContextOptionsExtensionInfo(this));

    protected override RelationalOptionsExtension Clone()
    {
        return new OpenEdgeOptionsExtension();
    }

    public override void ApplyServices(IServiceCollection services)
    {
        services.AddEntityFrameworkOpenEdge();
    }

    private sealed class OpenEdgeDbContextOptionsExtensionInfo(IDbContextOptionsExtension instance) : DbContextOptionsExtensionInfo(instance)
    {
        public override bool IsDatabaseProvider => true;
        public override string LogFragment => "";
        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo) { }
        public override int GetServiceProviderHashCode() => 0;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        {
            return other is OpenEdgeDbContextOptionsExtensionInfo;
        }
    }
}