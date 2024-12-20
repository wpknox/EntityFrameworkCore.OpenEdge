using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EntityFrameworkCore.OpenEdge.Query.Sql.Internal;

#pragma warning disable EF1001 // Internal EF Core API usage.
public class OpenEdgeSqlGeneratorFactory(
    QuerySqlGeneratorDependencies dependencies) : QuerySqlGeneratorFactory(dependencies)
{
    public override QuerySqlGenerator Create()
        => new OpenEdgeSqlGenerator(Dependencies);
}
#pragma warning restore EF1001 // Internal EF Core API usage.