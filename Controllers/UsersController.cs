using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZHamaster.Api.Data;

namespace ZHamaster.Api.Controllers;

[Authorize(Policy = "Admin")]
[ApiController]
[Route("api/users")]
public class UsersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) =>
        Ok(await db.Users.AsNoTracking().OrderByDescending(x => x.CreatedAt).Take(100)
            .Select(x => new { x.Id, x.Name, x.Email, x.CreatedAt }).ToListAsync(cancellationToken));
}
