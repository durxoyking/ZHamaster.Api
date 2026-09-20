namespace ZHamaster.Api.Models;

public class Transaction
{
    public long Id {get;set;}

    public long UserId {get;set;}

    public decimal Amount {get;set;}

    public string Type {get;set;} = "";

    public string PaymentMethod {get;set;} = "";

    public string Status {get;set;} = "";

    public DateTime CreatedAt {get;set;}
        = DateTime.UtcNow;
}
