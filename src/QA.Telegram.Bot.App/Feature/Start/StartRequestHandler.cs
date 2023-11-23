using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.Start;

public record StartRequest(TelegramUserMessage UserMessage) : MediatR.IRequest<QaBotResponse>;

public class StartRequestHandler : IRequestHandler<StartRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public StartRequestHandler(IQaRepo qaRepo)
    {
        this._repo = qaRepo;
    }


    public async Task<QaBotResponse> Handle(StartRequest request, CancellationToken cancellationToken)
    {
        await this._repo.SetUserCurrentStep(request.UserMessage.Message.Chat.Id, UserCurrentStep.Menu);
        var categories = this._repo.GetAllCategories();
        return new QaBotResponse()
        {
            Text = TelegramMessages.HELLO(this._repo.ElementsCount()) + "\n" +
                   string.Join("\n", categories.Select(x => x.Name)),
            Keyboard = TelegramMarkups.MAIN_MENU(
                await this._repo.IsTelegramUserAdmin(request.UserMessage.Message.Chat.Id)),
        };
    }
}