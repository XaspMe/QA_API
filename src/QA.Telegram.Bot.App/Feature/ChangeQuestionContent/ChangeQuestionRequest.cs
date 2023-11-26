using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.ChangeQuestionContent;

public record ChangeQuestionRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage, true);

public class ChangeQuestionRequestHandler : IRequestHandler<ChangeQuestionRequest, QaBotResponse>
{
    public IQaRepo _repo { get; set; }

    public ChangeQuestionRequestHandler(IQaRepo repo)
    {
        _repo = repo;
    }

    public async Task<QaBotResponse> Handle(ChangeQuestionRequest request, CancellationToken cancellationToken)
    {
        var question = await _repo.GetElementOnCurrentTelegramUser(
            request.UserMessage.User.TelegramChatId);
        await _repo.SetTelegramUserMode(request.UserMessage.User.TelegramChatId, UserInputMode.ChangeContentQuestion);
        return new QaBotResponse()
        {
            Text = $"{TelegramMessages.REQUEST_NEW_QUESTION}\nТекущий ответ:\n{question.Question}",
            Keyboard = TelegramMarkups.GO_TO_MENU,
        };
    }
}