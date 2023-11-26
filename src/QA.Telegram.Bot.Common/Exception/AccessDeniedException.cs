namespace QA.Telegram.Bot.Common.Exception;

public class AccessDeniedException : System.Exception
{
public AccessDeniedException() : base("Access denied for current user.")
{
}

public AccessDeniedException(string message) : base(message)
{
}

public AccessDeniedException(string message, System.Exception innerException) : base(message, innerException)
{
}
}