using System.ComponentModel.DataAnnotations;

namespace ZHamaster.Api.DTOs;

public class LoginRequest
{
    [Required]
    public string IdToken { get; set; } = "";
}
