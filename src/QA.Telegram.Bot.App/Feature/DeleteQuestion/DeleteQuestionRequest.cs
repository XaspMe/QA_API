using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.CreateNewQuestion;
using QA.Telegram.Bot.App.Feature.Return;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot;

namespace QA.Telegram.Bot.App.Feature.DeleteQuestion;

public record DeleteQuestionRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage, true);

public class DeleteQuestionRequestHandler : IRequestHandler<DeleteQuestionRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;
    private readonly IMediator _mediator;

    public DeleteQuestionRequestHandler(IQaRepo repo, IMediator mediator)
    {
        _repo = repo;
        _mediator = mediator;
    }

    public async Task<QaBotResponse> Handle(DeleteQuestionRequest request, CancellationToken cancellationToken)
    {
        var currentElement = await _repo.GetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id);
        await _repo.DeleteElement(currentElement);
        await request.UserMessage.BotClient.SendTextMessageAsync(
            request.UserMessage.Message.Chat.Id,
            TelegramMessages.DELETE_SUCCESS,
            cancellationToken: cancellationToken);
        return await _mediator.Send(new ReturnRequest(request.UserMessage), cancellationToken);
    }
}