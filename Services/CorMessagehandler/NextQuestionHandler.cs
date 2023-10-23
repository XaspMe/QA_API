using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QA_API.Constants;
using QA_API.Data;
using QA_API.Services.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace QA_API.CorMessagehandler;

public class NextQuestionHandler : MessageHandler
{
    private readonly IQaRepo _repo;
    private readonly CancellationToken _ct;
    private readonly Dictionary<long, int> _userCurrentQuestion;
    private readonly Dictionary<long, int> _userCurrentCategory;
    private readonly ITelegramBotClient _telegramBotClient;

    public NextQuestionHandler(IQaRepo repo, Dictionary<long, int> userCurrentQuestion,
        ITelegramBotClient telegramBotClient, CancellationToken ct, Dictionary<long, int> userCurrentCategory)
    {
        _repo = repo;
        _ct = ct;
        _userCurrentCategory = userCurrentCategory;
        _userCurrentQuestion = userCurrentQuestion;
        _telegramBotClient = telegramBotClient;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text!.Contains(TelegramCommands.NEXT_QUESTION))
        {
            var hasValue = _userCurrentCategory.TryGetValue(message.Chat.Id, out var value);
            try
            {
                var question = hasValue ? _repo.GetElementRandomInCategory(value) : _repo.GetElementRandom();
                _userCurrentQuestion[message.Chat.Id] = question.Id;

                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: question.Question,
                    replyMarkup: TelegramMarkups.QUESTIONS_KEYBOARD,
                    cancellationToken: _ct,
                    parseMode: ParseMode.Html);
            }
            catch (Exception e)
            {
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: TelegramMessages.ERROR,
                    replyMarkup: TelegramMarkups.MAIN_MENU,
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
            // todo reply markup
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.HANDLE_ERROR,
                cancellationToken: _ct);
        }
    }
}