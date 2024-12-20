//using System;
//using System.Linq.Expressions;
//using EntityFrameworkCore.OpenEdge.Query.ExpressionVisitors.Internal;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Diagnostics;
//using Microsoft.EntityFrameworkCore.Infrastructure;
//using Microsoft.EntityFrameworkCore.Internal;
//using Microsoft.EntityFrameworkCore.Query;
//using Microsoft.EntityFrameworkCore.Query.Internal;

//namespace EntityFrameworkCore.OpenEdge.Query.Internal;

//public class OpenEdgeQueryModelGenerator : QueryModelGenerator
//{
//    private readonly IEvaluatableExpressionFilter _evaluatableExpressionFilter;
//    private readonly ICurrentDbContext _currentDbContext;

//    public OpenEdgeQueryModelGenerator(INodeTypeProviderFactory nodeTypeProviderFactory,
//        IEvaluatableExpressionFilter evaluatableExpressionFilter,
//        ICurrentDbContext currentDbContext)
//        : base(nodeTypeProviderFactory, evaluatableExpressionFilter, currentDbContext)
//    {
//        _evaluatableExpressionFilter = evaluatableExpressionFilter;
//        _currentDbContext = currentDbContext;
//    }

//#pragma warning disable EF1001 // Internal EF Core API usage.
//    public override Expression ExtractParameters(IDiagnosticsLogger<DbLoggerCategory.Query> logger, Expression query, IParameterValues parameterValues,
//#pragma warning restore EF1001 // Internal EF Core API usage.
//        bool parameterize = true, bool generateContextAccessors = false)
//    {
//        return new OpenEdgeParameterExtractingExpressionVisitor(_evaluatableExpressionFilter, parameterValues, logger,
//            _currentDbContext.Context,
//            parameterize, generateContextAccessors).ExtractParameters(query);
//    }
//}