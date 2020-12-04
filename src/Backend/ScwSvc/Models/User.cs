using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        /// <summary>
        /// A unique ID to identify the user.
        /// </summary>
        [Key]
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// A unique user name.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// The hashed password of the user.
        /// </summary>
        [Required]
        [MinLength(32)]
        [MaxLength(32)]
        [JsonIgnore]
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// Tables that this user owns.
        /// </summary>
        [JsonIgnore]
        public ICollection<TableRef> OwnTables { get; set; }

        /// <summary>
        /// Tables that this user may edit.
        /// </summary>
        [JsonIgnore]
        public ICollection<TableRef> Collaborations { get; set; }

        /// <summary>
        /// The role of this user.
        /// </summary>
        [Required]
        public UserRole Role { get; set; }
    }

    /// <summary>
    /// Lists different roles users can have.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserRole
    {
        /// <summary>
        /// A regular user with no special permissions.
        /// </summary>
        Common,

        /// <summary>
        /// A trusted user, basically an admin with readonly rights.
        /// Users of this kind may view any user's table.
        /// </summary>
        Manager,

        /// <summary>
        /// An administrator.
        /// Users of this kind may edit any user's tables.
        /// </summary>
        Admin
    }
}
