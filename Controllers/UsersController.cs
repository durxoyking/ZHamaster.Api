using Microsoft.AspNetCore.Mvc;
using ZHamaster.Api.Data;
using ZHamaster.Api.Models;

namespace ZHamaster.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }


    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_db.Users.ToList());
    }


    [HttpPost]
    public IActionResult Post(AppUser user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();

        return Ok(user);
    }
}
