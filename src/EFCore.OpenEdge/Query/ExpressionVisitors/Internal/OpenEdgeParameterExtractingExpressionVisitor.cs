using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFrameworkCore.OpenEdge.Query.ExpressionVisitors.Internal;

#pragma warning disable EF1001 // Internal EF Core API usage.
public class OpenEdgeParameterExtractingExpressionVisitor : ExpressionTreeFuncletizer
{
    public OpenEdgeParameterExtractingExpressionVisitor(
        IModel model,
        IEvaluatableExpressionFilter evaluatableExpressionFilter,
        Type contextType,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger,
        bool generateContextAccessors = false)
        : base(model, evaluatableExpressionFilter, contextType, generateContextAccessors, logger)
    {

    }

    protected Expression VisitNewMember(MemberExpression memberExpression)
    {
        if (memberExpression.Expression is ConstantExpression constant
            && constant.Value != null)
        {
            switch (memberExpression.Member.MemberType)
            {
                case MemberTypes.Field:
                    return Expression.Constant(constant.Value.GetType().GetField(memberExpression.Member.Name).GetValue(constant.Value));

                case MemberTypes.Property:
                    var propertyInfo = constant.Value.GetType().GetProperty(memberExpression.Member.Name);
                    if (propertyInfo == null)
                    {
                        break;
                    }

                    return Expression.Constant(propertyInfo.GetValue(constant.Value));
            }
        }

        return base.VisitMember(memberExpression);
    }

    protected override Expression VisitNew(NewExpression node)
    {
        var memberArguments = node.Arguments
            .Select(m => m is MemberExpression mem ? VisitNewMember(mem) : Visit(m))
            .ToList();

        var newNode = node.Update(memberArguments);

        return newNode;
    }

    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
        if (methodCallExpression.Method.Name.Equals("Take", StringComparison.OrdinalIgnoreCase)
         || methodCallExpression.Method.Name.Equals("Skip", StringComparison.OrdinalIgnoreCase))
        {
            return methodCallExpression;
        }

        return base.VisitMethodCall(methodCallExpression);
    }
}
#pragma warning restore EF1001 // Internal EF Core API usage.