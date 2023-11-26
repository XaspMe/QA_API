using MediatR;
using QA.Data;
using QA.Telegram.Bot.App.Feature.Return;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot;

namespace QA.Telegram.Bot.App.Feature.RemoveFromFavorites;

public record RemoveFromFavoritesRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage);

public class RemoveFromFavoritesRequestHandler : IRequestHandler<RemoveFromFavoritesRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;
    private readonly IMediator _mediator;

    public RemoveFromFavoritesRequestHandler(IQaRepo qaRepo, IMediator mediator)
    {
        _repo = qaRepo;
        _mediator = mediator;
    }

    public async Task<QaBotResponse> Handle(RemoveFromFavoritesRequest request, CancellationToken cancellationToken)
    {
        var curent = await _repo.GetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id);
        await _repo.RemoveFromTelegramUserFavoriteElements(request.UserMessage.Message.Chat.Id, curent);
        await request.UserMessage.BotClient.SendTextMessageAsync(
            request.UserMessage.Message.Chat.Id,
            TelegramMessages.REMOVED_FROM_FAVORITES,
            cancellationToken: cancellationToken);
        return await _mediator.Send(new ReturnRequest(request.UserMessage), cancellationToken);
    }
}