using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QA_API.Constants;
using QA_API.Data;
using QA_API.Models;
using QA_API.Services.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace QA_API.CorMessagehandler;

public class AnswerCurrentQuestionHandler : MessageHandler
{
    private readonly IQaRepo _repo;
    private readonly CancellationToken _ct;
    private readonly ITelegramBotClient _telegramBotClient;

    public AnswerCurrentQuestionHandler(IQaRepo repo, ITelegramBotClient telegramBotClient, CancellationToken ct)
    {
        _repo = repo;
        _ct = ct;
        _telegramBotClient = telegramBotClient;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text == TelegramCommands.ANSWER_CURRENT_QUESTION)
        {
            var question = await _repo.GetElementOnCurrentTelegramUser(message.Chat.Id);
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                // replace br's for telegram only
                text: question.Answer?.Replace("<br>", "\n") ?? string.Empty,
                replyMarkup: TelegramMarkups.QUESTIONS_KEYBOARD,
                parseMode: ParseMode.Html,
                cancellationToken: _ct);
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