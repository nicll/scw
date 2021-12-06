using System.Text.Json.Serialization;

namespace ScwSvc.Models;

/// <summary>
/// Lists the different kinds of tables.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TableType
{
    /// <summary>
    /// A table that is directly mapped to the database.
    /// </summary>
    DataSet,

    /// <summary>
    /// A spreadsheet table.
    /// </summary>
    Sheet
}
