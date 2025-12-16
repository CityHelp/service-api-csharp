    using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using service_api_csharp.Application.Common.RepositoriesInterfaces.Others;

namespace service_api_csharp.API.Authentication;

public class CustomJwtHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ITokenValidator _tokenValidator;

    public CustomJwtHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ITokenValidator tokenValidator)
        : base(options, logger, encoder, clock)
    {
        _tokenValidator = tokenValidator;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValues))
            return AuthenticateResult.NoResult();

        var authorizationHeader = authorizationHeaderValues.FirstOrDefault();
        
        // Log received authorization header for debugging purposes
        Logger.LogWarning("Authorization Header Received: '{Header}'", authorizationHeader);

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            Logger.LogWarning("Authorization header is invalid or missing Bearer prefix");
            return AuthenticateResult.NoResult();
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        
        // Clean up token in case user pasted "token", ... or whole JSON
        if (token.Contains(","))
        {
            token = token.Split(',')[0];
        }
        token = token.Trim().Trim('"');

        // Log extracted and sanitized token for debugging purposes
        Logger.LogWarning("Extracted Token (Sanitized): '{Token}'", token);


        if (string.IsNullOrEmpty(token))
            return AuthenticateResult.NoResult();

        var result = await _tokenValidator.ValidateTokenAsync(token);

        if (!result.IsValid || result.Principal == null)
            return AuthenticateResult.Fail("Invalid token");

        return AuthenticateResult.Success(
            new AuthenticationTicket(result.Principal, Scheme.Name)
        );
    }
}
