using System.Net;
using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.Favorites;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.EditQuestion;

public record EditQuestionRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage, true);

public class EditQuestionRequestHandler : IRequestHandler<EditQuestionRequest, QaBotResponse>
{

    public Task<QaBotResponse> Handle(EditQuestionRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new QaBotResponse
        {
            Text = TelegramMessages.SELECTOR,
            Keyboard = TelegramMarkups.EDIT_QUESTION_KEYBOARD,
        });
    }
}