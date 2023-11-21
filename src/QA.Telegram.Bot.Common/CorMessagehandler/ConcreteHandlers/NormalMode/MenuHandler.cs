using QA.Data;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.NormalMode;

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
            var replyKeyboardMarkup = TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id));
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
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.HANDLE_ERROR,
                cancellationToken: _ct);
        }
    }
}