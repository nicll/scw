using System;
using System.ComponentModel.DataAnnotations;

namespace ScwSvc.Models
{
    public class CreateSheetModel
    {
        /// <summary>
        /// An optional display name.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string DisplayName { get; set; }
    }
}
