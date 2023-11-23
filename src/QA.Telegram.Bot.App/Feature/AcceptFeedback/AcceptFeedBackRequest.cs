using System.Net;
using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.Favorites;
using QA.Telegram.Bot.App.Feature.Menu;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.AcceptFeedback;

public record AcceptFeedBackRequest(TelegramUserMessage UserMessage) : MediatR.IRequest<QaBotResponse>;

public class AcceptFeedBackRequestHandler : IRequestHandler<AcceptFeedBackRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;
    private readonly IMediator _mediator;

    public AcceptFeedBackRequestHandler(IQaRepo qaRepo, IMediator mediator)
    {
        this._repo = qaRepo;
        _mediator = mediator;
    }

    public async Task<QaBotResponse> Handle(AcceptFeedBackRequest request, CancellationToken cancellationToken)
    {
        if (request.UserMessage.Message.Text != string.Empty &&
            request.UserMessage.Message.Text != TelegramCommands.MENU)
        {
            await this._repo.AddTelegramUserFeedBack(request.UserMessage.Message.Chat.Id, request.UserMessage.Message.Text!);
            await this._repo.SetTelegramUserMode(request.UserMessage.Message.Chat.Id, UserInputMode.Normal);
            return new QaBotResponse
            {
                Text = TelegramMessages.FEEDBACK_ACCEPTED_MESSAGE,
                Keyboard = TelegramMarkups.MAIN_MENU(
                    await this._repo.IsTelegramUserAdmin(request.UserMessage.Message.Chat.Id)),
            };
        }

        return await this._mediator.Send(new MenuRequest(request.UserMessage), cancellationToken);
    }
}