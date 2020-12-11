using Microsoft.EntityFrameworkCore;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScwSvc.Interactors
{
    public static class DynDbInteractor // ToDo: this will be refactored to something nice later on...
    {
        public static async ValueTask CreateDataSet(TableRef table, DbDynContext db)
        {
            if (table.Columns.Count < 1)
                throw new InvalidTableException("No columns were specified.");

            if (table.Type != TableType.DataSet)
                throw new InvalidTableException("Not the correct table type.");

            using var conn = db.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = ConvertTableRefToCreateTable(table);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async ValueTask RemoveDataSet(TableRef table, DbDynContext db)
        {
            if (table.Type != TableType.DataSet)
                throw new InvalidTableException("Not the correct table type.");

            using var conn = db.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DROP TABLE " + table.LookupName.ToNameString();
            await cmd.ExecuteNonQueryAsync();
        }

        public static async ValueTask CreateSheet(TableRef table, DbDynContext db)
        {
            if (table.Type != TableType.Sheet)
                throw new InvalidTableException("Not the correct table type.");

            using var conn = db.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "CREATE TABLE (A varchar(200), B varchar(200), B varchar(200), D varchar(200), E varchar(200), F varchar(200), G varchar(200), H varchar(200))";
            await cmd.ExecuteNonQueryAsync();
        }

        public static async ValueTask RemoveSheet(TableRef table, DbDynContext db)
        {
            if (table.Type != TableType.Sheet)
                throw new InvalidTableException("Not the correct table type.");

            using var conn = db.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DROP TABLE " + table.LookupName.ToNameString();
            await cmd.ExecuteNonQueryAsync();
        }

        private static string ConvertTableRefToCreateTable(TableRef table)
        {
            var create = new StringBuilder();
            create.Append("CREATE TABLE \"")
                .Append(table.LookupName.ToNameString())
                .Append("\" (");

            foreach (var column in table.Columns)
            {
                if (column.Name.Length > 20)
                    throw new InvalidTableException("Name for column is too long: " + column.Name);

                if (column.Name.Any(c => !Char.IsLetterOrDigit(c)))
                    throw new InvalidTableException("Invalid character(s) in column: " + column.Name);

                create.Append("\"").Append(column.Name).Append("\"")
                    .Append(" ")
                    .Append(column.Type switch
                    {
                        ColumnType.Integer => "bigint",
                        ColumnType.Real => "double precision",
                        ColumnType.Timestamp => "timestamp",
                        ColumnType.String => "varchar(200)",
                        _ => throw new InvalidTableException("Invalid column type: " + column.Type)
                    })
                    .Append(column.Nullable ? " null" : " not null")
                    .Append(",");
            }

            create.Length -= ",".Length;
            create.Append(")");
            return create.ToString();
        }
    }
}
