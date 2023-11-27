using MediatR;
using QA.Common.Extensions;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.AcceptNewQuestionCategory;
using QA.Telegram.Bot.App.Feature.EditQuestion;
using QA.Telegram.Bot.App.Feature.Menu;
using QA.Telegram.Bot.App.Feature.Return;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot;

namespace QA.Telegram.Bot.App.Feature.AcceptQuestionUpdate;

public record AcceptAnswerUpdateRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage, true);

public class AcceptAnswerUpdateRequestHandler : IRequestHandler<AcceptAnswerUpdateRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;
    private readonly IMediator _mediator;

    public AcceptAnswerUpdateRequestHandler(IQaRepo repo, IMediator mediator)
    {
        _repo = repo;
        _mediator = mediator;
    }

    public async Task<QaBotResponse> Handle(AcceptAnswerUpdateRequest request, CancellationToken cancellationToken)
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
        question.Answer = request.UserMessage.Message.Text;
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