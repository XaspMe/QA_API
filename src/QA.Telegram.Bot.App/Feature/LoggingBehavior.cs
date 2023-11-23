using MediatR;

namespace QA.Telegram.Bot.App.Feature;

class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    // private readonly Logger _logger;

    public LoggingBehavior()
    {
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            // _logger.Log($"Before execution for {typeof(TRequest).Name}");

            var response = await next();
            return response;
        }
        finally
        {
            // _logger.Log($"After execution for {typeof(TRequest).Name}");
        }
    }
}