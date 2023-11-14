using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

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
    public UserInputMode UserInputMode { get; set; }
}