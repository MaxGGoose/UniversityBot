namespace UniversityBot.Commands;

public static class DefaultCommand
{
    public static Task<Action> Command(ITelegramBotClient tgBotClient, Message message, CancellationToken cancellationToken)
    {
        async void Action() => 
            await tgBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id, 
                text: $"Я не знаю такой команды :(", 
                cancellationToken: cancellationToken);

        return Task.FromResult(Action);
    }
}