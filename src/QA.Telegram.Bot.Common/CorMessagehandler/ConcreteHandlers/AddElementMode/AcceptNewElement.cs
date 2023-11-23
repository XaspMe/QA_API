using Microsoft.Extensions.Caching.Memory;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.AddElementMode;

public class AcceptNewElement : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;
    private readonly IMemoryCache _cache;

    public AcceptNewElement(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo,
        IMemoryCache cache)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
        _cache = cache;
    }

    public override async Task HandleMessage(Message message)
    {
        // todo refactor
        if (message.Text is "" or
            TelegramCommands.MENU)
        {
            await ErrorSaving(message);
            return;
        }

        if (_cache.TryGetValue(message.Chat.Id, out QAElement qaElement) && qaElement != null)
        {
            if (qaElement.Question is null)
            {
                await SaveQuestionRequestAnswer(message, qaElement);
                return;
            }
            if (qaElement.Answer is null)
            {
                await SaveElementReturnUserToNormalMode(message, qaElement);
            }
        }
        else
        {
            await CacheNewElementCategory(message);
        }
    }

    private async Task SaveElementReturnUserToNormalMode(Message message, QAElement qaElement)
    {
        qaElement.Answer = message.Text;
        await _telegramBotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: TelegramMessages.QA_ELEMENT_CREATED +
                  $"\nКатегория:\n{qaElement.Category.Name}\nВопрос:\n{qaElement.Question}\nОтвет:\n{qaElement.Answer}",
            replyMarkup: TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id)),
            parseMode: ParseMode.Html,
            cancellationToken: _ct);
        await _repo.CreateElementWithCategoryLoading(qaElement);
        // _repo.CreateElement(qaElement);
        _cache.Remove(message.Chat.Id);
        await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
    }

    private async Task SaveQuestionRequestAnswer(Message message, QAElement qaElement)
    {
        qaElement.Question = message.Text;
        _cache.Set(message.Chat.Id, qaElement);
        await _telegramBotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: TelegramMessages.REQUEST_ANSWER_FOR_NEW_QA,
            replyMarkup: TelegramMarkups.GO_TO_MENU,
            cancellationToken: _ct);
    }

    private async Task ErrorSaving(Message message)
    {
        await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
        var replyKeyboardMarkup = TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id));
        await _telegramBotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: message.Text is TelegramCommands.MENU
                ? TelegramMessages.MAIN_MENU_WITH_COUNT(_repo.ElementsCount())
                : TelegramMessages.HANDLE_ERROR,
            replyMarkup: replyKeyboardMarkup,
            cancellationToken: _ct);
        await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
    }

    private async Task CacheNewElementCategory(Message message)
    {
        var categories = _repo.GetAllCategories().ToList();
        var categoriesNames = categories.Select(x => x.Name).ToList();

        if (categoriesNames.Any(x => message.Text!.Contains(x)))
        {
            await CacheCategoryRequestQuestion(message);
        }
        else
        {
            await ErrorSavingUnknownCategory(message);
        }
    }

    private async Task CacheCategoryRequestQuestion(Message message)
    {
        var category = _repo.GetCategoryByName(message.Text!);
        var element = new QAElement { Category = category };
        _cache.Set(message.Chat.Id, element);
        await _telegramBotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: TelegramMessages.REQUEST_QUESTION_FOR_NEW_QA,
            replyMarkup: TelegramMarkups.GO_TO_MENU,
            cancellationToken: _ct);
    }

    private async Task ErrorSavingUnknownCategory(Message message)
    {
        await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
        await _telegramBotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: TelegramMessages.CATEGORY_SELECT_FAIL,
            replyMarkup: TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id)),
            cancellationToken: _ct);
        await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
    }
}