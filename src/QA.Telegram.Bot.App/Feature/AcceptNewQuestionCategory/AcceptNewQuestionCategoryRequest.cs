using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.AcceptNewQuestionCategory;

public record AcceptNewQuestionCategoryRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class AcceptNewQuestionCategoryRequesttHandler : IRequestHandler<AcceptNewQuestionCategoryRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public AcceptNewQuestionCategoryRequesttHandler(IQaRepo repo)
    {
        _repo = repo;
    }

    public async Task<QaBotResponse> Handle(AcceptNewQuestionCategoryRequest request, CancellationToken cancellationToken)
    {
        var categories = _repo.GetAllCategories().ToList();
        var categoriesNames = categories.Select(x => x.Name).ToList();
        await _repo.SetTelegramUserMode(request.UserMessage.Message.Chat.Id, UserInputMode.Normal);

        if (categoriesNames.Any(x => request.UserMessage.Message.Text!.Contains(x)))
        {
            var cat = categories.FirstOrDefault(x => x.Name == request.UserMessage.Message.Text);
            QAElement elementOnCurrentTelegramUser = await _repo.GetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id);
            await _repo.ChangeQuestionCategory(elementOnCurrentTelegramUser,
                cat!);
            // todo return to prev mode instead of menu
            return new QaBotResponse()
            {
                Text = TelegramMessages.QA_ELEMENT_MOVED_TO_NEW_CATEGORY +
                       $"\nКатегория:\n{elementOnCurrentTelegramUser.Category.Name}\nВопрос:\n{elementOnCurrentTelegramUser.Question}\nОтвет:\n{elementOnCurrentTelegramUser.Answer}",
                Keyboard = TelegramMarkups.MAIN_MENU(request.UserMessage.User.isAdmin),
            };
        }
        else
        {
            return new QaBotResponse()
            {
                Text = TelegramMessages.CATEGORY_INVALID,
                Keyboard = TelegramMarkups.MAIN_MENU(request.UserMessage.User.isAdmin),
            };
        }
    }
}