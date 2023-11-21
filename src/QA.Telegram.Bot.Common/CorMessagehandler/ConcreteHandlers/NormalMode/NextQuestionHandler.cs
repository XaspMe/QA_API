using System.Net;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.NormalMode;

public class NextQuestionHandler : MessageHandler
{
    private readonly IQaRepo _repo;
    private readonly CancellationToken _ct;
    private readonly ITelegramBotClient _telegramBotClient;

    public NextQuestionHandler(IQaRepo repo, ITelegramBotClient telegramBotClient, CancellationToken ct)
    {
        _repo = repo;
        _ct = ct;
        _telegramBotClient = telegramBotClient;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text is TelegramCommands.NEXT_QUESTION or TelegramCommands.REMOVE_FROM_FAVORITES)
        {
            if (message.Text is TelegramCommands.REMOVE_FROM_FAVORITES)
            {
                var curent = await _repo.GetElementOnCurrentTelegramUser(message.Chat.Id);
                if (curent != null)
                {
                    await _repo.RemoveFromTelegramUserFavoriteElements(message.Chat.Id, curent);
                    await _telegramBotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        // replace br's for telegram only
                        text: TelegramMessages.REMOVED_FROM_FAVORITES,
                        replyMarkup: TelegramMarkups.QUESTIONS_KEYBOARD(
                            await _repo.IsElementTelegramUserFavorite(message.Chat.Id, curent)),
                        cancellationToken: _ct,
                        parseMode: ParseMode.Html);
                    return;
                }
                return;
            }

            var userChosenCategories = await _repo.GetTelegramUserCategories(message.Chat.Id);

            try
            {
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
                    text: WebUtility.HtmlEncode($"Вопрос /{question.Id}\nКатегория: {question.Category.Name}\n{question.Question?.Replace("<br>", "\n") ?? string.Empty}"),
                    replyMarkup: TelegramMarkups.QUESTIONS_KEYBOARD(await _repo.IsElementTelegramUserFavorite(message.Chat.Id, question)),
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