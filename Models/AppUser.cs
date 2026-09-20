namespace ZHamaster.Api.Models;

public class AppUser
{
    public long Id {get;set;}

    public string FirebaseUid {get;set;} = "";

    public string Name {get;set;} = "";

    public string Email {get;set;} = "";

    public string PhotoUrl {get;set;} = "";

    public decimal Coins {get;set;} = 0;

    public bool IsAdmin {get;set;} = false;

    public DateTime CreatedAt {get;set;}
        = DateTime.UtcNow;
}
