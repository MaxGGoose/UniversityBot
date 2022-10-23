using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace UniversityBot.TelegramHandlers;

public sealed class ErrorHandler
{
    public Task HandleErrorAsync(ITelegramBotClient tgBotClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]" +
                   $"\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        
        return Task.CompletedTask;
    }
}