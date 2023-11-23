using System.Net;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.NormalMode;

public class ChangeQuestionCategoryHandler : MessageHandler
{
    private readonly IQaRepo _repo;
    private readonly CancellationToken _ct;
    private readonly ITelegramBotClient _telegramBotClient;

    public ChangeQuestionCategoryHandler(IQaRepo repo, ITelegramBotClient telegramBotClient, CancellationToken ct)
    {
        _repo = repo;
        _ct = ct;
        _telegramBotClient = telegramBotClient;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text is TelegramCommands.CHANGE_QUESTION_CATEGORY)
        {
            try
            {
                await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.ChangeQuestionCategory);
                var categories = _repo.GetAllCategories();
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: TelegramMessages.CATEGORIES,
                    replyMarkup: TelegramMarkups.CATEGORIES_WITH_MENU(categories.Select(x => x.Name)),
                    cancellationToken: _ct);
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