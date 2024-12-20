using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.OpenEdge.Query.ExpressionTranslators.Internal;

public class OpenEdgeCompositeMethodCallTranslator : RelationalMethodCallTranslatorProvider
{
    private static readonly List<Type> _translatorsMethods
        = GetTranslatorMethods<IMethodCallTranslator>().ToList();

    public OpenEdgeCompositeMethodCallTranslator(RelationalMethodCallTranslatorProviderDependencies dependencies)
        : base(dependencies)
        => AddTranslators(_translatorsMethods.Select(type => (IMethodCallTranslator)Activator.CreateInstance(type)));

    public static IEnumerable<Type> GetTranslatorMethods<TInterface>()
        => Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i == typeof(TInterface))
                && t.GetConstructors().Any(c => c.GetParameters().Length == 0));
}