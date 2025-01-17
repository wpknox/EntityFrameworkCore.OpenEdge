using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.OpenEdge.Query.ExpressionTranslators.Internal;

public class OpenEdgeCompositeMemberTranslator : RelationalMemberTranslatorProvider
{
    private static readonly List<Type> s_translatorsMethods
        = OpenEdgeCompositeMethodCallTranslator.GetTranslatorMethods<IMemberTranslator>().ToList();

    public OpenEdgeCompositeMemberTranslator(RelationalMemberTranslatorProviderDependencies dependencies)
        : base(dependencies)
        => AddTranslators(s_translatorsMethods.Select(type => (IMemberTranslator)Activator.CreateInstance(type)));
}