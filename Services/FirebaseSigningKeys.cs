using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace ZHamaster.Api.Services;

// Firebase publishes rotating signing certificates. ConfigurationManager caches
// them and refreshes on an unknown kid; no service-account private key is needed.
public sealed class FirebaseSigningKeys(string issuer) : IConfigurationRetriever<OpenIdConnectConfiguration>
{
    public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(
        string address, IDocumentRetriever retriever, CancellationToken cancel)
    {
        var document = await retriever.GetDocumentAsync(address, cancel);
        var certificates = JsonSerializer.Deserialize<Dictionary<string, string>>(document)
            ?? throw new InvalidOperationException("Firebase signing certificates unavailable.");
        var configuration = new OpenIdConnectConfiguration { Issuer = issuer };
        foreach (var (id, pem) in certificates)
            configuration.SigningKeys.Add(new X509SecurityKey(X509Certificate2.CreateFromPem(pem)) { KeyId = id });
        return configuration;
    }
}
