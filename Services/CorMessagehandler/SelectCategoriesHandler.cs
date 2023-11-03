using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

public class SelectCategoriesHandler : MessageHandler
{
    private readonly IQaRepo _repo;
    private readonly CancellationToken _ct;
    private readonly ITelegramBotClient _telegramBotClient;

    public SelectCategoriesHandler(IQaRepo repo, ITelegramBotClient telegramBotClient, CancellationToken ct)
    {
        _repo = repo;
        _ct = ct;
        _telegramBotClient = telegramBotClient;
    }

    public override async Task HandleMessage(Message message)
    {
        try
        {
            var categories = _repo.GetAllCategories().ToList();
            var categoriesNames = categories.Select(x => x.Name).ToList();
            
            if (categoriesNames.Any(x => message.Text!.Contains(x)) || message.Text!.Contains(TelegramCommands.ALL_CATEGORIES))
            {
                if (message.Text!.Contains(TelegramCommands.ALL_CATEGORIES))
                {
                    await _repo.UpdateTelegramUserFavoriteCategories(message.Chat.Id, new List<QACategory>());
                }
                else
                {
                    var userDbCats = categories.FirstOrDefault(x => x.Name == message.Text);
                    await _repo.UpdateTelegramUserFavoriteCategories(message.Chat.Id, new List<QACategory>() {
                        userDbCats
                    });
                }

                var telegramUserCategories = await _repo.GetTelegramUserCategories(message.Chat.Id);
                var question = message.Text!.Contains(TelegramCommands.ALL_CATEGORIES)
                    ? _repo.GetElementRandom()
                    // todo refactor to many categories
                    : _repo.GetElementRandomInCategory(telegramUserCategories.FirstOrDefault().Id);
                await _repo.SetElementOnCurrentTelegramUser(message.Chat.Id, question);

                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: WebUtility.HtmlEncode($"Категория: {question.Category.Name}\n{question.Question?.Replace("<br>", "\n") ?? string.Empty}"),
                    replyMarkup: TelegramMarkups.QUESTIONS_KEYBOARD(await _repo.IsElementTelegramUserFavorite(message.Chat.Id, question)),
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
        catch (Exception e)
        {
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.ERROR,
                // todo move to admins list
                replyMarkup: TelegramMarkups.MAIN_MENU(message.Chat.Id != 87584263),
                cancellationToken: _ct);
            throw e;
        }
    }
}