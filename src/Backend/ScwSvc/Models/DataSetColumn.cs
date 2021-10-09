using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace ScwSvc.Models
{
    /// <summary>
    /// Contains all information about a column of a data set.
    /// </summary>
    public class DataSetColumn
    {
        /// <summary>
        /// The ID of the table reference.
        /// </summary>
        [Required]
        [JsonIgnore]
        public Guid TableRefId { get; set; }

        /// <summary>
        /// A reference to the table reference.
        /// </summary>
        [Required]
        [JsonIgnore]
        public virtual TableRef TableRef { get; set; }

        /// <summary>
        /// The position of the column from left to right.
        /// </summary>
        [Required]
        public byte Position { get; set; }

        /// <summary>
        /// The name of the column.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// The data type of the values in this column.
        /// </summary>
        [Required]
        public ColumnType Type { get; set; }

        /// <summary>
        /// Whether or not this column can contain nullable values.
        /// </summary>
        [Required]
        public bool Nullable { get; set; }
    }
}
