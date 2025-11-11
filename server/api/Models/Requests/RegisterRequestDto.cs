using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Requests;

public class RegisterRequestDto
{
    [EmailAddress] public string Email { get; set; } = null!;
    [MinLength(8)] public string Password { get; set; } = null!;
}