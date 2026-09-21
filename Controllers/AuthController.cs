using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZHamaster.Api.Data;
using ZHamaster.Api.Models;

namespace ZHamaster.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db) : ControllerBase
{
    // Accept only verified Firebase bearer tokens, never client-supplied roles.
    [HttpPost("login")]
    public async Task<IActionResult> Login(CancellationToken cancellationToken)
    {
        var uid = User.FindFirst("sub")!.Value;
        var user = await db.Users.FirstOrDefaultAsync(x => x.FirebaseUid == uid, cancellationToken);
        if (user == null)
        {
            user = new AppUser { FirebaseUid = uid };
            db.Users.Add(user);
        }
        user.Email = User.FindFirst("email")?.Value ?? "";
        user.Name = User.FindFirst("name")?.Value ?? "";
        user.PhotoUrl = User.FindFirst("picture")?.Value ?? "";
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { user.Id, user.Name, user.Email, user.PhotoUrl });
    }
}
