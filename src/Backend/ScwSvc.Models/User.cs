using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ScwSvc.Models;

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
    /// Date when this user was created.
    /// </summary>
    [Required]
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Tables that this user owns.
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<Table> OwnTables { get; set; }

    /// <summary>
    /// Tables that this user may edit.
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<Table> Collaborations { get; set; }

    /// <summary>
    /// The role of this user.
    /// </summary>
    [Required]
    public UserRole Role { get; set; }
}
