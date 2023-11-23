using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.AddCategoryMode;

public class AcceptNewCategory : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;

    public AcceptNewCategory(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text != string.Empty && message.Text != TelegramCommands.MENU)
        {
            await _repo.CreateTelegramUserQaCategory(message.Chat.Id, new QACategory() { Name = message.Text });
        }
        await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
        var categories = _repo.GetAllCategories();
        var replyKeyboardMarkup = TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id));
        await _telegramBotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: message.Text is TelegramCommands.MENU
                ? TelegramMessages.MAIN_MENU_WITH_COUNT(_repo.ElementsCount())
                : TelegramMessages.CATEGORY_CREATED,
            replyMarkup: replyKeyboardMarkup,
            cancellationToken: _ct);
    }
}