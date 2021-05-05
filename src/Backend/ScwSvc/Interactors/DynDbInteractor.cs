using Microsoft.EntityFrameworkCore;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScwSvc.Interactors
{
    public static class DynDbInteractor
    {
        /// <summary>
        /// Creates a new data set from the DYN database.
        /// </summary>
        /// <param name="db">The DYN database context.</param>
        /// <param name="table">The <see cref="TableRef"/> object.</param>
        public static async ValueTask CreateDataSet(this DbDynContext db, TableRef table)
        {
            if (table.Columns.Count < 1)
                throw new InvalidTableException("No columns were specified.");

            if (table.Columns.Count > 255)
                throw new InvalidTableException("Too many columns were specified.");

            if (table.TableType != TableType.DataSet)
                throw new InvalidTableException("Not the correct table type.");

            using var conn = db.Database.GetDbConnection();
            await conn.OpenAsync().ConfigureAwait(false);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = ConvertTableRefToCreateTable(table);
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Removes a data set from the DYN database.
        /// </summary>
        /// <param name="db">The DYN database context.</param>
        /// <param name="table">The <see cref="TableRef"/> object.</param>
        public static async ValueTask RemoveDataSet(this DbDynContext db, TableRef table)
        {
            if (table.TableType != TableType.DataSet)
                throw new InvalidTableException("Not the correct table type.");

            await RemoveTable(db, table);
        }

        /// <summary>
        /// Creates a new sheet from the DYN database.
        /// </summary>
        /// <param name="db">The DYN database context.</param>
        /// <param name="table">The <see cref="TableRef"/> object.</param>
        public static async ValueTask CreateSheet(this DbDynContext db, TableRef table)
        {
            if (table.TableType != TableType.Sheet)
                throw new InvalidTableException("Not the correct table type.");

            using var conn = db.Database.GetDbConnection();
            await conn.OpenAsync().ConfigureAwait(false);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "CREATE TABLE \"" + table.LookupName.ToNameString() + "\" (A varchar(200), B varchar(200), C varchar(200), D varchar(200), E varchar(200), F varchar(200), G varchar(200), H varchar(200))";
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Removes a sheet from the DYN database.
        /// </summary>
        /// <param name="db">The DYN database context.</param>
        /// <param name="table">The <see cref="TableRef"/> object.</param>
        public static async ValueTask RemoveSheet(this DbDynContext db, TableRef table)
        {
            if (table.TableType != TableType.Sheet)
                throw new InvalidTableException("Not the correct table type.");

            await RemoveTable(db, table);
        }

        /// <summary>
        /// Removes a table from the DYN database.
        /// This method does not check the table's <see cref="TableRef.TableType"/>.
        /// </summary>
        /// <param name="db">The DYN database context.</param>
        /// <param name="table">The <see cref="TableRef"/> object.</param>
        public static async ValueTask RemoveTable(this DbDynContext db, TableRef table)
        {
            using var conn = db.Database.GetDbConnection();
            await conn.OpenAsync().ConfigureAwait(false);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DROP TABLE \"" + table.LookupName.ToNameString() + '\"';
            await cmd.ExecuteNonQueryAsync();
        }

        private static string ConvertTableRefToCreateTable(TableRef table)
        {
            var create = new StringBuilder();
            create.Append("CREATE TABLE \"")
                .Append(table.LookupName.ToNameString())
                .Append("\" (")
                .Append("\"id\" serial not null primary key,"); // first column (id) is primary key

            foreach (var column in table.Columns)
            {
                if (column.Name.Length > 20)
                    throw new InvalidTableException("Name for column is too long: " + column.Name);

                if (column.Name.Any(c => !Char.IsLetterOrDigit(c)))
                    throw new InvalidTableException("Invalid character(s) in column: " + column.Name);

                if (String.Compare(column.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
                    throw new InvalidTableException("Invalid column name: " + column.Name);

                create.Append('"').Append(column.Name).Append('"')
                    .Append(' ')
                    .Append(column.Type switch
                    {
                        ColumnType.Integer => "bigint",
                        ColumnType.Real => "double precision",
                        ColumnType.Timestamp => "timestamp",
                        ColumnType.String => "varchar(200)",
                        _ => throw new InvalidTableException("Invalid column type: " + column.Type)
                    })
                    .Append(column.Nullable ? " null" : " not null")
                    .Append(',');
            }

            create.Length -= ",".Length;
            create.Append(')');
            return create.ToString();
        }
    }
}
