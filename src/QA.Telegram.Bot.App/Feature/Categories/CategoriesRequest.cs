using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.Categories;

public record CategoriesRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class CategoriesRequestHandler : IRequestHandler<CategoriesRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public CategoriesRequestHandler(IQaRepo qaRepo)
    {
        _repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(CategoriesRequest request, CancellationToken cancellationToken)
    {
        await _repo.SetTelegramUserMode(request.UserMessage.User.TelegramChatId, UserInputMode.SelectCategory);
        var categories = _repo.GetAllCategories();
        return new QaBotResponse
        {
            Text = TelegramMessages.CATEGORIES,
            Keyboard = TelegramMarkups.CATEGORIES_WITH_MENU_AND_ALL_SELECTED(categories.Select(x => x.Name)),
        };
    }
}