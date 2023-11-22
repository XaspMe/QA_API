namespace QA.Models.Models;

public class User
{
    public Guid Id { get; set; }
    public QAElement? CurrentQuestion { get; set; }
    public long TelegramChatId { get; set; }
    public ICollection<QACategory>? FavoriteCategories { get; set; }
    public ICollection<QACategory>? CategoriesCreated { get; set; }
    public ICollection<QAElement>? FavoriteElements { get; set; }
    public ICollection<FeedBack>? FeedBacks { get; set; }
    public UserCurrentStep UserCurrentStep { get; set; }
    public UserInputMode UserInputMode { get; set; }
    // am using it before adding proper auth approach
    public bool isAdmin { get; set; } = false;
}