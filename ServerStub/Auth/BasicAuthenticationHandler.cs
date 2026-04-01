using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace ServerStub.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            string? authorizationHeader = Request.Headers.Authorization;
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            if (!authorizationHeader.StartsWith("basic ", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            var token = authorizationHeader.Substring(6);
            var credentialAsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            var credentials = credentialAsString.Split(":");
            if (credentials?.Length != 2)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            var username = credentials[0];
            var password = credentials[1];

            if (username != "user" && password != "user")
            {
                return AuthenticateResult.Fail("Authentication failed");
            }

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, username)
        };
            var identity = new ClaimsIdentity(claims, "Basic");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
        }
    }
}
