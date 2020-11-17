using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScwSvc.Models
{
    /// <summary>
    /// Describes a single user.
    /// </summary>
    public class User
    {
        [Key]
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [MinLength(32)]
        [MaxLength(32)]
        public byte[] PasswordHash { get; set; }

        public ICollection<TableRef> OwnTables { get; set; }

        public ICollection<TableRef> Collaborations { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        Common,
        Manager,
        Admin
    }
}
