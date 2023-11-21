using Microsoft.Extensions.Caching.Memory;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.CorMessagehandler.@abstract;
using QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.AddCategoryMode;
using QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.AddElementMode;
using QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.AppFeedBackMode;
using QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.NormalMode;
using QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.ServiceHandlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.App;

public class BotService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private Dictionary<long, DateTime> _lastMessage = new Dictionary<long, DateTime>();
    private IQaRepo _qaRepo;
    private IMemoryCache _cache;

    public BotService(IServiceScopeFactory scopeFactory)
    {
        this._scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var qaBotToken =
            Environment.GetEnvironmentVariable("QA_BOT_TOKEN", EnvironmentVariableTarget.User);
        if (qaBotToken is "" or null)
        {
            throw new ArgumentException("QA_BOT_TOKEN environment variable dos not exists on this machine or empty");
        }

        var botClient = new TelegramBotClient(qaBotToken);

        using CancellationTokenSource cts = new ();
        ReceiverOptions receiverOptions = new ()
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
        };

        await botClient.ReceiveAsync(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token);

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
        {
            return;
        }

        if (message.Text is not { } messageText)
        {
            return;
        }

        // if (lastMessage.TryGetValue(message.Chat.Id, out var lastUserMessage) &&
        //     (DateTime.Now - lastUserMessage).TotalSeconds < 1)
        // {
        //     Console.WriteLine("DDOS handling");
        //     return;
        // }
        // todo pass ct properly

        Console.WriteLine($"Received a '{messageText}' message in chat {message.Chat.Id}.");
        using var scope = this._scopeFactory.CreateScope();

        this._qaRepo = scope.ServiceProvider.GetRequiredService<IQaRepo>();
        _cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        await _qaRepo.СreateTelegramUserIfDoesntExist(message.Chat.Id);

        #region normal_mode
        MessageHandler menuHandler = new MenuHandler(botClient,
            cancellationToken,
            _qaRepo);
        MessageHandler nextQuestionHandler = new NextQuestionHandler(
            _qaRepo,
            botClient,
            cancellationToken);
        MessageHandler answerCurrentQuestionHandler = new AnswerCurrentQuestionHandler(
            _qaRepo,
            botClient,
            cancellationToken);
        MessageHandler categoriesHandler = new CategoriesHandler(
            botClient,
            cancellationToken,
            _qaRepo);
        SelectCategoriesHandler selectCategories = new SelectCategoriesHandler(
            _qaRepo,
            botClient,
            cancellationToken);
        CategoryStatisticsHandler categoryStatisticsHandler = new CategoryStatisticsHandler(
            botClient,
            cancellationToken,
            _qaRepo);
        AddToFavoritesHandler addToFavoritesHandler =
            new AddToFavoritesHandler(botClient, cancellationToken, _qaRepo);
        DeveloperContactsHandler developerContactsHandler = new DeveloperContactsHandler(
            botClient,
            cancellationToken);
        MyFavoritesQuestionMessageHandler myFavoritesQuestionMessageHandler =
            new MyFavoritesQuestionMessageHandler(_qaRepo, botClient, cancellationToken);
        FeedBackHandler feedBackHandler = new FeedBackHandler(botClient, cancellationToken, _qaRepo);
        CreateCategoryHandler createCategoryHandler = new CreateCategoryHandler(botClient, cancellationToken, _qaRepo);
        AddTestData addTestData = new AddTestData(botClient, cancellationToken, _qaRepo);
        CreateQuestionHandler createQuestionHandler = new CreateQuestionHandler(botClient, cancellationToken, _qaRepo);
        SelectedQuestionHandler selectedQuestionHandler =
            new SelectedQuestionHandler(_qaRepo, botClient, cancellationToken);

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
        addTestData.SetNextHandler(createQuestionHandler);
        createQuestionHandler.SetNextHandler(selectedQuestionHandler);
        #endregion

        #region app_feedback_mode
        AcceptFeedbackText acceptFeedback = new AcceptFeedbackText(botClient,
            cancellationToken,
            _qaRepo);
        #endregion

        #region create_category
        AcceptNewCategory acceptNewCategory = new AcceptNewCategory(botClient, cancellationToken, _qaRepo);
        #endregion

        #region create_element
        AcceptNewElement acceptNewElement = new AcceptNewElement(botClient, cancellationToken, _qaRepo, _cache);
        #endregion

        try
        {
            switch (await _qaRepo.GetTelegramUserMode(message.Chat.Id))
            {
                case UserInputMode.Normal: await menuHandler.HandleMessage(message); break;
                case UserInputMode.AppFeedBack: await acceptFeedback.HandleMessage(message); break;
                case UserInputMode.CreateCategory: await acceptNewCategory.HandleMessage(message); break;
                case UserInputMode.CreateQuestion: await acceptNewElement.HandleMessage(message); break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        // todo добавить раздел о приложении
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