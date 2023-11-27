using System.Threading.Channels;
using MediatR;
using QA.Telegram.Bot.Models;
using Telegram.Bot.Types;

namespace QA.Telegram.Bot.App.Feature;

class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    // private readonly Logger _logger;

    public LoggingBehavior()
    {
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        int messageMessageId = 0;
        if (request is TelegramUserRequest telegramUserRequest)
        {
            Console.WriteLine("======================");
            messageMessageId = telegramUserRequest.UserMessage.Message.MessageId;
            Console.WriteLine($"Request message #{messageMessageId}");
            Console.WriteLine($"ChatId {telegramUserRequest.UserMessage.User.TelegramChatId}");
            Console.WriteLine($"From {telegramUserRequest.UserMessage.Message.From?.Username}");
            if (telegramUserRequest.UserMessage.User.CurrentQuestion != null)
            {
                Console.WriteLine($"CurrentQuestion {telegramUserRequest.UserMessage.User.CurrentQuestion.Id}");
            }

            Console.WriteLine($"From {telegramUserRequest.UserMessage.Message.From?.FirstName}");
            Console.WriteLine($"From {telegramUserRequest.UserMessage.Message.From?.LastName}");
            Console.WriteLine($"Chat.Id {telegramUserRequest.UserMessage.Message.Chat.Id}");
            Console.WriteLine($"IsAdmin {telegramUserRequest.UserMessage.User.isAdmin}");
            Console.WriteLine($"Step {telegramUserRequest.UserMessage.User.UserCurrentStep}");
            Console.WriteLine($"Mode {telegramUserRequest.UserMessage.User.UserInputMode}");
            Console.WriteLine($"Message {telegramUserRequest.UserMessage.Message.Text}");
            Console.WriteLine("======================");
        }

        try
        {
            var response = await next();
            if (response is QaBotResponse botResponse)
            {
                Console.WriteLine("======================");
                Console.WriteLine($"Request message #{messageMessageId}");
                Console.WriteLine($"Text {botResponse.Text}");
                Console.WriteLine("======================");

            }

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine("======================");
            Console.WriteLine($"Request message #{messageMessageId}");
            Console.WriteLine($"Exception {ex.Message}");
            Console.WriteLine($"StackTrace {ex.StackTrace}");
            Console.WriteLine("======================");
            throw;
        }
    }
}