using Microsoft.EntityFrameworkCore;
using ZHamaster.Api.Models;

namespace ZHamaster.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options
    ) : base(options)
    {

    }


    public DbSet<AppUser> Users { get; set; }

    public DbSet<Video> Videos { get; set; }

    public DbSet<Music> Musics { get; set; }

    public DbSet<Photo> Photos { get; set; }

    public DbSet<Transaction> Transactions { get; set; }
}
