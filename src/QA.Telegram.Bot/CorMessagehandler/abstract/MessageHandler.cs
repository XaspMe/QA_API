using Telegram.Bot.Types;

namespace QA.Telegram.Bot.CorMessagehandler.@abstract;

public abstract class MessageHandler
{
    protected MessageHandler _nextHandler;

    public void SetNextHandler(MessageHandler nextHandler)
    {
        _nextHandler = nextHandler;
    }

    public abstract Task HandleMessage(Message message);
}