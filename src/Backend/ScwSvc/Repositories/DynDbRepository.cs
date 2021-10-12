using Microsoft.EntityFrameworkCore;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScwSvc.Utils;

namespace ScwSvc.Repositories
{
    public static class DynDbRepository
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

            if (table.Columns.Count > Byte.MaxValue)
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
        /// Adds a column to an existing table in the DYN database.
        /// </summary>
        /// <param name="db">The DYN database context.</param>
        /// <param name="table">The <see cref="TableRef"/> object.</param>
        /// <param name="column">The column to add.</param>
        public static async ValueTask AddColumnToDataSet(this DbDynContext db, TableRef table, DataSetColumn column)
        {
            if (table.TableType != TableType.DataSet)
                throw new InvalidTableException("Not the correct table type.");

            if (table.Columns.Count > Byte.MaxValue)
                throw new InvalidTableException("Too many columns were specified.");

            using var conn = db.Database.GetDbConnection();
            await conn.OpenAsync().ConfigureAwait(false);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "ALTER TABLE \"" + table.LookupName.ToNameString() + "\" ADD COLUMN " + ConvertToSqlColumn(column);
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Removes a column from an existing table in the DYN database.
        /// </summary>
        /// <param name="db">The DYN database context.</param>
        /// <param name="table">The <see cref="TableRef"/> object.</param>
        /// <param name="columnName">The column to remove.</param>
        /// <returns></returns>
        public static async ValueTask RemoveColumnFromDataSet(this DbDynContext db, TableRef table, string columnName)
        {
            if (table.TableType != TableType.DataSet)
                throw new InvalidTableException("Not the correct table type.");

            if (table.Columns.SingleOrDefault(c => c.Name == columnName) is not (var column and not null))
                throw new InvalidTableException("Column does not exist.");

            using var conn = db.Database.GetDbConnection();
            await conn.OpenAsync().ConfigureAwait(false);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "ALTER TABLE \"" + table.LookupName.ToNameString() + "\" DROP COLUMN \"" + column.Name + "\"";
            await cmd.ExecuteNonQueryAsync();
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
                .Append("\" (\"_id\" serial not null primary key,"); // first column (_id) is primary key

            foreach (var column in table.Columns)
                create.Append(ConvertToSqlColumn(column)).Append(',');

            create.Length -= ",".Length;
            create.Append(')');
            return create.ToString();
        }
    }
}
