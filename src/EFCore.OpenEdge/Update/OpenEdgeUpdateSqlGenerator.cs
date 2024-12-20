using EntityFrameworkCore.OpenEdge.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFrameworkCore.OpenEdge.Update;

public class OpenEdgeUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies) : UpdateSqlGenerator(dependencies), IOpenEdgeUpdateSqlGenerator
{
    protected void AppendValues(StringBuilder commandStringBuilder, IReadOnlyList<IColumnModification> operations)
    {
        bool useLiterals = true;

        if (operations.Count > 0)
        {
            commandStringBuilder
                .Append('(')
                .AppendJoin(
                    operations,
                    SqlGenerationHelper,
                    (sb, o, helper) =>
                    {
                        if (useLiterals)
                        {
                            AppendSqlLiteral(sb, o.Value, o.Property);
                        }
                        else
                        {
                            // Use '?' rather than named parameters
                            AppendParameter(sb, o);
                        }
                    })
                .Append(')');
        }
    }

    private static void AppendParameter(StringBuilder commandStringBuilder, IColumnModification modification)
    {
        commandStringBuilder.Append(modification.IsWrite ? "?" : "DEFAULT");
    }

    private void AppendSqlLiteral(StringBuilder commandStringBuilder, object value, IProperty property)
    {
        var mapping = property != null
            ? Dependencies.TypeMappingSource.FindMapping(property)
            : null;
        mapping ??= Dependencies.TypeMappingSource.GetMappingForValue(value);
        commandStringBuilder.Append(mapping.GenerateProviderValueSqlLiteral(value));
    }


    protected override void AppendUpdateCommandHeader(StringBuilder commandStringBuilder, string name, string schema,
        IReadOnlyList<IColumnModification> operations)
    {
        commandStringBuilder.Append("UPDATE ");
        SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, name, schema);
        commandStringBuilder.Append(" SET ")
            .AppendJoin(
                operations,
                SqlGenerationHelper,
                (sb, o, helper) =>
                {
                    helper.DelimitIdentifier(sb, o.ColumnName);
                    sb.Append(" = ");
                    if (!o.UseCurrentValueParameter)
                    {
                        AppendSqlLiteral(sb, o.Value, o.Property);
                    }
                    else
                    {
                        sb.Append('?');
                    }
                });
    }

    protected override void AppendWhereCondition(StringBuilder commandStringBuilder, IColumnModification columnModification,
        bool useOriginalValue)
    {
        SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, columnModification.ColumnName);

        var parameterValue = useOriginalValue
            ? columnModification.OriginalValue
            : columnModification.Value;

        if (parameterValue == null)
        {
            base.AppendWhereCondition(commandStringBuilder, columnModification, useOriginalValue);
        }
        else
        {
            commandStringBuilder.Append(" = ");
            if (!columnModification.UseCurrentValueParameter
                && !columnModification.UseOriginalValueParameter)
            {
                base.AppendWhereCondition(commandStringBuilder, columnModification, useOriginalValue);
            }
            else
            {
                commandStringBuilder.Append('?');
            }
        }
    }

    public override ResultSetMapping AppendInsertOperation(StringBuilder commandStringBuilder, IReadOnlyModificationCommand command, int commandPosition)
        => AppendInsertOperation(commandStringBuilder, command, commandPosition, out _);

    public override ResultSetMapping AppendInsertOperation(StringBuilder commandStringBuilder, IReadOnlyModificationCommand command, int commandPosition, out bool requiresTransaction)
    {
        var name = command.TableName;
        var schema = command.Schema;
        var operations = command.ColumnModifications;

        // the override is because of the writeOperations here
        var writeOperations = operations.Where(o => o.IsWrite)
            .Where(o => !o.ColumnName.Equals("rowid", StringComparison.OrdinalIgnoreCase))
            .ToList();
        var readOperations = operations.Where(o => o.IsRead).ToList();
        AppendInsertCommand(commandStringBuilder, name, schema, writeOperations, readOperations);
        requiresTransaction = false;
        return readOperations.Count > 0 ? ResultSetMapping.LastInResultSet : ResultSetMapping.NoResults;
    }
}