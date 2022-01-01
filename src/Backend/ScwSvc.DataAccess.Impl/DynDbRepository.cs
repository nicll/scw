using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScwSvc.DataAccess.Interfaces;
using ScwSvc.Exceptions;
using ScwSvc.Models;
using static ScwSvc.DataAccess.Impl.Utils;

namespace ScwSvc.DataAccess.Impl;

public class DynDbRepository : IDynDbRepository
{
    private readonly DbDynContext _dynDb;

    public DynDbRepository(DbDynContext dynDb)
        => _dynDb = dynDb;

    public async Task CreateTable(TableRef table)
    {
        switch (table.TableType)
        {
            case TableType.DataSet:
                await CreateDataSet(table);
                break;

            case TableType.Sheet:
                await CreateSheet(table);
                break;

            default:
                throw new InvalidTableException("TableType not implemented in DA layer: " + table.TableType);
        }
    }

    private async ValueTask CreateDataSet(TableRef table)
    {
        using var conn = _dynDb.Database.GetDbConnection();
        await conn.OpenAsync().ConfigureAwait(false);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = ConvertTableRefToCreateTable(table);
        await cmd.ExecuteNonQueryAsync();
    }

    private async ValueTask CreateSheet(TableRef table)
    {
        using var conn = _dynDb.Database.GetDbConnection();
        await conn.OpenAsync().ConfigureAwait(false);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "CREATE TABLE \"" + table.LookupName.ToDbName() + "\" (A varchar(200), B varchar(200), C varchar(200), D varchar(200), E varchar(200), F varchar(200), G varchar(200), H varchar(200))";
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task RemoveTable(TableRef table)
    {
        using var conn = _dynDb.Database.GetDbConnection();
        await conn.OpenAsync().ConfigureAwait(false);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DROP TABLE \"" + table.LookupName.ToDbName() + '\"';
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task AddDataSetColumn(TableRef table, DataSetColumn column)
    {
        using var conn = _dynDb.Database.GetDbConnection();
        await conn.OpenAsync().ConfigureAwait(false);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "ALTER TABLE \"" + table.LookupName.ToDbName() + "\" ADD COLUMN " + ConvertToSqlColumn(column);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task RemoveDataSetColumn(TableRef table, string columnName)
    {
        if (columnName == "_id")
            throw new TableColumnException("Cannot delete ID column.");

        using var conn = _dynDb.Database.GetDbConnection();
        await conn.OpenAsync().ConfigureAwait(false);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "ALTER TABLE \"" + table.LookupName.ToDbName() + "\" DROP COLUMN \"" + columnName + "\" CASCADE";
        await cmd.ExecuteNonQueryAsync();
    }

    private static string ConvertTableRefToCreateTable(TableRef table)
    {
        var create = new StringBuilder();
        create.Append("CREATE TABLE \"")
            .Append(table.LookupName.ToDbName())
            .Append("\" (\"_id\" serial not null primary key,"); // first column (_id) is primary key

        foreach (var column in table.Columns)
            create.Append(ConvertToSqlColumn(column)).Append(',');

        create.Length -= ",".Length;
        create.Append(')');
        return create.ToString();
    }
}
