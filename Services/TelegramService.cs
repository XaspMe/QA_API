using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QA_API.CorMessagehandler;
using QA_API.Data;
using QA_API.Services.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA_API;

public class TelegramService : IHostedService
{
    private readonly IServiceScopeFactory scopeFactory;
    private Dictionary<long, int> userCurrentElement = new Dictionary<long, int>();

    public TelegramService(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // todo move to config
        var botClient = new TelegramBotClient("");

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

        var me = await botClient.GetMeAsync();

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

        Console.WriteLine($"Received a '{messageText}' message in chat {message.Chat.Id}.");
        using var scope = scopeFactory.CreateScope();
        MessageHandler mainHandler = new StartHandler(botClient, message.Chat.Id, cancellationToken);
        MessageHandler nextQuestionhandler = new NextQuestionHandler(
            scope.ServiceProvider.GetRequiredService<IQaRepo>(),
            userCurrentElement,
            botClient,
            message.Chat.Id,
            cancellationToken);
        MessageHandler answerCurrentQuestionHandler = new AnswerCurrentQuestionHandler(
            scope.ServiceProvider.GetRequiredService<IQaRepo>(),
            userCurrentElement,
            botClient,
            message.Chat.Id,
            cancellationToken);

        mainHandler.SetNextHandler(nextQuestionhandler);
        nextQuestionhandler.SetNextHandler(answerCurrentQuestionHandler);

        await mainHandler.HandleMessage(message);
    }

    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}