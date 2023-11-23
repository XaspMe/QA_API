using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.Start;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.Menu;

public record MenuRequest(TelegramUserMessage UserMessage) : MediatR.IRequest<QaBotResponse>;

public class MenuRequestHandler : IRequestHandler<MenuRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public MenuRequestHandler(IQaRepo qaRepo)
    {
        this._repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(MenuRequest request, CancellationToken cancellationToken)
    {
        await this._repo.SetUserCurrentStep(request.UserMessage.Message.Chat.Id, UserCurrentStep.Menu);
        await this._repo.SetTelegramUserMode(request.UserMessage.Message.Chat.Id, UserInputMode.Normal);
        var qaCount = this._repo.ElementsCount();
        return new QaBotResponse
        {
            Text = TelegramMessages.MAIN_MENU_WITH_COUNT(qaCount),
            Keyboard = TelegramMarkups.MAIN_MENU(request.UserMessage.User.isAdmin),
        };
    }
}