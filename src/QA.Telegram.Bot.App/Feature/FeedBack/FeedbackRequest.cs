using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.FeedBack;

public record FeedbackRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage);

public class FeedbackRequestHandler : IRequestHandler<FeedbackRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public FeedbackRequestHandler(IQaRepo qaRepo)
    {
        _repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(FeedbackRequest request, CancellationToken cancellationToken)
    {
        await _repo.SetTelegramUserMode(request.UserMessage.Message.Chat.Id, UserInputMode.AppFeedBack);
        return new QaBotResponse()
        {
            Text = TelegramMessages.FEEDBACK_MESSAGE,
            Keyboard = TelegramMarkups.GO_TO_MENU,
        };
    }
}