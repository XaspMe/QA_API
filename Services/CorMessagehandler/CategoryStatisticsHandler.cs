using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QA_API.Constants;
using QA_API.Data;
using QA_API.Services.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA_API.CorMessagehandler;

public class CategoryStatisticsHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly IQaRepo _repo;

    public CategoryStatisticsHandler(ITelegramBotClient telegramBotClient, CancellationToken ct, IQaRepo repo)
    {
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _repo = repo;
    }

    public override async Task HandleMessage(Message message)
    {
        if (message.Text!.Contains(TelegramCommands.SHOW_CATEGORIES_STATISTICS))
        {
            var categories = _repo.GetAllCategories();

            var responceMessage = new StringBuilder();
            responceMessage.AppendLine("Статистика по вашим категориям");
            foreach (var stat in _repo.CategoriesStats())
            {
                responceMessage.AppendLine(stat);
            }

            // todo добавить категории
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: responceMessage.ToString(),
                replyMarkup: TelegramMarkups.MAIN_MENU(false),
                cancellationToken: _ct);
        }
        else if (_nextHandler != null)
        {
            await _nextHandler.HandleMessage(message);
        }
        else
        {
            // todo reply markup  
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.HANDLE_ERROR,
                cancellationToken: _ct);
        }
    }
}