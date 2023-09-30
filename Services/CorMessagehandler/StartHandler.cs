using System.Threading;
using System.Threading.Tasks;
using QA_API.Services.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace QA_API.CorMessagehandler;

public class StartHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly long _chatId;
    private readonly CancellationToken _ct;

    public StartHandler(ITelegramBotClient telegramBotClient, long chatId, CancellationToken ct)
    {
        _telegramBotClient = telegramBotClient;
        _chatId = chatId;
        _ct = ct;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text == "/start")
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Следующий вопрос" },
                new KeyboardButton[] { "Ответ" },
            });

            // todo добавить категории
            await _telegramBotClient.SendTextMessageAsync(
                chatId: _chatId,
                text: "Привет! Я умею готовить вас к собеседованию по любому известному мне направлению, пока я знаю следующие темы:",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: _ct);
        }
        else if (_nextHandler != null)
        {
            await _nextHandler.HandleMessage(message);
        }
        else
        {
            // todo reply markup
            await _telegramBotClient.SendTextMessageAsync(
                chatId: _chatId,
                text: "Извините, я не могу обработать ваше сообщение.",
                cancellationToken: _ct);
        }
    }
}