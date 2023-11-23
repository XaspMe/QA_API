using System.Net;
using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.Menu;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.App.Feature.Favorites;

public record FavoritesRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class FavoritesRequestHandler : IRequestHandler<FavoritesRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public FavoritesRequestHandler(IQaRepo qaRepo)
    {
        _repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(FavoritesRequest request, CancellationToken cancellationToken)
    {
        var question = await _repo.GetRandomElementFromTelegramUserFavorites(request.UserMessage.Message.Chat.Id);
        if (question != null)
        {
            await _repo.SetUserCurrentStep(request.UserMessage.Message.Chat.Id, UserCurrentStep.FavoriteQuestion);
            await _repo.SetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id, question);
            return new QaBotResponse
            {
                Text = WebUtility.HtmlEncode(
                    $"Вопрос /{question.Id}\nКатегория: {question.Category.Name}\n{question.Question?.Replace("<br>", "\n") ?? string.Empty}"),
                Keyboard = TelegramMarkups.FAVORITE_QUESTIONS_KEYBOARD(),
            };
        }

        await _repo.SetUserCurrentStep(request.UserMessage.Message.Chat.Id, UserCurrentStep.Menu);
        return new QaBotResponse
        {
            Text = TelegramMessages.NO_FAVORITES,
            Keyboard = TelegramMarkups.MAIN_MENU(request.UserMessage.User.isAdmin),
        };
    }
}