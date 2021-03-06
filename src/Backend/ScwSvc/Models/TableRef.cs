﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScwSvc.Models
{
    /// <summary>
    /// Contains information about a user-specified table including its name,
    /// columns and collaborators (users allowed to edit the table).
    /// </summary>
    public class TableRef
    {
        /// <summary>
        /// A unique ID to identify the table.
        /// </summary>
        [Key]
        [Required]
        public Guid TableRefId { get; set; }

        /// <summary>
        /// An optional display name.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Whether this table is a data set or a spreadsheet.
        /// </summary>
        [Required]
        public TableType TableType { get; set; }

        /// <summary>
        /// The table's name in the database.
        /// </summary>
        /// <remarks>
        /// The name is generated by using <see cref="Guid.ToString(string?)"/> with the argument "N".
        /// </remarks>
        [Required]
        [JsonIgnore]
        public Guid LookupName { get; set; }

        /// <summary>
        /// ID of the user that created this table.
        /// </summary>
        [Required]
        public Guid OwnerUserId { get; set; }

        /// <summary>
        /// The user that created this table.
        /// </summary>
        /// <remarks>
        /// This user may add/remove collaborators and change the table's <see cref="DisplayName"/>.
        /// </remarks>
        [Required]
        [JsonIgnore]
        public virtual User Owner { get; set; }

        /// <summary>
        /// Users that are allowed to edit this table's content.
        /// </summary>
        [Required]
        [JsonIgnore]
        public virtual ICollection<User> Collaborators { get; set; }

        /// <summary>
        /// The columns that this table consists of.
        /// This is ignored if <see cref="TableType"/> is <see cref="TableType.Sheet"/>.
        /// </summary>
        [Required]
        public virtual ICollection<DataSetColumn> Columns { get; set; }
    }

    /// <summary>
    /// Lists the different kinds of tables.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
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
}
