using System.Text;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace test_backend_project.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Authentication failed");
            }

            var headerValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var headerBytes = Convert.FromBase64String(headerValue.Parameter ?? "");
            var credentials = Encoding.UTF8.GetString(headerBytes);
            if (!string.IsNullOrEmpty(credentials))
            {
                string[] credentialArray = credentials.Split(":");
                string userName = credentialArray[0];
                string password = credentialArray[1];

                if (userName == "admin" && password == "admin")
                {
                    var claims = new[] {
                        new Claim(ClaimTypes.Name, userName),
                        new Claim(ClaimTypes.NameIdentifier, userName),
                     };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var authTicket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(authTicket);
                }
                else
                {
                    return AuthenticateResult.Fail("invalid credential");
                }
            }
            return AuthenticateResult.Fail("UnAuthorized");
        }
    }
}