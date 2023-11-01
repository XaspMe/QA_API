using System;
using System.Collections.Generic;

namespace QA_API.Models;

public class User
{
    public Guid Id { get; set; }
    public QAElement CurrentQuestion { get; set; }
    public long TelegramChatId { get; set; }
    public ICollection<QACategory> FavoriteCategories { get; set; }
}