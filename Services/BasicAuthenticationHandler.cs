using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        if (!Request.Headers.ContainsKey("authorization"))
            return Task.FromResult(AuthenticateResult.Fail("No Authorization header"));
        if (!Request.Headers["authorization"][0].StartsWith("Basic "))
            return Task.FromResult(AuthenticateResult.Fail("Invalid Schema"));
        var encoded = Request.Headers["authorization"][0].Substring("Basic ".Length);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        var credentials = decoded.Split(':');
        var username = credentials[0];
        var password = credentials[1];
        if (username != "admin" || password != "12341234")
            return Task.FromResult(AuthenticateResult.Fail("invalid credintials"));

        var identity = new ClaimsIdentity(new Claim[] {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, "10")
        }, "Basic");

        var principle = new ClaimsPrincipal(identity);
        var authenticationTicket = new AuthenticationTicket(principle, "Basic");
        return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
    }
}