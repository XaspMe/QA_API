using System.Net;
using QA.Data;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.NormalMode;

public class MyFavoritesQuestionMessageHandler : MessageHandler
{
    private readonly IQaRepo _repo;
    private readonly CancellationToken _ct;
    private readonly ITelegramBotClient _telegramBotClient;

    public MyFavoritesQuestionMessageHandler(IQaRepo repo, ITelegramBotClient telegramBotClient, CancellationToken ct)
    {
        _repo = repo;
        _ct = ct;
        _telegramBotClient = telegramBotClient;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text is TelegramCommands.MY_FAVORITES_QUESTIONS
            or TelegramCommands.NEXT_FAVORITE_QUESTION
            or TelegramCommands.EXCLUDE_FROM_FAVORITES)
        {
            try
            {
                if (message.Text is TelegramCommands.EXCLUDE_FROM_FAVORITES)
                {
                    var curent = await _repo.GetElementOnCurrentTelegramUser(message.Chat.Id);
                    if (curent != null)
                    {
                        await _repo.RemoveFromTelegramUserFavoriteElements(message.Chat.Id, curent);
                        await _telegramBotClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            // replace br's for telegram only
                            text: TelegramMessages.REMOVED_FROM_FAVORITES,
                            cancellationToken: _ct);
                    }
                }


                var question = await _repo.GetRandomElementFromTelegramUserFavorites(message.Chat.Id);
                if (question != null)
                    await _repo.SetElementOnCurrentTelegramUser(message.Chat.Id, question);

                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    // replace br's for telegram only
                    text: question != null
                        ? WebUtility.HtmlEncode($"Категория: {question.Category.Name}\n{question.Question?.Replace("<br>", "\n") ?? string.Empty}")
                        : TelegramMessages.NO_FAVORITES,
                    replyMarkup: question != null
                        ? TelegramMarkups.FAVORITE_QUESTIONS_KEYBOARD()
                        : TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id)),
                    cancellationToken: _ct,
                    parseMode: ParseMode.Html);
            }
            catch (Exception e)
            {
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: TelegramMessages.ERROR,
                    replyMarkup: TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id)),
                    parseMode: ParseMode.Html,
                    cancellationToken: _ct);

                throw e;
            }
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