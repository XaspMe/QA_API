using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.ChangeQuestionCategory;

public record ChangeQuestionCategoryRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage, true);

public class ChangeQuestionCategoryRequestHandler : IRequestHandler<ChangeQuestionCategoryRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public ChangeQuestionCategoryRequestHandler(IQaRepo repo)
    {
        _repo = repo;
    }

    public async Task<QaBotResponse> Handle(ChangeQuestionCategoryRequest request, CancellationToken cancellationToken)
    {
        await _repo.SetTelegramUserMode(request.UserMessage.Message.Chat.Id, UserInputMode.ChangeQuestionCategory);
        var categories = _repo.GetAllCategories();
        return new QaBotResponse()
        {
            Text = TelegramMessages.CATEGORIES,
            Keyboard = TelegramMarkups.CATEGORIES_WITH_MENU(categories.Select(x => x.Name)),
        };
    }
}