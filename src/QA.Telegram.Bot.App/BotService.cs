using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.AcceptFeedback;
using QA.Telegram.Bot.App.Feature.AcceptNewCategory;
using QA.Telegram.Bot.App.Feature.AcceptNewQuestion;
using QA.Telegram.Bot.App.Feature.AcceptNewQuestionCategory;
using QA.Telegram.Bot.App.Feature.AddTestDataToDb;
using QA.Telegram.Bot.App.Feature.AddToFavorites;
using QA.Telegram.Bot.App.Feature.Categories;
using QA.Telegram.Bot.App.Feature.CategorySelected;
using QA.Telegram.Bot.App.Feature.CategoryStatistics;
using QA.Telegram.Bot.App.Feature.ChangeQuestionCategory;
using QA.Telegram.Bot.App.Feature.CreateNewQuestion;
using QA.Telegram.Bot.App.Feature.DeveloperContacts;
using QA.Telegram.Bot.App.Feature.Favorites;
using QA.Telegram.Bot.App.Feature.FeedBack;
using QA.Telegram.Bot.App.Feature.Menu;
using QA.Telegram.Bot.App.Feature.NewCategory;
using QA.Telegram.Bot.App.Feature.NextQuestion;
using QA.Telegram.Bot.App.Feature.RemoveFromFavorites;
using QA.Telegram.Bot.App.Feature.SelectedQuestion;
using QA.Telegram.Bot.App.Feature.ShowAnswer;
using QA.Telegram.Bot.App.Feature.Start;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
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
    private IMediator _mediator;

    public BotService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
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
        using var scope = _scopeFactory.CreateScope();

        _qaRepo = scope.ServiceProvider.GetRequiredService<IQaRepo>();
        _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await _qaRepo.СreateTelegramUserIfDoesntExist(message.Chat.Id);
        var user = await _qaRepo.GetTelegramUser(message.Chat.Id);
        TelegramUserMessage userMessage = new TelegramUserMessage(message, user);

        QaBotResponse? botResponse = null;

        switch (user.UserInputMode)
        {
            case UserInputMode.Normal:
                if (message.Text.Contains('/') &&
                    int.TryParse(message.Text.Replace("/", string.Empty), out var _))
                {
                    botResponse =
                        await _mediator.Send(new SelectedQuestionRequest(userMessage), cancellationToken);
                }
                else
                {
                    botResponse = messageText switch
                    {
                        TelegramCommands.START => await _mediator.Send(
                            new StartRequest(userMessage),
                            cancellationToken),
                        TelegramCommands.MENU => await _mediator.Send(
                            new MenuRequest(userMessage),
                            cancellationToken),
                        TelegramCommands.NEXT_QUESTION => await _mediator.Send(
                            new NextQuestionRequest(userMessage), cancellationToken),
                        TelegramCommands.REMOVE_FROM_FAVORITES => await _mediator.Send(
                            new RemoveFromFavoritesRequest(userMessage), cancellationToken),
                        TelegramCommands.SHOW_ANSWER => await _mediator.Send(
                            new ShowAnswerRequest(userMessage),
                            cancellationToken),
                        TelegramCommands.CATEGORIES => await _mediator.Send(
                            new CategoriesRequest(userMessage),
                            cancellationToken),
                        TelegramCommands.SHOW_CATEGORIES_STATISTICS => await _mediator.Send(
                            new CategoryStatisticsRequest(userMessage), cancellationToken),
                        TelegramCommands.ADD_TO_FAVORITES => await _mediator.Send(
                            new AddToFavoritesRequest(userMessage), cancellationToken),
                        TelegramCommands.DEVELOPER_CONTACTS => await _mediator.Send(
                            new DeveloperContactsRequest(userMessage), cancellationToken),
                        TelegramCommands.MY_FAVORITES_QUESTIONS => await _mediator.Send(
                            new FavoritesRequest(userMessage), cancellationToken),
                        TelegramCommands.FEEDBACK => await _mediator.Send(
                            new FeedbackRequest(userMessage),
                            cancellationToken),
                        TelegramCommands.CREATE_CATEGORY => await _mediator.Send(
                            new NewCategoryRequest(userMessage), cancellationToken),
                        TelegramCommands.CHANGE_QUESTION_CATEGORY => await _mediator.Send(
                            new ChangeQuestionCategoryRequest(userMessage),
                            cancellationToken),
                        TelegramCommands.ADD_QUESTION => await _mediator.Send(
                            new CreateNewQuestionRequest(userMessage), cancellationToken),
                        TelegramCommands.ADD_TEST_DATA => await _mediator.Send(
                            new AddTestDataRequest(userMessage), cancellationToken),
                        _ => botResponse
                    };
                }

                break;
            case UserInputMode.Favorites:
                break;
            case UserInputMode.AppFeedBack:
                botResponse = await _mediator.Send(new AcceptFeedBackRequest(userMessage), cancellationToken);
                break;
            case UserInputMode.CreateCategory:
                botResponse = await _mediator.Send(new AcceptNewCategoryRequest(userMessage), cancellationToken);
                break;
            case UserInputMode.CreateQuestion:
                botResponse = await _mediator.Send(new AcceptNewQuestionRequest(userMessage), cancellationToken);
                break;
            case UserInputMode.ChangeQuestionCategory:
                botResponse =
                    await _mediator.Send(new AcceptNewQuestionCategoryRequest(userMessage), cancellationToken);
                break;
            case UserInputMode.SelectCategory:
                botResponse = await _mediator.Send(new CategorySelectedRequest(userMessage), cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (botResponse is null)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.UNKNOWN_COMMAND,
                replyMarkup: TelegramMarkups.GO_TO_MENU,
                cancellationToken: cancellationToken);
        }
        else
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: botResponse.Text,
                replyMarkup: botResponse.Keyboard,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }



        // #region normal_mode
        // MessageHandler menuHandler = new MenuHandler(botClient, +
        //     cancellationToken,
        //     _qaRepo);
        // MessageHandler nextQuestionHandler = new NextQuestionHandler( +
        //     _qaRepo,
        //     botClient,
        //     cancellationToken);
        // MessageHandler answerCurrentQuestionHandler = new AnswerCurrentQuestionHandler( +
        //     _qaRepo,
        //     botClient,
        //     cancellationToken);
        // MessageHandler categoriesHandler = new CategoriesHandler( +
        //     botClient,
        //     cancellationToken,
        //     _qaRepo);
        // SelectCategoriesHandler selectCategories = new SelectCategoriesHandler( +
        //     _qaRepo,
        //     botClient,
        //     cancellationToken);
        // CategoryStatisticsHandler categoryStatisticsHandler = new CategoryStatisticsHandler( +
        //     botClient,
        //     cancellationToken,
        //     _qaRepo);
        // AddToFavoritesHandler addToFavoritesHandler = +
        //     new AddToFavoritesHandler(botClient, cancellationToken, _qaRepo);
        // DeveloperContactsHandler developerContactsHandler = new DeveloperContactsHandler( +
        //     botClient,
        //     cancellationToken);
        // MyFavoritesQuestionMessageHandler myFavoritesQuestionMessageHandler = +
        //     new MyFavoritesQuestionMessageHandler(_qaRepo, botClient, cancellationToken);
        // FeedBackHandler feedBackHandler = new FeedBackHandler(botClient, cancellationToken, _qaRepo); +
        // CreateCategoryHandler createCategoryHandler = new CreateCategoryHandler(botClient, cancellationToken, _qaRepo); +
        // AddTestData addTestData = new AddTestData(botClient, cancellationToken, _qaRepo);
        // CreateQuestionHandler createQuestionHandler = new CreateQuestionHandler(botClient, cancellationToken, _qaRepo);
        // SelectedQuestionHandler selectedQuestionHandler = +
        //     new SelectedQuestionHandler(_qaRepo, botClient, cancellationToken);
        // ChangeQuestionCategoryHandler questionCategoryHandler =
        //     new ChangeQuestionCategoryHandler(_qaRepo, botClient, cancellationToken); +
        // ReturnToPreviousStep previousStep = new ReturnToPreviousStep(botClient, cancellationToken, _qaRepo);
        //
        // menuHandler.SetNextHandler(nextQuestionHandler);
        // nextQuestionHandler.SetNextHandler(answerCurrentQuestionHandler);
        // answerCurrentQuestionHandler.SetNextHandler(categoriesHandler);
        // categoriesHandler.SetNextHandler(selectCategories);
        // selectCategories.SetNextHandler(categoryStatisticsHandler);
        // categoryStatisticsHandler.SetNextHandler(addToFavoritesHandler);
        // addToFavoritesHandler.SetNextHandler(developerContactsHandler);
        // developerContactsHandler.SetNextHandler(myFavoritesQuestionMessageHandler);
        // myFavoritesQuestionMessageHandler.SetNextHandler(feedBackHandler);
        // feedBackHandler.SetNextHandler(createCategoryHandler);
        // createCategoryHandler.SetNextHandler(addTestData);
        // addTestData.SetNextHandler(createQuestionHandler);
        // createQuestionHandler.SetNextHandler(selectedQuestionHandler);
        // selectedQuestionHandler.SetNextHandler(questionCategoryHandler);
        // questionCategoryHandler.SetNextHandler(previousStep);
        // #endregion
        //
        // #region app_feedback_mode
        // AcceptFeedbackText acceptFeedback = new AcceptFeedbackText(botClient, +
        //     cancellationToken,
        //     _qaRepo);
        // #endregion
        //
        // #region create_category
        // AcceptNewCategory acceptNewCategory = new AcceptNewCategory(botClient, cancellationToken, _qaRepo); +
        // #endregion
        //
        // #region create_element
        // AcceptNewElement acceptNewElement = new AcceptNewElement(botClient, cancellationToken, _qaRepo, _cache); +
        // #endregion
        //
        // #region change_element_category
        //
        // AcceptNewCategoryOrGoToMenu acceptNewCategoryOrGoToMenu = +
        //     new AcceptNewCategoryOrGoToMenu(botClient, cancellationToken, _qaRepo);
        //
        // #endregion
        //
        // try
        // {
        //     switch (await _qaRepo.GetTelegramUserMode(message.Chat.Id))
        //     {
        //         case UserInputMode.Normal: await menuHandler.HandleMessage(message); break;
        //         case UserInputMode.AppFeedBack: await acceptFeedback.HandleMessage(message); break;
        //         case UserInputMode.CreateCategory: await acceptNewCategory.HandleMessage(message); break;
        //         case UserInputMode.CreateQuestion: await acceptNewElement.HandleMessage(message); break;
        //         case UserInputMode.ChangeQuestionCategory: await acceptNewCategoryOrGoToMenu.HandleMessage(message); break;
        //     }
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e);
        // }
        //
        // // todo добавить раздел о приложении
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

// public class StartRequestHandler : IRequestHandler<StartRequest, QaBotResponce>
// {
//     private IQaRepo _repo;
//
//     public StartRequestHandler(IQaRepo qaRepo)
//     {
//         _repo = qaRepo;
//     }
//
//     public async Task<QaBotResponce> Handle(StartRequest request, CancellationToken cancellationToken)
//     {
//         await _repo.SetUserCurrentStep(request.UserMessage.Message.Chat.Id, UserCurrentStep.Menu);
//         var categories = _repo.GetAllCategories();
//         return new QaBotResponce(
//             TelegramMessages.HELLO(_repo.ElementsCount()) + "\n" +
//             String.Join("\n", categories.Select(x => x.Name)),
//             TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(request.UserMessage.Message.Chat.Id))
//         );
//     }
// }