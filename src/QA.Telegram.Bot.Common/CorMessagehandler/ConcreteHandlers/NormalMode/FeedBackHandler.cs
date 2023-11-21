using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.NormalMode;

public class FeedBackHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;

    public FeedBackHandler(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text == TelegramCommands.FEEDBACK)
        {
            await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.AppFeedBack);
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.FEEDBACK_MESSAGE,
                replyMarkup: TelegramMarkups.GO_TO_MENU,
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