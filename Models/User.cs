using System;
using System.Collections.Generic;

namespace QA_API.Models;

public class User
{
    public Guid Id { get; set; }
    public QAElement CurrentQuestion { get; set; }
    public long TelegramChatId { get; set; }
    public ICollection<QACategory> FavoriteCategories { get; set; }
    public ICollection<QAElement> FavoriteElements { get; set; }
    public ICollection<FeedBack> FeedBacks { get; set; }
    public UserInputMode UserInputMode { get; set; }
}