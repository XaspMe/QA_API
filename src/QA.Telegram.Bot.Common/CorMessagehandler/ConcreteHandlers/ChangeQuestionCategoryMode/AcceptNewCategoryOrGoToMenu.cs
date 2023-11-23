using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.ChangeQuestionCategoryMode;

public class AcceptNewCategoryOrGoToMenu : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;

    public AcceptNewCategoryOrGoToMenu(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text != string.Empty && message.Text != TelegramCommands.MENU)
        {
            var categories = _repo.GetAllCategories().ToList();
            var categoriesNames = categories.Select(x => x.Name).ToList();

            if (categoriesNames.Any(x => message.Text!.Contains(x)))
            {
                var cat = categories.FirstOrDefault(x => x.Name == message.Text);
                QAElement elementOnCurrentTelegramUser = await _repo.GetElementOnCurrentTelegramUser(message.Chat.Id);
                await _repo.ChangeQuestionCategory(elementOnCurrentTelegramUser,
                    cat!);
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: TelegramMessages.QA_ELEMENT_MOVED_TO_NEW_CATEGORY +
                          $"\nКатегория:\n{elementOnCurrentTelegramUser.Category.Name}\nВопрос:\n{elementOnCurrentTelegramUser.Question}\nОтвет:\n{elementOnCurrentTelegramUser.Answer}",
                    replyMarkup: TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id)),
                    parseMode: ParseMode.Html,
                    cancellationToken: _ct);
            }
            else
            {
                await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: TelegramMessages.CATEGORY_INVALID,
                    replyMarkup: TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id)),
                    cancellationToken: _ct);
                return;
            }
        }

        await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
        await _telegramBotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: message.Text is TelegramCommands.MENU
                ? TelegramMessages.MAIN_MENU_WITH_COUNT(_repo.ElementsCount())
                : TelegramMessages.FEEDBACK_ACCEPTED_MESSAGE,
            replyMarkup: TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id)),
            cancellationToken: _ct);
    }
}