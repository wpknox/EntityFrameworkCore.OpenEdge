using EntityFrameworkCore.OpenEdge.Infrastructure.Internal;
using EntityFrameworkCore.OpenEdge.Metadata.Conventions.Internal;
using EntityFrameworkCore.OpenEdge.Query.ExpressionTranslators.Internal;
using EntityFrameworkCore.OpenEdge.Query.Internal;
using EntityFrameworkCore.OpenEdge.Query.Sql.Internal;
using EntityFrameworkCore.OpenEdge.Storage;
using EntityFrameworkCore.OpenEdge.Storage.Internal;
using EntityFrameworkCore.OpenEdge.Storage.Internal.Mapping;
using EntityFrameworkCore.OpenEdge.Update;
using EntityFrameworkCore.OpenEdge.Update.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.OpenEdge.Extensions;

public static class OpenEdgeServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkOpenEdge(this IServiceCollection serviceCollection)
    {
#pragma warning disable EF1001 // Internal EF Core API usage.
        var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
            .TryAdd<IBatchExecutor, BatchExecutor>()
            .TryAdd<IAsyncQueryProvider, EntityQueryProvider>()
            .TryAdd<IQueryCompiler, QueryCompiler>()
            .TryAdd<IDatabaseProvider, DatabaseProvider<OpenEdgeOptionsExtension>>()
            .TryAdd<IRelationalTypeMappingSource, OpenEdgeTypeMappingSource>()
            .TryAdd<ISqlGenerationHelper, OpenEdgeSqlGenerationHelper>()
            .TryAdd<IEvaluatableExpressionFilter, RelationalEvaluatableExpressionFilter>()
            .TryAdd<RelationalConventionSetBuilder, OpenEdgeRelationalConventionSetBuilder>()
            .TryAdd<IUpdateSqlGenerator, OpenEdgeUpdateSqlGenerator>()
            .TryAdd<IRelationalConnection>(p => p.GetService<IOpenEdgeRelationalConnection>())
            .TryAdd<IQueryTranslationPreprocessorFactory, OpenEdgeQueryTranslationPreprocessorFactory>()
            .TryAdd<IMemberTranslatorProvider, OpenEdgeCompositeMemberTranslator>()
            .TryAdd<IMethodCallTranslatorProvider, OpenEdgeCompositeMethodCallTranslator>()
            .TryAdd<IQuerySqlGeneratorFactory, OpenEdgeSqlGeneratorFactory>()
            .TryAdd<IModificationCommandBatchFactory, OpenEdgeModificationCommandBatchFactory>()

            .TryAddProviderSpecificServices(b => b
                .TryAddScoped<IOpenEdgeUpdateSqlGenerator, OpenEdgeUpdateSqlGenerator>()
                .TryAddScoped<IOpenEdgeRelationalConnection, OpenEdgeRelationalConnection>()); ;
#pragma warning restore EF1001 // Internal EF Core API usage.

        builder.TryAddCoreServices();
        return serviceCollection;
    }

}