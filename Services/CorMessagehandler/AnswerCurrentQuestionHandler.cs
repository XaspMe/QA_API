using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QA_API.Data;
using QA_API.Services.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace QA_API.CorMessagehandler;

public class AnswerCurrentQuestionHandler : MessageHandler
{
    private readonly IQaRepo _repo;
    private readonly long _chatId;
    private readonly CancellationToken _ct;
    private readonly Dictionary<long, int> _userCurrentQuestion;
    private readonly ITelegramBotClient _telegramBotClient;

    public AnswerCurrentQuestionHandler(IQaRepo repo, Dictionary<long, int> userCurrentQuestion, ITelegramBotClient telegramBotClient, long chatId, CancellationToken ct)
    {
        _repo = repo;
        _chatId = chatId;
        _ct = ct;
        _userCurrentQuestion = userCurrentQuestion;
        _telegramBotClient = telegramBotClient;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text == "Ответ" && _userCurrentQuestion.TryGetValue(_chatId, out var value))
        {
            var question = _repo.GetElementById(value);

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Следующий вопрос" },
                new KeyboardButton[] { "Ответ" },
            });

            // todo добавить категории
            await _telegramBotClient.SendTextMessageAsync(
                chatId: _chatId,
                text: question.Answer.Replace("<br>", "\n"),
                replyMarkup: replyKeyboardMarkup,
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
                chatId: _chatId,
                text: "Извините, я не могу обработать ваше сообщение.",
                cancellationToken: _ct);
        }
    }
}