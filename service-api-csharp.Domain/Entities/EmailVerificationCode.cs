using System;

namespace service_api_csharp.Domain.Entities;

public class EmailVerificationCode
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string Code { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public bool IsUsed { get; set; }
    
    public int Attempts { get; set; }

    public User? User { get; set; }
}