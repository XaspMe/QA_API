using System.Net;
using MediatR;
using QA.Common.Extensions;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.AcceptQuestionUpdate;
using QA.Telegram.Bot.App.Feature.EditQuestion;
using QA.Telegram.Bot.App.Feature.Menu;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace QA.Telegram.Bot.App.Feature.AcceptSearchKeyword;

public record AcceptSearchKeywordRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage, true);

public class AcceptSearchKeywordRequestHandler : IRequestHandler<AcceptSearchKeywordRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;
    private readonly IMediator _mediator;

    public AcceptSearchKeywordRequestHandler(IQaRepo repo, IMediator mediator)
    {
        _repo = repo;
        _mediator = mediator;
    }

    public async Task<QaBotResponse> Handle(AcceptSearchKeywordRequest request, CancellationToken cancellationToken)
    {
        await _repo.SetTelegramUserMode(request.UserMessage.Message.Chat.Id, UserInputMode.Normal);
        if (string.IsNullOrEmpty(request.UserMessage.Message.Text))
        {
            await request.UserMessage.BotClient.SendTextMessageAsync(
                request.UserMessage.User.TelegramChatId,
                TelegramMessages.CONTENT_INVALID,
                cancellationToken: cancellationToken);
            return await _mediator.Send(new MenuRequest(request.UserMessage), cancellationToken);
        }

        if (request.UserMessage.Message.Text is TelegramCommands.MENU)
        {
            return await _mediator.Send(new MenuRequest(request.UserMessage), cancellationToken);
        }

        var result = await _repo.SearchInQuestions(request.UserMessage.Message.Text);

        if (result.Any())
        {
            var textResult = 
                result.Select(x => $"/{x.Id}\nКатегория: {x.Category.Name}\n{x.Question}\n").ToList();

            return new QaBotResponse()
            {
                Text = WebUtility.HtmlEncode(string.Join("\n", textResult)),
                Keyboard = TelegramMarkups.MAIN_MENU(request.UserMessage.User.isAdmin),
            };
        }

        return new QaBotResponse()
        {
            Text = "Ничего не найдено, перефразируйте поиск",
            Keyboard = TelegramMarkups.MAIN_MENU(request.UserMessage.User.isAdmin),
        };
    }
}