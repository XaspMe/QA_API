﻿using System.Net;
using System.Text;
using QA.Data;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.NormalMode;

public class CategoryStatisticsHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;

    public CategoryStatisticsHandler(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text!.Contains(TelegramCommands.SHOW_CATEGORIES_STATISTICS))
        {
            var categories = _repo.GetAllCategories();

            var responseMessage = new StringBuilder();
            responseMessage.AppendLine("Статистика по вашим категориям");
            foreach (var stat in _repo.CategoriesStats())
            {
                responseMessage.AppendLine(WebUtility.HtmlEncode(stat));
            }

            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: responseMessage.ToString(),
                replyMarkup: TelegramMarkups.MAIN_MENU(false),
                cancellationToken: _ct);
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