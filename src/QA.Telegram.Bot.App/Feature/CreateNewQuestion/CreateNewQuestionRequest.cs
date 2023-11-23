using System.Net;
using System.Text;
using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.CategoryStatistics;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.CreateNewQuestion;

public record CreateNewQuestionRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class CreateNewQuestionRequestHandler : IRequestHandler<CreateNewQuestionRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public CreateNewQuestionRequestHandler(IQaRepo repo)
    {
        _repo = repo;
    }

    public async Task<QaBotResponse> Handle(CreateNewQuestionRequest request, CancellationToken cancellationToken)
    {
        await _repo.SetTelegramUserMode(request.UserMessage.User.TelegramChatId, UserInputMode.CreateQuestion);
        var categories = _repo.GetAllCategories();
        return new QaBotResponse
        {
            Text = TelegramMessages.REQUEST_CATEGORY_FOR_NEW_QA,
            Keyboard = TelegramMarkups.CATEGORIES_WITH_MENU_AND_ALL_SELECTED(categories.Select(x => x.Name)),
        };
    }
}