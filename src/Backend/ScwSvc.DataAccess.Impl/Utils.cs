using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.DataAccess.Impl;

internal static class Utils
{
    private static readonly Dictionary<ColumnType, (string typeName, string defaultValue)> _typeMap = new()
    {
        { ColumnType.Integer, ("bigint", "0") },
        { ColumnType.Real, ("double precision", "0.0") },
        { ColumnType.Timestamp, ("timestamp", "'1970-01-01'") },
        { ColumnType.String, ("varchar(200)", "''") }
    };

    internal static void EnsureValidColumnName(string columnName)
    {
        if (columnName.Length > 20)
            throw new TableColumnException("Name for column is too long: " + columnName);

        if (columnName.Any(c => !Char.IsLetterOrDigit(c)))
            throw new TableColumnException("Invalid character(s) in column: " + columnName);

        if (String.Equals(columnName, "_id", StringComparison.OrdinalIgnoreCase))
            throw new TableColumnException("Invalid column name: " + columnName);
    }

    internal static string ConvertToSqlColumn(DataSetColumn column)
    {
        EnsureValidColumnName(column.Name);

        if (!_typeMap.TryGetValue(column.Type, out var col))
            throw new InvalidTableException("Invalid column type: " + column.Type);

        return new StringBuilder().Append('"').Append(column.Name).Append('"')
            .Append(' ')
            .Append(col.typeName)
            .Append(column.Nullable ? " null" : " not null default " + col.defaultValue)
            .ToString();
    }

    /// <summary>
    /// Turns a <see cref="Guid"/> into a <see cref="String"/> that can be used as a name.
    /// </summary>
    /// <param name="guid">The given <see cref="Guid"/>.</param>
    /// <returns>The ID as a normalized <see cref="String"/>.</returns>
    internal static string ToDbName(this Guid guid)
        => guid.ToString("N");
}
