using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.AcceptNewCategory;

public record AcceptNewCategoryRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class AcceptNewCategoryRequestHandler : IRequestHandler<AcceptNewCategoryRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public AcceptNewCategoryRequestHandler(IQaRepo qaRepo)
    {
        _repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(AcceptNewCategoryRequest request, CancellationToken cancellationToken)
    {
        await _repo.SetTelegramUserMode(request.UserMessage.Message.Chat.Id, UserInputMode.Normal);

        if (request.UserMessage.Message.Text != string.Empty &&
            request.UserMessage.Message.Text != TelegramCommands.MENU)
        {
            await _repo.CreateTelegramUserQaCategory(
                request.UserMessage.Message.Chat.Id,
                new QACategory()
                {
                    Name = request.UserMessage.Message.Text!,
                });

            return new QaBotResponse
            {
                Text = TelegramMessages.CATEGORY_CREATED,
                Keyboard = TelegramMarkups.MAIN_MENU(request.UserMessage.User.isAdmin),
            };
        }

        return new QaBotResponse
        {
            Text = TelegramMessages.MAIN_MENU_WITH_COUNT(_repo.ElementsCount()),
            Keyboard = TelegramMarkups.MAIN_MENU(request.UserMessage.User.isAdmin),
        };
    }
}