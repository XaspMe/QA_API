﻿using QA.Telegram.Bot.Constants;
using QA.Telegram.Bot.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA.Telegram.Bot.CorMessagehandler.ConcreteHandlers.NormalMode;

public class DeveloperContactsHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;

    public DeveloperContactsHandler(ITelegramBotClient telegramBotClient, CancellationToken ct)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text == TelegramCommands.DEVELOPER_CONTACTS)
        {
            // todo добавить категории
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.DEVELOPER_CONTACT,
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