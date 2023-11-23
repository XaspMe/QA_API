using System.Net;
using MediatR;
using QA.Data;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.ShowAnswer;

public record ShowAnswerRequest(TelegramUserMessage UserMessage) : MediatR.IRequest<QaBotResponse>;

public class ShowAnswerRequestHandler : IRequestHandler<ShowAnswerRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public ShowAnswerRequestHandler(IQaRepo qaRepo)
    {
        this._repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(ShowAnswerRequest request, CancellationToken cancellationToken)
    {
        var question = await this._repo.GetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id);
        return new QaBotResponse
        {
            Text = WebUtility.HtmlEncode(question.Answer?.Replace("<br>", "\n")) ?? string.Empty,
        };
    }
}