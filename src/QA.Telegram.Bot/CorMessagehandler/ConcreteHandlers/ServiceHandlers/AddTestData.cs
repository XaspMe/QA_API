using QA.Common.Data;
using QA.Common.Services;
using QA.Telegram.Bot.Constants;
using QA.Telegram.Bot.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA.Telegram.Bot.CorMessagehandler.ConcreteHandlers.ServiceHandlers;

public class AddTestData  : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;

    public AddTestData(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        // todo highlight if already favorite
        if (message.Text == TelegramCommands.ADD_TEST_DATA)
        {
            await new TestDataAppender().Append(_repo);
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
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.HANDLE_ERROR,
                cancellationToken: _ct);
        }
    }
}