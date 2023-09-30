using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace QA_API.Services.CorMessagehandler.@abstract;

public abstract class MessageHandler
{
    protected MessageHandler _nextHandler;

    public void SetNextHandler(MessageHandler nextHandler)
    {
        _nextHandler = nextHandler;
    }

    public abstract Task HandleMessage(Message message);
}