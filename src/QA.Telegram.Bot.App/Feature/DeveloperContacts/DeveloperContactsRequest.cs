using MediatR;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.DeveloperContacts;

public record DeveloperContactsRequest(TelegramUserMessage UserMessage) : MediatR.IRequest<QaBotResponse>;

public class DeveloperContactsRequestHandler : IRequestHandler<DeveloperContactsRequest, QaBotResponse>
{
    public async Task<QaBotResponse> Handle(DeveloperContactsRequest request, CancellationToken cancellationToken)
    {
        return new QaBotResponse
        {
            Text = TelegramMessages.DEVELOPER_CONTACT,
        };
    }
}