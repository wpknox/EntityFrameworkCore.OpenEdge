using System.Data.Common;
using System.Data.Odbc;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.OpenEdge.Storage;

public class OpenEdgeRelationalConnection(RelationalConnectionDependencies dependencies) : RelationalConnection(dependencies), IOpenEdgeRelationalConnection
{
    protected override DbConnection CreateDbConnection()
        => new OdbcConnection(ConnectionString);
}