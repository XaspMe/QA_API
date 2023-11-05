namespace QA_API.Models;

public class FeedBack
{
    public int Id { get; set; }
    public string Message { get; set; }
    public User User { get; set; }
}