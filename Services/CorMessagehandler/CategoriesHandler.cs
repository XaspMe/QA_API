﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QA_API.Constants;
using QA_API.Data;
using QA_API.Services.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA_API.CorMessagehandler;

public class CategoriesHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;

    public CategoriesHandler(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text == TelegramCommands.CATEGORIES)
        {
            var categories = _repo.GetAllCategories();

            // todo добавить категории
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.CATEGORIES,
                replyMarkup: TelegramMarkups.CATEGORIES_KEYBOARD(categories.Select(x => x.Name)),
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