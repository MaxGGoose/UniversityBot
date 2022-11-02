namespace UniversityBot.Commands;

public class StartCommand : ICommand
{
    public bool IsFreeForUsers => true;

    public BotCommand CommandProperties => new()
    {
        Command = "/start",
        Description = "Получить список команд"
    };

    public Task<Action> Command(ITelegramBotClient tgBotClient, Message message, CancellationToken cancellationToken,
        AnswerRequest answerRequest)
    {
        
        async void Action()
        {
            switch (answerRequest.StageOfDialog)
            {
                case 0:
                    var commands = tgBotClient
                        .GetMyCommandsAsync(cancellationToken: cancellationToken).Result
                        .Aggregate(
                            string.Empty, 
                            (current, command) => 
                                current + ("/" + command.Command + " - " + command.Description + "\n"));

                    await tgBotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "🤖 Список доступных команд:\n" +
                              $"{commands}",
                        cancellationToken: cancellationToken);
                    break;
            }
        }

        return Task.FromResult(Action);
    }
}