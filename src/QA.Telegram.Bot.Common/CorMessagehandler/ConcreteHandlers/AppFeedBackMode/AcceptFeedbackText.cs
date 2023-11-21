﻿using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.AppFeedBackMode;

public class AcceptFeedbackText : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;

    public AcceptFeedbackText(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text != string.Empty && message.Text != TelegramCommands.MENU)
            await _repo.AddTelegramUserFeedBack(message.Chat.Id, message.Text);
        await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
        var categories = _repo.GetAllCategories();
        // todo move to admins list
        var replyKeyboardMarkup = TelegramMarkups.MAIN_MENU(message.Chat.Id == 87584263);
        await _telegramBotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: message.Text is TelegramCommands.MENU
                ? TelegramMessages.MAIN_MENU_SELECTOR(_repo.ElementsCount())
                : TelegramMessages.FEEDBACK_ACCEPTED_MESSAGE,
            replyMarkup: replyKeyboardMarkup,
            cancellationToken: _ct);
    }
}