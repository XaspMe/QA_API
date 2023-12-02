using MediatR;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.App.Feature.About;

public record AboutRequest(TelegramUserMessage UserMessage) : TelegramUserRequest(UserMessage);

public class AboutRequestHandler : IRequestHandler<AboutRequest, QaBotResponse>
{
    public Task<QaBotResponse> Handle(AboutRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(
            new QaBotResponse
            {
                Text = TelegramMessages.ABOUT,
                ParseMode = ParseMode.Markdown,
            });
    }
}