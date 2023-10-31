using System;

namespace QA_API.Models;

public class UserState
{
    public Guid Id { get; set; }
    public QAElement CurrentQuestion { get; set; }
    public long TelegramChatId { get; set; }
}