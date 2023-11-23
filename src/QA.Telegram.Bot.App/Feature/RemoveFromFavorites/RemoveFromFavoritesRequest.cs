using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.Start;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.App.Feature.RemoveFromFavorites;

public record RemoveFromFavoritesRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class RemoveFromFavoritesRequestHandler : IRequestHandler<RemoveFromFavoritesRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public RemoveFromFavoritesRequestHandler(IQaRepo qaRepo)
    {
        this._repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(RemoveFromFavoritesRequest request, CancellationToken cancellationToken)
    {
        var curent = await this._repo.GetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id);
        await this._repo.RemoveFromTelegramUserFavoriteElements(request.UserMessage.Message.Chat.Id, curent);
        return new QaBotResponse()
        {
            Text = TelegramMessages.REMOVED_FROM_FAVORITES,
            Keyboard = TelegramMarkups.QUESTIONS_KEYBOARD(
                await this._repo.IsElementTelegramUserFavorite(request.UserMessage.Message.Chat.Id, curent),
                request.UserMessage.User.isAdmin),
        };
    }
}