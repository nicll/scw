using System;
using System.ComponentModel.DataAnnotations;

namespace ScwSvc.Models
{
    public class CreateSheet
    {
        /// <summary>
        /// An optional display name.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string DisplayName { get; set; }
    }
}
