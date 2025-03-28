namespace EzCad.Extensions.Discord.Exceptions;

public class DiscordException : Exception
{
    public DiscordException(string message) : base(message)
    {
    }
}