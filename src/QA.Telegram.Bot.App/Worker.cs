using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QA.Common.Data;
using QA.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QA.Common.Data;
using QA.Models.Models;
using QA.Telegram.Bot.CorMessagehandler.@abstract;
using QA.Telegram.Bot.CorMessagehandler.ConcreteHandlers.AddCategoryMode;
using QA.Telegram.Bot.CorMessagehandler.ConcreteHandlers.AppFeedBackMode;
using QA.Telegram.Bot.CorMessagehandler.ConcreteHandlers.NormalMode;
using QA.Telegram.Bot.CorMessagehandler.ConcreteHandlers.ServiceHandlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.App;

public class BotService : BackgroundService
{
    // todo move this to repo
    private readonly IServiceScopeFactory scopeFactory;
    private Dictionary<long, DateTime> lastMessage = new Dictionary<long, DateTime>();
    private IQaRepo qaRepo;

    public BotService(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // move to config
        var botClient = new TelegramBotClient("6598126500:AAGwJxzSV8xtE7d-7FL0SoTMmSBIEe0QwYM");

        using CancellationTokenSource cts = new ();
        ReceiverOptions receiverOptions = new ()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        await botClient.ReceiveAsync(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync(cancellationToken: cts.Token);

        Console.WriteLine($"Start listening for @{me.Username}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
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
        // todo pass ct properly

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
        AddTestData addTestData = new AddTestData(botClient, cancellationToken, qaRepo);

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
        createCategoryHandler.SetNextHandler(addTestData);

        #endregion

        #region app_feedback_mode

        AcceptFeedbackText acceptFeedback = new AcceptFeedbackText(botClient,
            cancellationToken,
            qaRepo);

        #endregion

        #region create_category

        AcceptCategoryName acceptCategoryName = new AcceptCategoryName(botClient, cancellationToken, qaRepo);

        #endregion

        try
        {
            switch (await qaRepo.GetTelegramUserMode(message.Chat.Id))
            {
                case UserInputMode.Normal : await menuHandler.HandleMessage(message); break;
                case UserInputMode.AppFeedBack : await acceptFeedback.HandleMessage(message); break;
                case UserInputMode.CreateCategory : await acceptCategoryName.HandleMessage(message); break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // todo log all exceptions
        // var ErrorMessage = exception switch
        // {
        //     ApiRequestException apiRequestException
        //         => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        //     _ => exception.ToString()
        // };

        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}