namespace service_api_csharp.Domain.Entities;

using System;

public class RefreshToken
{
    public long Id { get; set; }
    
    public string Token { get; set; } = string.Empty;
    
    public int UserId { get; set; }
    
    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsRevoked { get; set; }
    
    public User? User { get; set; } 
}