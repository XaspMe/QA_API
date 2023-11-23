using System.Net;
using System.Text;
using MediatR;
using QA.Data;
using QA.Telegram.Bot.Common.Constants;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature.CategoryStatistics;

public record CategoryStatisticsRequest(TelegramUserMessage UserMessage) : IRequest<QaBotResponse>;

public class CategoryStatisticsRequestHandler : IRequestHandler<CategoryStatisticsRequest, QaBotResponse>
{
    private readonly IQaRepo _repo;

    public CategoryStatisticsRequestHandler(IQaRepo repo)
    {
        _repo = repo;
    }

    public async Task<QaBotResponse> Handle(CategoryStatisticsRequest request, CancellationToken cancellationToken)
    {
        var categories = _repo.GetAllCategories();

        var responseMessage = new StringBuilder();
        responseMessage.AppendLine("Статистика по вашим категориям");
        foreach (var stat in _repo.CategoriesStats())
        {
            responseMessage.AppendLine(WebUtility.HtmlEncode(stat));
        }

        return new QaBotResponse()
        {
            Text = responseMessage.ToString(),
        };
    }
}