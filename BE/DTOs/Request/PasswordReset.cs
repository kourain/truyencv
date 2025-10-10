using System.ComponentModel.DataAnnotations;

namespace TruyenCV.DTO.Request;

public class RequestPasswordResetRequest
{
    [Required]
    [EmailAddress]
    public required string email { get; set; }
}

public class ConfirmPasswordResetRequest
{
    [Required]
    [EmailAddress]
    public required string email { get; set; }

    [Required]
    [StringLength(6, MinimumLength = 4)]
    public required string otp { get; set; }

    [Required]
    [MinLength(8)]
    public required string new_password { get; set; }
}
