using System.Net;
using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.Menu;
using QA.Telegram.Bot.App.Feature.NextQuestion;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.CorMessagehandler.ConcreteHandlers.NormalMode;
using QA.Telegram.Bot.Models;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.App.Feature.CategorySelected;

public record CategorySelectedRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class CategorySelectedRequestHandler : IRequestHandler<CategorySelectedRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;
    private readonly IMediator _mediator;

    public CategorySelectedRequestHandler(IQaRepo qaRepo, IMediator mediator)
    {
        this._repo = qaRepo;
        _mediator = mediator;
    }

    public async Task<QaBotResponse> Handle(CategorySelectedRequest request, CancellationToken cancellationToken)
    {

        var categories = this._repo.GetAllCategories().ToList();
        var categoriesNames = categories.Select(x => x.Name).ToList();

        if (categoriesNames.Any(x => request.UserMessage.Message.Text!.Contains(x)) ||
            request.UserMessage.Message.Text!.Contains(TelegramCommands.ALL_CATEGORIES))
        {
            if (request.UserMessage.Message.Text!.Contains(TelegramCommands.ALL_CATEGORIES))
            {
                // todo multiselect
                await this._repo.UpdateTelegramUserFavoriteCategories(
                    request.UserMessage.Message.Chat.Id,
                    new List<QACategory>());
            }
            else
            {
                var userDbCats =
                    categories.FirstOrDefault(x => x.Name == request.UserMessage.Message.Text);

                await this._repo.UpdateTelegramUserFavoriteCategories(
                    request.UserMessage.Message.Chat.Id,
                    new List<QACategory>()
                    {
                        userDbCats
                    });
            }

            await this._repo.SetTelegramUserMode(request.UserMessage.Message.Chat.Id, UserInputMode.Normal);
            return await this._mediator.Send(new NextQuestionRequest(request.UserMessage), cancellationToken);
        }

        await this._repo.SetTelegramUserMode(request.UserMessage.Message.Chat.Id, UserInputMode.Normal);
        return await this._mediator.Send(new MenuRequest(request.UserMessage), cancellationToken);
    }
}