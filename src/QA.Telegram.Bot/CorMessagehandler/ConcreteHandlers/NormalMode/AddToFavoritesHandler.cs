using QA.Common.Data;
using QA.Telegram.Bot.Constants;
using QA.Telegram.Bot.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA.Telegram.Bot.CorMessagehandler.ConcreteHandlers.NormalMode;

public class AddToFavoritesHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;

    public AddToFavoritesHandler(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        // todo highlight if already favorite
        if (message.Text == TelegramCommands.ADD_TO_FAVORITES)
        {
            var question = await _repo.GetElementOnCurrentTelegramUser(message.Chat.Id);
            await _repo.AddToTelegramUserFavoriteElements(message.Chat.Id, question);

            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.ADDED_TO_FAVORITES,
                replyMarkup: TelegramMarkups.QUESTIONS_KEYBOARD(
                    await _repo.IsElementTelegramUserFavorite(message.Chat.Id, question)),
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