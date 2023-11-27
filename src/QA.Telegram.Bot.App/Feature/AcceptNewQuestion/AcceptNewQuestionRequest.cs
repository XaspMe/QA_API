using System.Net;
using System.Text;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QA.Common.Extensions;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.CategoryStatistics;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.App.Feature.AcceptNewQuestion;

public record AcceptNewQuestionRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage, true);

public class AcceptNewQuestionRequestHandler : IRequestHandler<AcceptNewQuestionRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;
    private readonly IMemoryCache _cache;


    public AcceptNewQuestionRequestHandler(IQaRepo repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<QaBotResponse> Handle(AcceptNewQuestionRequest request, CancellationToken cancellationToken)
    {
        if (request.UserMessage.Message.Text is "" or
            TelegramCommands.MENU)
        {
            return await StopSaving(request.UserMessage.Message);
        }

        if (_cache.TryGetValue(request.UserMessage.Message.Chat.Id, out QAElement qaElement) && qaElement != null)
        {
            if (qaElement.Question is null)
            {
                return await SaveQuestionRequestAnswer(request.UserMessage.Message, qaElement);
            }

            if (qaElement.Answer is null)
            {
                return await SaveElementReturnUserToNormalMode(request.UserMessage.Message, qaElement);
            }
        }
        else
        {
            return await CacheNewElementCategory(request.UserMessage.Message);
        }

        // todo wtfs going on
        return await StopSaving(request.UserMessage.Message);
    }

    private async Task<QaBotResponse> SaveElementReturnUserToNormalMode(Message message, QAElement qaElement)
    {
        qaElement.Answer = message.Text;
        await _repo.CreateElementWithCategoryLoading(qaElement);
        _cache.Remove(message.Chat.Id);
        await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
        return new QaBotResponse()
        {
            Text = TelegramMessages.QA_ELEMENT_CREATED +
                   qaElement.TelegramMarkupShowAsString(),
            Keyboard = TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id)),
        };
    }

    private async Task<QaBotResponse> SaveQuestionRequestAnswer(Message message, QAElement qaElement)
    {
        qaElement.Question = message.Text;
        _cache.Set(message.Chat.Id, qaElement);
        return new QaBotResponse()
        {
            Text = TelegramMessages.REQUEST_ANSWER_FOR_NEW_QA,
            Keyboard = TelegramMarkups.GO_TO_MENU,
        };
    }

    private async Task<QaBotResponse> StopSaving(Message message)
    {
        await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
        return new QaBotResponse()
        {
            Text = message.Text is TelegramCommands.MENU
                ? TelegramMessages.MAIN_MENU_WITH_COUNT(_repo.ElementsCount())
                : TelegramMessages.HANDLE_ERROR,
            Keyboard = TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id)),
        };
    }

    private async Task<QaBotResponse> CacheNewElementCategory(Message message)
    {
        var categories = _repo.GetAllCategories().ToList();
        var categoriesNames = categories.Select(x => x.Name).ToList();

        if (categoriesNames.Any(x => message.Text!.Contains(x)))
        {
            return await CacheCategoryRequestQuestion(message);
        }
        else
        {
            return await ErrorSavingUnknownCategory(message);
        }
    }

    private async Task<QaBotResponse> CacheCategoryRequestQuestion(Message message)
    {
        var category = _repo.GetCategoryByName(message.Text!);
        var element = new QAElement { Category = category };
        _cache.Set(message.Chat.Id, element);
        return new QaBotResponse()
        {
            Text = TelegramMessages.REQUEST_QUESTION_FOR_NEW_QA,
            Keyboard = TelegramMarkups.GO_TO_MENU,
        };
    }

    private async Task<QaBotResponse> ErrorSavingUnknownCategory(Message message)
    {
        await _repo.SetTelegramUserMode(message.Chat.Id, UserInputMode.Normal);
        return new QaBotResponse()
        {
            Text = TelegramMessages.CATEGORY_INVALID,
            Keyboard = TelegramMarkups.MAIN_MENU(await _repo.IsTelegramUserAdmin(message.Chat.Id)),
        };
    }
}