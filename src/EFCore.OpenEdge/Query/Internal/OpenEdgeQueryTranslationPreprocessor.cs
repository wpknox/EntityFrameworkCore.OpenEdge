using EntityFrameworkCore.OpenEdge.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EntityFrameworkCore.OpenEdge.Query.Internal;
public class OpenEdgeQueryTranslationPreprocessor(QueryTranslationPreprocessorDependencies dependencies,
    RelationalQueryTranslationPreprocessorDependencies relationalDependencies,
    QueryCompilationContext queryCompilationContext) : RelationalQueryTranslationPreprocessor(dependencies, relationalDependencies, queryCompilationContext)
{

    public override Expression Process(Expression query) => base.Process(Preprocess(query));

#pragma warning disable EF1001 // Internal EF Core API usage.
    private Expression Preprocess(Expression query)
    {
        var res = new OpenEdgeParameterExtractingExpressionVisitor(QueryCompilationContext.Model,
                                                                   Dependencies.EvaluatableExpressionFilter,
                                                                   QueryCompilationContext.ContextType,
                                                                   QueryCompilationContext.Logger).Visit(query);
        return res;
    }
#pragma warning restore EF1001 // Internal EF Core API usage.
}
