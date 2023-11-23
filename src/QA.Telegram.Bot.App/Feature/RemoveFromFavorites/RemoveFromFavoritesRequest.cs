using MediatR;
using QA.Data;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.RemoveFromFavorites;

public record RemoveFromFavoritesRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class RemoveFromFavoritesRequestHandler : IRequestHandler<RemoveFromFavoritesRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public RemoveFromFavoritesRequestHandler(IQaRepo qaRepo)
    {
        _repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(RemoveFromFavoritesRequest request, CancellationToken cancellationToken)
    {
        var curent = await _repo.GetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id);
        await _repo.RemoveFromTelegramUserFavoriteElements(request.UserMessage.Message.Chat.Id, curent);
        return new QaBotResponse()
        {
            Text = TelegramMessages.REMOVED_FROM_FAVORITES,
            Keyboard = TelegramMarkups.QUESTIONS_KEYBOARD(
                await _repo.IsElementTelegramUserFavorite(request.UserMessage.Message.Chat.Id, curent),
                request.UserMessage.User.isAdmin),
        };
    }
}