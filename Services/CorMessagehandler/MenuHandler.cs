using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QA_API.Constants;
using QA_API.Data;
using QA_API.Services.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace QA_API.CorMessagehandler;

public class MenuHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;

    public MenuHandler(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text is TelegramCommands.START or TelegramCommands.MENU)
        {
            var categories = _repo.GetAllCategories();
            // todo move to admins list
            var replyKeyboardMarkup = TelegramMarkups.MAIN_MENU(message.Chat.Id == 87584263);
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: message.Text is TelegramCommands.START?
                    TelegramMessages.HELLO(_repo.ElementsCount()) + "\n" + String.Join("\n", categories.Select(x => x.Name)):
                    TelegramMessages.MAIN_MENU_SELECTOR(_repo.ElementsCount()),
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
                chatId: message.Chat.Id,
                text: TelegramMessages.HANDLE_ERROR,
                cancellationToken: _ct);
        }
    }
}