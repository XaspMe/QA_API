using System.Net;
using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.Favorites;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.NewCategory;

public record NewCategoryRequest(TelegramUserMessage UserMessage) : MediatR.IRequest<QaBotResponse>;

public class NewCategoryRequestHandler : IRequestHandler<NewCategoryRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public NewCategoryRequestHandler(IQaRepo qaRepo)
    {
        this._repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(NewCategoryRequest request, CancellationToken cancellationToken)
    {
        await this._repo.SetTelegramUserMode(request.UserMessage.Message.Chat.Id, UserInputMode.CreateCategory);
        return new QaBotResponse()
        {
            Text = TelegramMessages.REQUEST_CATEGORY_NAME,
            Keyboard = TelegramMarkups.GO_TO_MENU,
        };
    }
}