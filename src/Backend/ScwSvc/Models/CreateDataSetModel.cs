﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ScwSvc.Models
{
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

        public class ColumnDefinition
        {
            public ColumnType Type { get; set; }

            public string Name { get; set; }

            public bool Nullable { get; set; }
        }
    }
}
