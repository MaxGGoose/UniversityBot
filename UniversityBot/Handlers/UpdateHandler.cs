using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace UniversityBot.Handlers;

public class UpdateHandler : IUpdateHandler
{
    private static readonly CommandHandler CommandHandler = new();

    public Task HandleUpdateAsync(ITelegramBotClient tgBotClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message) return Task.CompletedTask;
        if (update.Message?.Text is null) return Task.CompletedTask;
        if (update.Message!.Type != MessageType.Text) return Task.CompletedTask;
        
        var message = update.Message;

        Console.WriteLine($"Received a '{message.Text}' in chat with @{message.From!.Username}");

        CommandHandler.SetCommandHandler(tgBotClient, cancellationToken);

        var command = CommandHandler.GetCommand(message);
        command?.Invoke();
        
        return Task.CompletedTask;
    }
    
    public Task HandlePollingErrorAsync(ITelegramBotClient tgBotClient, Exception exception, CancellationToken cancellationToken)
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