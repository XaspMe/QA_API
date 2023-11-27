using MediatR;
using QA.Common.Extensions;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.EditQuestion;
using QA.Telegram.Bot.App.Feature.Menu;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot;

namespace QA.Telegram.Bot.App.Feature.AcceptQuestionUpdate;

public record AcceptQuestionUpdateRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage, true);

public class AcceptQuestionUpdateRequestHandler : IRequestHandler<AcceptQuestionUpdateRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;
    private readonly IMediator _mediator;

    public AcceptQuestionUpdateRequestHandler(IQaRepo repo, IMediator mediator)
    {
        _repo = repo;
        _mediator = mediator;
    }

    public async Task<QaBotResponse> Handle(AcceptQuestionUpdateRequest request, CancellationToken cancellationToken)
    {
        await _repo.SetTelegramUserMode(request.UserMessage.User.TelegramChatId, UserInputMode.Normal);
        if (string.IsNullOrEmpty(request.UserMessage.Message.Text))
        {
            await request.UserMessage.BotClient.SendTextMessageAsync(
                request.UserMessage.User.TelegramChatId,
                TelegramMessages.CONTENT_INVALID,
                cancellationToken: cancellationToken);
            return await _mediator.Send(new MenuRequest(request.UserMessage), cancellationToken);
        }

        if (request.UserMessage.Message.Text is TelegramCommands.MENU)
        {
            return await _mediator.Send(new MenuRequest(request.UserMessage), cancellationToken);
        }

        var question = await _repo.GetElementOnCurrentTelegramUser(request.UserMessage.User.TelegramChatId);
        question.Question = request.UserMessage.Message.Text;
        await _repo.UpdateElement(question);
        await request.UserMessage.BotClient.SendTextMessageAsync(
            request.UserMessage.User.TelegramChatId,
            TelegramMessages.UPDATE_SUCCESS,
            cancellationToken: cancellationToken);

        await request.UserMessage.BotClient.SendTextMessageAsync(
            request.UserMessage.User.TelegramChatId,
            question.TelegramMarkupShowAsString(),
            cancellationToken: cancellationToken);

        return await _mediator.Send(new EditQuestionRequest(request.UserMessage), cancellationToken);
    }
}