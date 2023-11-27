using MediatR;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.ManageApp;

public record ManageAppRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage, true);

public class ManageAppRequestHandler : IRequestHandler<ManageAppRequest, QaBotResponse>
{
    public Task<QaBotResponse> Handle(ManageAppRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult<QaBotResponse>(
            new QaBotResponse()
            {
                Text = TelegramMessages.SELECTOR,
                Keyboard = TelegramMarkups.MANAGE_MENU,
            });
    }
}