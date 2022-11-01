namespace UniversityBot.Commands;

internal interface ICommand
{
    string CommandText { get; }
    Task<Action> Command(ITelegramBotClient tgBotClient, Message message, CancellationToken cancellationToken, AnswerRequest answerRequest);
}