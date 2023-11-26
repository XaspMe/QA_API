using MediatR;
using QA.Data;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.AddToFavorites;

public record AddToFavoritesRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage);

public class AddToFavoritesRequestHandler : IRequestHandler<AddToFavoritesRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public AddToFavoritesRequestHandler(IQaRepo qaRepo, IMediator mediator)
    {
        _repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(AddToFavoritesRequest request, CancellationToken cancellationToken)
    {
        var question = await _repo.GetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id);
        await _repo.AddToTelegramUserFavoriteElements(request.UserMessage.Message.Chat.Id, question);

        return new QaBotResponse
        {
            Text = TelegramMessages.ADDED_TO_FAVORITES,
            Keyboard = TelegramMarkups.QUESTIONS_KEYBOARD(
                await _repo.IsElementTelegramUserFavorite(request.UserMessage.Message.Chat.Id, question),
                await _repo.IsTelegramUserAdmin(request.UserMessage.Message.Chat.Id)),
        };
    }
}