using System.Net;
using MediatR;
using QA.Data;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.App.Feature.SelectedQuestion;

public record SelectedQuestionRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage);

public class SelectedQuestionRequestHandler : IRequestHandler<SelectedQuestionRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public SelectedQuestionRequestHandler(IQaRepo qaRepo)
    {
        _repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(SelectedQuestionRequest request, CancellationToken cancellationToken)
    {
        var id = int.TryParse(
            request.UserMessage.Message.Text!.Replace("/", string.Empty), out var number);
        var targetQuestion = _repo.GetElementById(number);
        if (!id || targetQuestion == null)
        {
            return new QaBotResponse()
            {
                Text = TelegramMessages.HANDLE_ERROR,
                Keyboard = TelegramMarkups.GO_TO_MENU,
            };
        }

        await _repo.SetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id, targetQuestion);
        return new QaBotResponse()
        {
            Text = WebUtility.HtmlEncode(
                $"Вопрос /{targetQuestion.Id}\nКатегория: {targetQuestion.Category.Name}\n{targetQuestion.Question?.Replace("<br>", "\n") ?? string.Empty}"),
            Keyboard = TelegramMarkups.SELECTED_QUESTION_KEYBOARD(
                await _repo.IsElementTelegramUserFavorite(
                    request.UserMessage.Message.Chat.Id,
                    targetQuestion),
                request.UserMessage.User.isAdmin)
        };
    }
}