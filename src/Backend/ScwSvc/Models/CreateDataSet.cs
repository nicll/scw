using System;
using System.ComponentModel.DataAnnotations;

namespace ScwSvc.Models
{
    /// <summary>
    /// This class specifies which information is required for creating a new data set.
    /// </summary>
    public class CreateDataSet
    {
        /// <summary>
        /// An optional display name.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string DisplayName { get; set; }

        /// <summary>
        /// The columns that this table consists of.
        /// </summary>
        [Required]
        public ColumnDefinition[] Columns { get; set; }
    }
}
