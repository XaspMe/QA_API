using Telegram.Bot.Types;
using User = QA.Models.Models.User;

namespace QA.Telegram.Bot.Models;

public class TelegramUserMessage
{
    public TelegramUserMessage(Message message, User user)
    {
        Message = message;
        User = user;
    }

    public Message Message { get; set; }
    public User User { get; set; }
}