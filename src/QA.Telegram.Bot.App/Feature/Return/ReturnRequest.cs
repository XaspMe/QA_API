using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.AcceptNewQuestionCategory;
using QA.Telegram.Bot.App.Feature.Favorites;
using QA.Telegram.Bot.App.Feature.Menu;
using QA.Telegram.Bot.App.Feature.NextQuestion;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.Return;

public record ReturnRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage);

public class ReturnRequestHandler : IRequestHandler<ReturnRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;
    private readonly IMediator _mediator;

    public ReturnRequestHandler(IQaRepo repo, IMediator mediator)
    {
        _repo = repo;
        _mediator = mediator;
    }

    public async Task<QaBotResponse> Handle(ReturnRequest request, CancellationToken cancellationToken)
    {
        var step = await _repo.GetUserCurrentStep(request.UserMessage.Message.Chat.Id);
        switch (step)
        {
            case UserCurrentStep.Menu:
                return await _mediator.Send(new MenuRequest(request.UserMessage), cancellationToken);
            case UserCurrentStep.Questions:
                return await _mediator.Send(new NextQuestionRequest(request.UserMessage), cancellationToken);
            case UserCurrentStep.FavoriteQuestion:
                return await _mediator.Send(new NextFavoritesRequest(request.UserMessage), cancellationToken);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}