﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ScwSvc.Models
{
    /// <summary>
    /// This class specifies which information is required for creating a new data set.
    /// </summary>
    public class CreateDataSetModel
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
    }
}
