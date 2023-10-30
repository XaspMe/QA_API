﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QA_API.CorMessagehandler;
using QA_API.Data;
using QA_API.Services.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace QA_API;

public class TelegramService : IHostedService
{
    private readonly IServiceScopeFactory scopeFactory;
    private Dictionary<long, int> userCurrentElement = new Dictionary<long, int>();
    private Dictionary<long, int> userCurrentCategory = new Dictionary<long, int>();
    private Dictionary<long, DateTime> lastMessage = new Dictionary<long, DateTime>();
    private Dictionary<long, int> userFavorites = new Dictionary<long, int>();

    public TelegramService(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // todo move to config
        var botClient = new TelegramBotClient("6598126500:AAGwJxzSV8xtE7d-7FL0SoTMmSBIEe0QwYM");

        using CancellationTokenSource cts = new ();
        ReceiverOptions receiverOptions = new ()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync(cancellationToken: cts.Token);

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();
        cts.Cancel();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;
        if (message.Text is not { } messageText)
            return;

        // if (lastMessage.TryGetValue(message.Chat.Id, out var lastUserMessage) &&
        //     (DateTime.Now - lastUserMessage).TotalSeconds < 1)
        // {
        //     Console.WriteLine("DDOS handling");
        //     return;
        // }

        Console.WriteLine($"Received a '{messageText}' message in chat {message.Chat.Id}.");
        using var scope = scopeFactory.CreateScope();

        IQaRepo qaRepo = scope.ServiceProvider.GetRequiredService<IQaRepo>();
        MessageHandler menuHandler = new MenuHandler(botClient,
            cancellationToken,
            qaRepo);
        MessageHandler nextQuestionHandler = new NextQuestionHandler(
            qaRepo,
            userCurrentElement,
            botClient,
            cancellationToken,
            userCurrentCategory);
        MessageHandler answerCurrentQuestionHandler = new AnswerCurrentQuestionHandler(
            qaRepo,
            userCurrentElement,
            botClient,
            cancellationToken);
        MessageHandler categoriesHandler = new CategoriesHandler(
            botClient,
            cancellationToken,
            qaRepo);
        SelectCategoriesHandler selectCategories = new SelectCategoriesHandler(
            qaRepo,
            userCurrentCategory,
            userCurrentElement,
            botClient,
            cancellationToken);
        CategoryStatisticsHandler categoryStatisticsHandler = new CategoryStatisticsHandler(
            botClient,
            cancellationToken,
            qaRepo);
        AddToFavoritesHandler addToFavoritesHandler = new AddToFavoritesHandler(
            userCurrentElement, botClient, cancellationToken, userFavorites);

        menuHandler.SetNextHandler(nextQuestionHandler);
        nextQuestionHandler.SetNextHandler(answerCurrentQuestionHandler);
        answerCurrentQuestionHandler.SetNextHandler(categoriesHandler);
        categoriesHandler.SetNextHandler(selectCategories);
        selectCategories.SetNextHandler(categoryStatisticsHandler);
        categoryStatisticsHandler.SetNextHandler(addToFavoritesHandler);

        await menuHandler.HandleMessage(message);
    }

    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}