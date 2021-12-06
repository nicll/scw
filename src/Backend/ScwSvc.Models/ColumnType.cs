using System.Text.Json.Serialization;

namespace ScwSvc.Models;

/// <summary>
/// The different kinds of data types that a column may contain.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ColumnType
{
    /// <summary>
    /// A signed 64 bit integer value.
    /// </summary>
    Integer,

    /// <summary>
    /// A double precision IEEE floating point value.
    /// </summary>
    Real,

    /// <summary>
    /// A timestamp containing both time and date without a timezone.
    /// Can at least contain dates from 0 to 9999 AD.
    /// </summary>
    Timestamp,

    /// <summary>
    /// A character array mit a max length of 200 chars.
    /// </summary>
    String
}
