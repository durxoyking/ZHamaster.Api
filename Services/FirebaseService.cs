using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace ZHamaster.Api.Services;

public class FirebaseService
{

    public FirebaseService()
    {

        if (FirebaseApp.DefaultInstance == null)
        {

            FirebaseApp.Create(
                new AppOptions
                {
                    Credential =
                    CredentialFactory
                    .FromFile<ServiceAccountCredential>("firebase-admin.json")
                    .ToGoogleCredential()
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
