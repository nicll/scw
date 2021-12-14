using System.ComponentModel.DataAnnotations;
using ScwSvc.Models;

namespace ScwSvc.SvcModels;

/// <summary>
/// This class specifies the necessary information of each column.
/// </summary>
public class ColumnDefinition
{
    /// <summary>
    /// The data type of the values in the column.
    /// </summary>
    [Required]
    public ColumnType Type { get; set; }

    /// <summary>
    /// The name of the column.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Whether or not this column can contain nullable values.
    /// </summary>
    [Required]
    public bool Nullable { get; set; }
}
