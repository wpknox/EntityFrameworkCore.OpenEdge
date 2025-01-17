using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.OpenEdge.Storage.Internal.Mapping;

public class OpenEdgeTypeMappingSource : RelationalTypeMappingSource
{
    private const int VARCHAR_MAX_SIZE = 32000;

    private readonly DateTimeTypeMapping _datetime = new("datetime", DbType.DateTime);
    private readonly DateTimeOffsetTypeMapping _datetimeOffset = new("datetime-tz", DbType.DateTimeOffset);
    private readonly DateTimeTypeMapping _date = new("date", DbType.Date);
    private readonly DateTimeTypeMapping _timeStamp = new("timestamp", DbType.DateTime);
    private readonly TimeSpanTypeMapping _time = new("time", DbType.Time);

    private readonly OpenEdgeBoolTypeMapping _boolean = new();
    private readonly ShortTypeMapping _smallInt = new("smallint", DbType.Int16);
    private readonly ShortTypeMapping _tinyInt = new("tinyint", DbType.Byte);
    private readonly IntTypeMapping _integer = new("integer", DbType.Int32);
    private readonly LongTypeMapping _bigInt = new("bigint");

    private readonly StringTypeMapping _char = new("char", DbType.String);
    private readonly StringTypeMapping _varchar = new("varchar", DbType.AnsiString);

    private readonly ByteArrayTypeMapping _binary = new("binary", DbType.Binary);

    private readonly FloatTypeMapping _float = new("real");
    private readonly DoubleTypeMapping _double = new("double precision");
    private readonly DecimalTypeMapping _decimal = new("decimal");

    private readonly Dictionary<string, RelationalTypeMapping> _storeTypeMappings;
    private readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;

    public OpenEdgeTypeMappingSource(TypeMappingSourceDependencies dependencies, RelationalTypeMappingSourceDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
        _clrTypeMappings
            = new Dictionary<Type, RelationalTypeMapping>
            {
                { typeof(int), _integer },
                { typeof(long), _bigInt },
                { typeof(DateTime), _datetime },
                { typeof(bool), _boolean },
                { typeof(byte), _tinyInt },
                { typeof(byte[]), _binary},
                { typeof(double), _double },
                { typeof(DateTimeOffset), _datetime },
                { typeof(short), _smallInt },
                { typeof(float), _float },
                { typeof(decimal), _decimal },
                { typeof(TimeSpan), _time }
            };

        _storeTypeMappings
            = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
            {
                { "bigint", _bigInt },
                { "int64", _bigInt },
                { "binary varying", _binary },
                { "raw", _binary },
                { "binary", _binary },
                { "blob", _binary },
                { "bit", _boolean},
                { "logical", _boolean},
                { "char varying", _char },
                { "char", _char },
                { "character varying", _char },
                { "character", _char },
                { "clob", _char },
                { "date", _date },
                { "datetime", _datetime },
                { "datetime2", _datetime },
                { "datetimeoffset", _datetimeOffset },
                { "datetime-tz", _datetimeOffset },
                { "dec", _decimal },
                { "decimal", _decimal },
                { "double precision", _double },
                { "double", _double },
                { "float", _double },
                { "image", _binary },
                { "int", _integer },
                { "integer", _integer },
                { "money", _decimal },
                { "numeric", _decimal },
                { "real", _float },
                { "recid", _char },
                { "smalldatetime", _datetime },
                { "smallint", _smallInt},
                { "short", _smallInt},
                { "smallmoney", _decimal },
                { "text", _char},
                { "time", _time },
                { "timestamp", _timeStamp },
                { "tinyint", _tinyInt},
                { "varbinary", _binary },
                { "varchar", _varchar }
            };
    }

    protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        return FindRawMapping(mappingInfo)?.Clone(mappingInfo);
    }

    private RelationalTypeMapping FindRawMapping(RelationalTypeMappingInfo mappingInfo)
    {
        var clrType = mappingInfo.ClrType;
        var storeTypeName = mappingInfo.StoreTypeName;
        var storeTypeNameBase = mappingInfo.StoreTypeNameBase;

        if (storeTypeName != null)
        {
            if (clrType == typeof(float)
                && mappingInfo.Size is <= 24
                && storeTypeNameBase != null
                && (storeTypeNameBase.Equals("float", StringComparison.OrdinalIgnoreCase)
                    || storeTypeNameBase.Equals("double precision", StringComparison.OrdinalIgnoreCase)))
            {
                return _float;
            }

            if (_storeTypeMappings.TryGetValue(storeTypeName, out var mapping)
             || _storeTypeMappings.TryGetValue(storeTypeNameBase ?? "", out mapping))
            {
                return clrType == null || mapping.ClrType == clrType
                    ? mapping
                    : null;
            }
        }

        if (clrType == null) return null;
        if (_clrTypeMappings.TryGetValue(clrType, out var clrMapping))
        {
            return clrMapping;
        }

        if (clrType != typeof(string)) return null;
        var isAnsi = mappingInfo.IsUnicode == false;
        var isFixedLength = mappingInfo.IsFixedLength == true;
        var baseName = isFixedLength ? "CHAR" : "VARCHAR";
        var size = (int?)(mappingInfo.Size ?? VARCHAR_MAX_SIZE);
        if (size > VARCHAR_MAX_SIZE)
        {
            size = null;
        }

        var dbType = isAnsi
            ? (isFixedLength ? DbType.AnsiStringFixedLength : DbType.AnsiString)
            : (isFixedLength ? DbType.StringFixedLength : (DbType?)null);

        var storeType = $"{baseName}({(size == null ? "max" : size.ToString())})";
        return new StringTypeMapping(
            storeType,
            dbType,
            !isAnsi,
            size);

    }
}