using Microsoft.AspNetCore.Mvc;
using ZHamaster.Api.Data;
using ZHamaster.Api.Models;
using ZHamaster.Api.Services;

namespace ZHamaster.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly FirebaseService _firebase;
    private readonly AppDbContext _db;

    public AuthController(
        FirebaseService firebase,
        AppDbContext db)
    {
        _firebase = firebase;
        _db = db;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] TokenRequest request)
    {
        var firebaseUser =
            await _firebase.VerifyToken(
                request.IdToken
            );


        var user =
            _db.Users.FirstOrDefault(
                x => x.FirebaseUid == firebaseUser.Uid
            );


        if(user == null)
        {
            user = new AppUser
            {
                FirebaseUid = firebaseUser.Uid,

                Email =
                firebaseUser.Claims["email"]
                ?.ToString() ?? "",

                Name =
                firebaseUser.Claims["name"]
                ?.ToString() ?? "",

                PhotoUrl =
                firebaseUser.Claims["picture"]
                ?.ToString() ?? ""
            };


            _db.Users.Add(user);

            await _db.SaveChangesAsync();
        }


        return Ok(user);
    }
}


public class TokenRequest
{
    public string IdToken { get; set; } = "";
}
