namespace ZHamaster.Api.Models;

public class Photo
{
    public long Id {get;set;}

    public string ImageUrl {get;set;} = "";

    public string Title {get;set;} = "";

    public DateTime CreatedAt {get;set;}
        = DateTime.UtcNow;
}
