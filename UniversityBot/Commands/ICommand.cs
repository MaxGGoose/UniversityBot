namespace UniversityBot.Commands;

internal interface ICommand
{
    bool IsFreeForUsers { get; }
    BotCommand CommandProperties { get; }
    Task<Action> Command(ITelegramBotClient tgBotClient, Message message, CancellationToken cancellationToken, AnswerRequest answerRequest);
}