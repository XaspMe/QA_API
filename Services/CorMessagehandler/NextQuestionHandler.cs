using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QA_API.Constants;
using QA_API.Data;
using QA_API.Models;
using QA_API.Services.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA_API.CorMessagehandler;

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
        if (message.Text == TelegramCommands.NEXT_QUESTION)
        {
            var userChosenCategories = await _repo.GetTelegramUserCategories(message.Chat.Id);
            
            try
            {
                var question = new QAElement();
                
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
                    text: $"Категория: {question.Category.Name}\n{question.Question?.Replace("<br>", "\n") ?? string.Empty}",
                    replyMarkup: TelegramMarkups.QUESTIONS_KEYBOARD(await _repo.IsElementTelegramUserFavorite(message.Chat.Id, question)),
                    cancellationToken: _ct,
                    parseMode: ParseMode.Html);
            }
            catch (Exception e)
            {
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: TelegramMessages.ERROR,
                    // todo move to admins list
                    replyMarkup: TelegramMarkups.MAIN_MENU(message.Chat.Id == 87584263),
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
            // todo reply markup
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.HANDLE_ERROR,
                cancellationToken: _ct);
        }
    }
}