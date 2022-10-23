using Telegram.Bot;
using Telegram.Bot.Types;

namespace UniversityBot.Commands;

public class DebugCommand : ICommand
{
    public Task<Action?> Command(ITelegramBotClient tgBotClient, Message message, CancellationToken cancellationToken)
    {
        async void Action() => 
            await tgBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id, 
                text: $"Debug message for '{tgBotClient.BotId}'", 
                cancellationToken: cancellationToken);

        return Task.FromResult(Action);
    }
}