using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace UniversityBot.TelegramHandlers;

public sealed class UpdateHandler
{
    public Task HandleUpdateAsync(ITelegramBotClient tgBotClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message) return Task.CompletedTask;
        if (update.Message!.Type != MessageType.Text) return Task.CompletedTask;
        
        var message = update.Message;

        Console.WriteLine($"Received a '{message.Text}' in chat with @{message.From!.Username}");
        
        //TODO: Create commands itself

        return Task.CompletedTask;

    }
}