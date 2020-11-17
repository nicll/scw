using System;
using System.ComponentModel.DataAnnotations;

namespace ScwSvc.Models
{
    public class DataSetColumn
    {
        [Required]
        public Guid TableRefId { get; set; }

        [Required]
        public TableRef TableRef { get; set; }

        [Required]
        public byte Position { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        public ColumnType Type { get; set; }

        [Required]
        public bool Nullable { get; set; }
    }

    public enum ColumnType
    {
        Integer,
        Float,
        Date,
        String
    }
}
