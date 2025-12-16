using System;

namespace service_api_csharp.Domain.Entities;

public class EmailVerificationCode
{
    public int Id { get; set; }


    public string Code { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsUsed { get; set; }
    
    public int Attempts { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}