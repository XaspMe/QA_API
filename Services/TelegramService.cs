using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QA_API.Data;
using QA_API.Models;
using QA_API.Services.CorMessagehandler.@abstract;
using QA_API.Services.CorMessagehandler.ConcreteHandlers.AddCategoryMode;
using QA_API.Services.CorMessagehandler.ConcreteHandlers.AppFeedBackMode;
using QA_API.Services.CorMessagehandler.ConcreteHandlers.NormalMode;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA_API.Services;

public class TelegramService : IHostedService
{
    // todo move this to repo
    private readonly IServiceScopeFactory scopeFactory;
    private Dictionary<long, DateTime> lastMessage = new Dictionary<long, DateTime>();
    private IQaRepo qaRepo;

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

        qaRepo = scope.ServiceProvider.GetRequiredService<IQaRepo>();
        await qaRepo.СreateTelegramUserIfDoesntExist(message.Chat.Id);

        #region normal_mode

        MessageHandler menuHandler = new MenuHandler(botClient,
            cancellationToken,
            qaRepo);
        MessageHandler nextQuestionHandler = new NextQuestionHandler(
            qaRepo,
            botClient,
            cancellationToken);
        MessageHandler answerCurrentQuestionHandler = new AnswerCurrentQuestionHandler(
            qaRepo,
            botClient,
            cancellationToken);
        MessageHandler categoriesHandler = new CategoriesHandler(
            botClient,
            cancellationToken,
            qaRepo);
        SelectCategoriesHandler selectCategories = new SelectCategoriesHandler(
            qaRepo,
            botClient,
            cancellationToken);
        CategoryStatisticsHandler categoryStatisticsHandler = new CategoryStatisticsHandler(
            botClient,
            cancellationToken,
            qaRepo);
        AddToFavoritesHandler addToFavoritesHandler =
            new AddToFavoritesHandler(botClient, cancellationToken, qaRepo);
        DeveloperContactsHandler developerContactsHandler = new DeveloperContactsHandler(
            botClient,
            cancellationToken);
        MyFavoritesQuestionMessageHandler myFavoritesQuestionMessageHandler =
            new MyFavoritesQuestionMessageHandler(qaRepo, botClient, cancellationToken);
        FeedBackHandler feedBackHandler = new FeedBackHandler(botClient, cancellationToken, qaRepo);
        CreateCategoryHandler createCategoryHandler = new CreateCategoryHandler(botClient, cancellationToken, qaRepo);

        menuHandler.SetNextHandler(nextQuestionHandler);
        nextQuestionHandler.SetNextHandler(answerCurrentQuestionHandler);
        answerCurrentQuestionHandler.SetNextHandler(categoriesHandler);
        categoriesHandler.SetNextHandler(selectCategories);
        selectCategories.SetNextHandler(categoryStatisticsHandler);
        categoryStatisticsHandler.SetNextHandler(addToFavoritesHandler);
        addToFavoritesHandler.SetNextHandler(developerContactsHandler);
        developerContactsHandler.SetNextHandler(myFavoritesQuestionMessageHandler);
        myFavoritesQuestionMessageHandler.SetNextHandler(feedBackHandler);
        feedBackHandler.SetNextHandler(createCategoryHandler);

        #endregion

        #region app_feedback_mode

        AcceptFeedbackText acceptFeedback = new AcceptFeedbackText(botClient,
            cancellationToken,
            qaRepo);

        #endregion

        #region create_category

        AcceptCategoryName acceptCategoryName = new AcceptCategoryName(botClient, cancellationToken, qaRepo);

        #endregion

        switch (await qaRepo.GetTelegramUserMode(message.Chat.Id))
        {
            case UserInputMode.Normal : await menuHandler.HandleMessage(message); break;
            case UserInputMode.AppFeedBack : await acceptFeedback.HandleMessage(message); break;
            case UserInputMode.CreateCategory : await acceptCategoryName.HandleMessage(message); break;
        }
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