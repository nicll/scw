using System.ComponentModel.DataAnnotations;

namespace ScwSvc.SvcModels;

public class AuthenticationCredentials
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
