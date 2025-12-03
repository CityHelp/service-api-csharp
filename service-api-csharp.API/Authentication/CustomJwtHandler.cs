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
            string? token = Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

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
