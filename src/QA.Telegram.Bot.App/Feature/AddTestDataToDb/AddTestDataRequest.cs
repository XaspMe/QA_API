using MediatR;
using QA.Common.Services;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.AcceptNewQuestionCategory;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.AddTestDataToDb;

public record AddTestDataRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class AAddTestDataRequestHandler : IRequestHandler<AddTestDataRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public AAddTestDataRequestHandler(IQaRepo repo)
    {
        _repo = repo;
    }

    public async Task<QaBotResponse> Handle(AddTestDataRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await new TestDataAppender().Append(_repo);
            return new QaBotResponse()
            {
                Text = TelegramMessages.TEST_DATA_APPEND_SUCCESS,
                Keyboard = TelegramMarkups.MAIN_MENU(request.UserMessage.User.isAdmin),
            };
        }
        catch (Exception e)
        {
            return new QaBotResponse()
            {
                Text = TelegramMessages.ERROR,
                Keyboard = TelegramMarkups.MAIN_MENU(request.UserMessage.User.isAdmin),
            };
        }
    }
}