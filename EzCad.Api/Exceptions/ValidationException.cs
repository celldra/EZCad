namespace EzCad.Api.Exceptions;

public class ValidationException : Exception
{
    public readonly IDictionary<string, string[]> Errors;

    public ValidationException(IDictionary<string, string[]> errors)
    {
        Errors = errors;
    }
}