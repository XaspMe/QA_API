using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.Start;

public record StartRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class StartRequestHandler : IRequestHandler<StartRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public StartRequestHandler(IQaRepo qaRepo)
    {
        _repo = qaRepo;
    }


    public async Task<QaBotResponse> Handle(StartRequest request, CancellationToken cancellationToken)
    {
        await _repo.SetUserCurrentStep(request.UserMessage.Message.Chat.Id, UserCurrentStep.Menu);
        var categories = _repo.GetAllCategories();
        return new QaBotResponse()
        {
            Text = TelegramMessages.HELLO(_repo.ElementsCount()) + "\n" +
                   string.Join("\n", categories.Select(x => x.Name)),
            Keyboard = TelegramMarkups.MAIN_MENU(
                await _repo.IsTelegramUserAdmin(request.UserMessage.Message.Chat.Id)),
        };
    }
}