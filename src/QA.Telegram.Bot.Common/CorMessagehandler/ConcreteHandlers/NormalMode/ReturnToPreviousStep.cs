using System.Net;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.NormalMode;

public class ReturnToPreviousStep : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;

    public ReturnToPreviousStep(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text is TelegramCommands.RETURN)
        {
            var step = await _repo.GetUserCurrentStep(message.Chat.Id);

            switch (step)
            {
                case UserCurrentStep.Menu:
                    await SendMenu(message);
                    break;
                case UserCurrentStep.Questions:
                    await SendNextQuestion(message);
                    break;
                case UserCurrentStep.FavoriteQuestion:
                    await SendNextFavoritesQuestionOrGoToMenu(message);
                    break;
            };
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

    private async Task SendNextQuestion(Message message)
    {
        var userChosenCategories = await _repo.GetTelegramUserCategories(message.Chat.Id);
        QAElement question;

        var chosenCategories = userChosenCategories as QACategory[] ?? userChosenCategories.ToArray();
        if (chosenCategories.Count() == _repo.GetAllCategories().Count())
            question = _repo.GetElementRandom();
        // todo множественный выбор категорий
        else question = _repo.GetElementRandomInCategory(chosenCategories.FirstOrDefault()!.Id);
        await _repo.SetElementOnCurrentTelegramUser(message.Chat.Id, question);
        Console.WriteLine($"текущий вопрос {question.Id}");

        await _telegramBotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            // replace br's for telegram only
            text: WebUtility.HtmlEncode(
                $"Вопрос /{question.Id}\nКатегория: {question.Category.Name}\n{question.Question?.Replace("<br>", "\n") ?? string.Empty}"),
            replyMarkup: TelegramMarkups.QUESTIONS_KEYBOARD(
                await _repo.IsElementTelegramUserFavorite(message.Chat.Id, question),
                await _repo.IsTelegramUserAdmin(message.Chat.Id)),
            cancellationToken: _ct,
            parseMode: ParseMode.Html);
    }

    private async Task SendNextFavoritesQuestionOrGoToMenu(Message message)
    {
        // todo bug with go to menu but step is forbidden
        var question = await _repo.GetRandomElementFromTelegramUserFavorites(message.Chat.Id);
        if (question != null)
            await _repo.SetElementOnCurrentTelegramUser(message.Chat.Id, question);

        await _telegramBotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            // replace br's for telegram only
            text: question != null
                ? WebUtility.HtmlEncode(
                    $"Категория: {question.Category.Name}\n{question.Question?.Replace("<br>", "\n") ?? string.Empty}")
                : TelegramMessages.NO_FAVORITES,
            replyMarkup: question != null
                ? TelegramMarkups.FAVORITE_QUESTIONS_KEYBOARD()
                : TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id)),
            cancellationToken: _ct,
            parseMode: ParseMode.Html);
    }

    private async Task SendMenu(Message message)
    {
        var categories = _repo.GetAllCategories();
        var replyKeyboardMarkup = TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id));
        await _telegramBotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: message.Text is TelegramCommands.START
                ? TelegramMessages.HELLO(_repo.ElementsCount()) + "\n" + String.Join("\n", categories.Select(x => x.Name))
                : TelegramMessages.MAIN_MENU_SELECTOR(_repo.ElementsCount()),
            replyMarkup: replyKeyboardMarkup,
            cancellationToken: _ct);
    }
}