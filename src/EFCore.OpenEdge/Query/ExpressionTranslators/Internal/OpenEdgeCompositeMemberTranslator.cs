using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.OpenEdge.Query.ExpressionTranslators.Internal;

public class OpenEdgeCompositeMemberTranslator : RelationalMemberTranslatorProvider
{
    private static readonly List<Type> _translatorsMethods
        = OpenEdgeCompositeMethodCallTranslator.GetTranslatorMethods<IMemberTranslator>().ToList();

    public OpenEdgeCompositeMemberTranslator(RelationalMemberTranslatorProviderDependencies dependencies)
        : base(dependencies)
        => AddTranslators(_translatorsMethods.Select(type => (IMemberTranslator)Activator.CreateInstance(type)));
}