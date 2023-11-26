using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.ChangeQuestionContent;

public record ChangeAnswerRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage, true);

public class ChangeAnswerRequestHandler : IRequestHandler<ChangeAnswerRequest, QaBotResponse>
{
    public IQaRepo _repo { get; set; }

    public ChangeAnswerRequestHandler(IQaRepo repo)
    {
        _repo = repo;
    }

    public async Task<QaBotResponse> Handle(ChangeAnswerRequest request, CancellationToken cancellationToken)
    {
        var question = await _repo.GetElementOnCurrentTelegramUser(
            request.UserMessage.User.TelegramChatId);
        await _repo.SetTelegramUserMode(request.UserMessage.User.TelegramChatId, UserInputMode.ChangeContentAnswer);
        return new QaBotResponse()
        {
            Text = $"{TelegramMessages.REQUEST_NEW_ANSWER}\nТекущий ответ:\n{question.Answer}",
            Keyboard = TelegramMarkups.GO_TO_MENU,
        };
    }
}