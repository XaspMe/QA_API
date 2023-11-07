using QA.Common.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Constants;
using QA.Telegram.Bot.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA.Telegram.Bot.CorMessagehandler.ConcreteHandlers.NormalMode;

public class CreateCategoryHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo; 

    public CreateCategoryHandler(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text == TelegramCommands.CREATE_CATEGORY)
        {
            await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.CreateCategory);
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.REQUEST_CATEGORY_NAME,
                replyMarkup: TelegramMarkups.GO_TO_MENU,
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