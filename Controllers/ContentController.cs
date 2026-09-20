using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZHamaster.Api.Data;

namespace ZHamaster.Api.Controllers;

[ApiController]
[Route("api/content")]
public class ContentController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(string? kind, string? q, int page = 1, int pageSize = 24,
        CancellationToken cancellationToken = default)
    {
        if (kind is not null && kind is not ("video" or "music" or "photo"))
            return BadRequest("Unknown content type.");
        if (page < 1 || page > 100 || pageSize < 1 || pageSize > 60 || q?.Length > 200)
            return BadRequest("Invalid pagination or search query.");
        var skip = (page - 1) * pageSize;
        var take = skip + pageSize + 1;
        var search = q?.Trim() ?? "";
        var items = new List<ContentItem>();
        // Paid media URLs are never exposed by this public catalog.
        if (kind is null or "video")
            items.AddRange(await db.Videos.AsNoTracking()
                .Where(x => x.VideoUrl != "" &&
                    (search == "" || x.Title.Contains(search)))
                .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).Take(take)
                .Select(x => new ContentItem(x.Id, "video", x.Title, x.ThumbnailUrl,
                    x.IsFree && x.CoinPrice == 0 ? x.VideoUrl : "", x.CreatedAt))
                .ToListAsync(cancellationToken));
        if (kind is null or "music")
            items.AddRange(await db.Musics.AsNoTracking()
                .Where(x => x.AudioUrl != "" && (search == "" || x.Title.Contains(search)))
                .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).Take(take)
                .Select(x => new ContentItem(x.Id, "music", x.Title, x.CoverUrl, x.AudioUrl, x.CreatedAt))
                .ToListAsync(cancellationToken));
        if (kind is null or "photo")
            items.AddRange(await db.Photos.AsNoTracking()
                .Where(x => x.ImageUrl != "" && (search == "" || x.Title.Contains(search)))
                .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).Take(take)
                .Select(x => new ContentItem(x.Id, "photo", x.Title, x.ImageUrl, x.ImageUrl, x.CreatedAt))
                .ToListAsync(cancellationToken));
        var result = items.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Kind)
            .ThenByDescending(x => x.Id).Skip(skip).Take(pageSize + 1).ToList();
        return Ok(new { items = result.Take(pageSize), hasMore = result.Count > pageSize, page });
    }

    public record ContentItem(long Id, string Kind, string Title, string ThumbnailUrl,
        string MediaUrl, DateTime CreatedAt);
}
