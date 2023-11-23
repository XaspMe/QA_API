using System.Net;
using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.Start;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.App.Feature.NextQuestion;

public record NextQuestionRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class NextQuestionRequestHandler : IRequestHandler<NextQuestionRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public NextQuestionRequestHandler(IQaRepo qaRepo)
    {
        _repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(NextQuestionRequest request, CancellationToken cancellationToken)
    {
        await _repo.SetTelegramUserMode(request.UserMessage.User.TelegramChatId, UserInputMode.Normal);
        await _repo.SetUserCurrentStep(request.UserMessage.Message.Chat.Id, UserCurrentStep.Questions);
        var userChosenCategories = await _repo.GetTelegramUserCategories(request.UserMessage.Message.Chat.Id);
        QAElement question;

        var chosenCategories = userChosenCategories as QACategory[] ?? userChosenCategories.ToArray();
        if (chosenCategories.Count() == _repo.GetAllCategories().Count())
        {
            question = _repo.GetElementRandom();
        }

        // todo множественный выбор категорий
        else
        {
            try
            {
                question = _repo.GetElementRandomInCategory(chosenCategories.FirstOrDefault()!.Id);
            }
            catch (ArgumentOutOfRangeException e)
            {
                var categories = _repo.GetAllCategories();
                await _repo.SetTelegramUserMode(request.UserMessage.User.TelegramChatId, UserInputMode.SelectCategory);
                return new QaBotResponse()
                {
                    Text = TelegramMessages.CATEGORY_IS_EMPTY,
                    Keyboard = TelegramMarkups.CATEGORIES_WITH_MENU_AND_ALL_SELECTED(categories.Select(x => x.Name)),
                };
            }
        }

        await _repo.SetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id, question);
        Console.WriteLine($"текущий вопрос {question.Id}");

        return new QaBotResponse()
        {
            Text = WebUtility.HtmlEncode(
                $"Вопрос /{question.Id}\nКатегория: {question.Category.Name}\n{question.Question?.Replace("<br>", "\n") ?? string.Empty}"),
            Keyboard = TelegramMarkups.QUESTIONS_KEYBOARD(
                await _repo.IsElementTelegramUserFavorite(request.UserMessage.Message.Chat.Id, question),
                await _repo.IsTelegramUserAdmin(request.UserMessage.Message.Chat.Id)),
        };
    }
}