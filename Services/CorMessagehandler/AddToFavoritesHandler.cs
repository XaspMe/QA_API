using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QA_API.Constants;
using QA_API.Services.CorMessagehandler.@abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QA_API.CorMessagehandler;

public class AddToFavoritesHandler : MessageHandler
{
    private readonly Dictionary<long, int> _userCurrentQuestion;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly CancellationToken _ct;
    private readonly Dictionary<long, int> _userFavorites;

    public AddToFavoritesHandler(Dictionary<long, int> userCurrentQuestion,
        ITelegramBotClient telegramBotClient, CancellationToken ct, Dictionary<long, int> userFavorites)
    {
        _userCurrentQuestion = userCurrentQuestion;
        _telegramBotClient = telegramBotClient;
        _ct = ct;
        _userFavorites = userFavorites;
    }
    
    public override async Task HandleMessage(Message message)
    {
        if (message.Text!.Contains(TelegramCommands.ADD_TO_FAVORITES))
        {
            if (_userCurrentQuestion.ContainsKey(message.Chat.Id) && !_userFavorites.ContainsValue(_userCurrentQuestion[message.Chat.Id]))
            {
                _userFavorites[message.Chat.Id] = _userCurrentQuestion[message.Chat.Id];
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: TelegramMessages.ADDED_TO_FAVORITES,
                    cancellationToken: _ct);
            }
            else
            {
                // todo сделать что-то если вопроса нет в активных и если вопрос уже в избранных
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: TelegramMessages.HANDLE_ERROR,
                    cancellationToken: _ct);
            }
        }
        else if (_nextHandler != null)
        {
            await _nextHandler.HandleMessage(message);
        }
        else
        {
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.HANDLE_ERROR,
                cancellationToken: _ct);
        }
    }
}