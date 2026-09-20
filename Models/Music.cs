namespace ZHamaster.Api.Models;

public class Music
{
    public long Id {get;set;}

    public string Title {get;set;} = "";

    public string AudioUrl {get;set;} = "";

    public string CoverUrl {get;set;} = "";

    public DateTime CreatedAt {get;set;}
        = DateTime.UtcNow;
}
