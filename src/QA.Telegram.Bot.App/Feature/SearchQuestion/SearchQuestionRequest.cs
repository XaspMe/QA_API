using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.AcceptFeedback;
using QA.Telegram.Bot.App.Feature.Menu;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.SearchQuestion;

public record SearchQuestionRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage);

public class SearchQuestionRequestHandler : IRequestHandler<SearchQuestionRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;
    private readonly IMediator _mediator;

    public SearchQuestionRequestHandler(IQaRepo qaRepo, IMediator mediator)
    {
        _repo = qaRepo;
        _mediator = mediator;
    }

    public async Task<QaBotResponse> Handle(SearchQuestionRequest request, CancellationToken cancellationToken)
    {
        await _repo.SetTelegramUserMode(request.UserMessage.Message.Chat.Id, UserInputMode.Search);
        return new QaBotResponse
        {
            Text = TelegramMessages.ENTER_SEARCH_KEYWORD,
            Keyboard = TelegramMarkups.GO_TO_MENU,
        };
    }
}