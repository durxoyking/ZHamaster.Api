namespace ZHamaster.Api.Models;

public class Video
{
    public long Id {get;set;}

    public string Title {get;set;} = "";

    // ISO 3166-1 alpha-2 code supplied when a video is uploaded (e.g. BD).
    [System.ComponentModel.DataAnnotations.MaxLength(2)]
    public string? CountryCode { get; set; }

    public string Description {get;set;} = "";

    public string VideoUrl {get;set;} = "";

    public string ThumbnailUrl {get;set;} = "";

    public decimal CoinPrice {get;set;} = 0;

    public bool IsFree {get;set;} = true;

    public DateTime CreatedAt {get;set;}
        = DateTime.UtcNow;
}
