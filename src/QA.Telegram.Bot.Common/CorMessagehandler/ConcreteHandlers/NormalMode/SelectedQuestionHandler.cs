using System.Net;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.NormalMode;

public class SelectedQuestionHandler : MessageHandler
{
    private readonly IQaRepo _repo;
    private readonly CancellationToken _ct;
    private readonly ITelegramBotClient _telegramBotClient;

    public SelectedQuestionHandler(IQaRepo repo, ITelegramBotClient telegramBotClient, CancellationToken ct)
    {
        _repo = repo;
        _ct = ct;
        _telegramBotClient = telegramBotClient;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text.Contains("/") &&
            int.TryParse(message.Text.Replace("/", string.Empty), out var number) &&
            _repo.GetElementById(number) != null)
        {
            // todo check user have access to question, simplify if's
            var question = _repo.GetElementById(number);
            await _repo.SetElementOnCurrentTelegramUser(message.Chat.Id, question);
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                // replace br's for telegram only
                text: WebUtility.HtmlEncode(
                    $"Вопрос /{question.Id}\nКатегория: {question.Category.Name}\n{question.Question?.Replace("<br>", "\n") ?? string.Empty}"),
                replyMarkup: TelegramMarkups.QUESTIONS_KEYBOARD(
                    await _repo.IsElementTelegramUserFavorite(message.Chat.Id, question)),
                cancellationToken: _ct,
                parseMode: ParseMode.Html);
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