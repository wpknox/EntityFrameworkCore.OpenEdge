using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.OpenEdge.Query.Internal;
public class OpenEdgeQueryTranslationPreprocessorFactory(QueryTranslationPreprocessorDependencies dependencies,
                                                         RelationalQueryTranslationPreprocessorDependencies relationalDependencies)
                                                       : IQueryTranslationPreprocessorFactory
{
    protected QueryTranslationPreprocessorDependencies Dependencies { get; } = dependencies;

    protected readonly RelationalQueryTranslationPreprocessorDependencies RelationalDependencies = relationalDependencies;

    public QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
        => new OpenEdgeQueryTranslationPreprocessor(Dependencies, RelationalDependencies, queryCompilationContext);
}
