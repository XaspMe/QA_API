using System.Net;
using MediatR;
using QA.Data;
using QA.Models.Models;
using QA.Telegram.Bot.App.Feature.Start;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;
using Telegram.Bot.Types.Enums;

namespace QA.Telegram.Bot.App.Feature.NextQuestion;

public record NextQuestionRequest(TelegramUserMessage UserMessage) : MediatR.IRequest<QaBotResponse>;

public class NextQuestionRequestHandler : IRequestHandler<NextQuestionRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public NextQuestionRequestHandler(IQaRepo qaRepo)
    {
        this._repo = qaRepo;
    }

    public async Task<QaBotResponse> Handle(NextQuestionRequest request, CancellationToken cancellationToken)
    {
        await this._repo.SetUserCurrentStep(request.UserMessage.Message.Chat.Id, UserCurrentStep.Questions);
        var userChosenCategories = await this._repo.GetTelegramUserCategories(request.UserMessage.Message.Chat.Id);
        QAElement question;

        var chosenCategories = userChosenCategories as QACategory[] ?? userChosenCategories.ToArray();
        if (chosenCategories.Count() == this._repo.GetAllCategories().Count())
        {
            question = this._repo.GetElementRandom();
        }

        // todo множественный выбор категорий
        else
        {
            question = this._repo.GetElementRandomInCategory(chosenCategories.FirstOrDefault()!.Id);
        }

        await this._repo.SetElementOnCurrentTelegramUser(request.UserMessage.Message.Chat.Id, question);
        Console.WriteLine($"текущий вопрос {question.Id}");

        return new QaBotResponse()
        {
            Text = WebUtility.HtmlEncode(
                $"Вопрос /{question.Id}\nКатегория: {question.Category.Name}\n{question.Question?.Replace("<br>", "\n") ?? string.Empty}"),
            Keyboard = TelegramMarkups.QUESTIONS_KEYBOARD(
                await this._repo.IsElementTelegramUserFavorite(request.UserMessage.Message.Chat.Id, question),
                await this._repo.IsTelegramUserAdmin(request.UserMessage.Message.Chat.Id)),
        };
    }
}