namespace QA.Data.Exceptions;

public class EmptyDatabaseException : Exception
{
    public EmptyDatabaseException() : base("Data base is empty.")
    {
    }

    public EmptyDatabaseException(string message) : base(message)
    {
    }

    public EmptyDatabaseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}