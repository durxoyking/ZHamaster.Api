using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace ZHamaster.Api.Services;

public class FirebaseService
{

    public FirebaseService(IConfiguration configuration)
    {

        if (FirebaseApp.DefaultInstance == null)
        {

            var credentialJson = configuration["Firebase:ServiceAccountJson"];
            var credential = string.IsNullOrWhiteSpace(credentialJson)
                ? CredentialFactory.FromFile<ServiceAccountCredential>(
                    configuration["GOOGLE_APPLICATION_CREDENTIALS"] ?? "firebase-admin.json").ToGoogleCredential()
                : CredentialFactory.FromJson<ServiceAccountCredential>(credentialJson).ToGoogleCredential();

            FirebaseApp.Create(
                new AppOptions
                {
                    Credential = credential
                }
            );

        }

    }


    public Task<FirebaseToken> VerifyIdTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        return FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(
            token, true, cancellationToken);
    }

    public async Task<FirebaseToken> VerifyToken(string token)
    {

        return await FirebaseAuth
            .DefaultInstance
            .VerifyIdTokenAsync(token);

    }

}
