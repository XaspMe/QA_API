namespace QA.Data.Exceptions;

public class EmptyDatabaseException : Exception
{
    public EmptyDatabaseException() : base("База данных пустая.")
    {
    }

    public EmptyDatabaseException(string message) : base(message)
    {
    }

    public EmptyDatabaseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}