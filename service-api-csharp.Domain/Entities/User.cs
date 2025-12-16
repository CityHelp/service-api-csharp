using System;
using System.Collections.Generic;

namespace service_api_csharp.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public Guid uuid { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string OAuthProvider { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginAttemps { get; set; }
    public DateTime LockedUntil { get; set; }
    public DateTime LastFailedLoginAttempt { get; set; }
    
    // Navigation properties
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    
    public ICollection<EmailVerificationCode> EmailVerificationCodes { get; set; } = new List<EmailVerificationCode>();
}