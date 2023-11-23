using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace QA.Telegram.Bot.Models;

public class QaBotResponse
{
    public QaBotResponse()
    {

    }

    public string Text { get; set; }
    public ReplyKeyboardMarkup Keyboard { get; set; }
    public ParseMode ParseMode { get; set; } = ParseMode.Html;
}