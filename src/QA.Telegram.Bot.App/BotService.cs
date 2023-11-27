using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.AcceptFeedback;
using QA.Telegram.Bot.App.Feature.AcceptNewCategory;
using QA.Telegram.Bot.App.Feature.AcceptNewQuestion;
using QA.Telegram.Bot.App.Feature.AcceptNewQuestionCategory;
using QA.Telegram.Bot.App.Feature.AcceptQuestionUpdate;
using QA.Telegram.Bot.App.Feature.AddTestDataToDb;
using QA.Telegram.Bot.App.Feature.AddToFavorites;
using QA.Telegram.Bot.App.Feature.Categories;
using QA.Telegram.Bot.App.Feature.CategorySelected;
using QA.Telegram.Bot.App.Feature.CategoryStatistics;
using QA.Telegram.Bot.App.Feature.ChangeQuestionCategory;
using QA.Telegram.Bot.App.Feature.ChangeQuestionContent;
using QA.Telegram.Bot.App.Feature.CreateNewQuestion;
using QA.Telegram.Bot.App.Feature.DeleteQuestion;
using QA.Telegram.Bot.App.Feature.DeveloperContacts;
using QA.Telegram.Bot.App.Feature.EditQuestion;
using QA.Telegram.Bot.App.Feature.Favorites;
using QA.Telegram.Bot.App.Feature.FeedBack;
using QA.Telegram.Bot.App.Feature.ManageApp;
using QA.Telegram.Bot.App.Feature.Menu;
using QA.Telegram.Bot.App.Feature.NewCategory;
using QA.Telegram.Bot.App.Feature.NextQuestion;
using QA.Telegram.Bot.App.Feature.RemoveFromFavorites;
using QA.Telegram.Bot.App.Feature.Return;
using QA.Telegram.Bot.App.Feature.SelectedQuestion;
using QA.Telegram.Bot.App.Feature.ShowAnswer;
using QA.Telegram.Bot.App.Feature.Start;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.Exception;
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
            qaBotToken =
                Environment.GetEnvironmentVariable("QA_BOT_TOKEN", EnvironmentVariableTarget.Process);
            if (qaBotToken is "" or null)
            {
                throw new ArgumentException("QA_DB environment variable dos not exists on this machine or empty");
            }
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

        Console.WriteLine($"Received a '{messageText}' message in chat {message.Chat.Id}.");
        using var scope = _scopeFactory.CreateScope();

        _qaRepo = scope.ServiceProvider.GetRequiredService<IQaRepo>();
        _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await _qaRepo.СreateTelegramUserIfDoesntExist(message.Chat.Id);
        var user = await _qaRepo.GetTelegramUser(message.Chat.Id);
        TelegramUserMessage userMessage = new TelegramUserMessage(message, user, botClient);
        try
        {
            var botResponse = await RequestData(cancellationToken, userMessage);
            if (botResponse is null)
            {
                // todo make as unknown command
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
        }
        catch (AccessDeniedException e)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.ACCESS_DENIED,
                replyMarkup: TelegramMarkups.GO_TO_MENU,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            // todo only for debug mode
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: e.Message,
                replyMarkup: TelegramMarkups.GO_TO_MENU,
                cancellationToken: cancellationToken);
        }
    }

    private async Task<QaBotResponse?> RequestData(CancellationToken cancellationToken, TelegramUserMessage userMessage)
    {
        QaBotResponse? botResponse = null;
        switch (userMessage.User.UserInputMode)
        {
            case UserInputMode.Normal:
                if (userMessage.Message.Text!.Contains('/') &&
                    int.TryParse(userMessage.Message.Text.Replace("/", string.Empty), out _))
                {
                    botResponse =
                        await _mediator.Send(new SelectedQuestionRequest(userMessage), cancellationToken);
                }
                else
                {
                    botResponse = userMessage.Message.Text switch
                    {
                        TelegramCommands.START => await _mediator.Send(
                            new StartRequest(userMessage),
                            cancellationToken),
                        TelegramCommands.MENU => await _mediator.Send(
                            new MenuRequest(userMessage),
                            cancellationToken),
                        TelegramCommands.NEXT_QUESTION => await _mediator.Send(
                            new NextQuestionRequest(userMessage), cancellationToken),
                        TelegramCommands.MY_FAVORITES_QUESTIONS => await _mediator.Send(
                            new NextFavoritesRequest(userMessage), cancellationToken),
                        TelegramCommands.NEXT_FAVORITE_QUESTION => await _mediator.Send(
                            new NextFavoritesRequest(userMessage), cancellationToken),
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
                        TelegramCommands.RETURN => await _mediator.Send(
                            new ReturnRequest(userMessage), cancellationToken),
                        TelegramCommands.EDIT_QUESTION => await _mediator.Send(
                             new EditQuestionRequest(userMessage), cancellationToken),
                        TelegramCommands.EDIT_ANSWER_CONTENT => await _mediator.Send(
                            new ChangeAnswerRequest(userMessage), cancellationToken),
                        TelegramCommands.EDIT_QUESTION_CONTENT => await _mediator.Send(
                            new ChangeQuestionRequest(userMessage), cancellationToken),
                        TelegramCommands.DELETE_QUESTION => await _mediator.Send(
                            new DeleteQuestionRequest(userMessage), cancellationToken),
                        TelegramCommands.MANAGE => await _mediator.Send(
                            new ManageAppRequest(userMessage), cancellationToken),
                        _ => botResponse
                    };
                }

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
            case UserInputMode.ChangeContentAnswer:
                botResponse = await _mediator.Send(new AcceptAnswerUpdateRequest(userMessage), cancellationToken);
                break;
            case UserInputMode.ChangeContentQuestion:
                botResponse = await _mediator.Send(new AcceptQuestionUpdateRequest(userMessage), cancellationToken);
                break;
        }

        return botResponse;
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