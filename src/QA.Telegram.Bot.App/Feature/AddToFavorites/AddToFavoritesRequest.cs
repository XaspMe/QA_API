using MediatR;
using QA.Data;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.AddToFavorites;

public record AddToFavoritesRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class AddToFavoritesRequestHandler : IRequestHandler<AddToFavoritesRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public AddToFavoritesRequestHandler(IQaRepo qaRepo, IMediator mediator)
    {
        this._repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(AddToFavoritesRequest request, CancellationToken cancellationToken)
    {
        var question = await this._repo.GetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id);
        await this._repo.AddToTelegramUserFavoriteElements(request.UserMessage.Message.Chat.Id, question);

        return new QaBotResponse
        {
            Text = TelegramMessages.ADDED_TO_FAVORITES,
            Keyboard = TelegramMarkups.QUESTIONS_KEYBOARD(
                await this._repo.IsElementTelegramUserFavorite(request.UserMessage.Message.Chat.Id, question),
                await this._repo.IsTelegramUserAdmin(request.UserMessage.Message.Chat.Id)),
        };
    }
}