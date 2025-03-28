namespace EzCad.Api.Middleware;

public abstract class BaseMiddleware
{
    protected BaseMiddleware(RequestDelegate request)
    {
        Request = request;
    }

    protected RequestDelegate Request { get; }
}