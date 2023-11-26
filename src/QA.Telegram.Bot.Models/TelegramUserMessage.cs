using Telegram.Bot;
using Telegram.Bot.Types;
using User = QA.Models.Models.User;

namespace QA.Telegram.Bot.Models;

public class TelegramUserMessage
{
    public TelegramUserMessage(Message message, User user, ITelegramBotClient botClient)
    {
        Message = message;
        User = user;
        BotClient = botClient;
    }

    public Message Message { get; set; }
    public User User { get; set; }
    public ITelegramBotClient BotClient { get; set; }
}