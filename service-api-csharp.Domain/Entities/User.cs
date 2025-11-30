using System;
using System.Collections.Generic;

namespace service_api_csharp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Identification { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? GoogleId { get; set; }
    public string AuthProvider { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string? VerificationToken { get; set; }
    public DateTime? TokenExpiryDate { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetTokenExpiresAt { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? LastLogin { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}