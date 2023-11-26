using MediatR;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Common.Exception;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature;

public class UserAdminAccessBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is TelegramUserRequest telegramUserRequest)
        {
            if (telegramUserRequest is { RequireAdminAccess: true, UserMessage.User.isAdmin: false })
            {
                throw new AccessDeniedException();
            }
        }

        var response = await next();
        return response;
    }
}