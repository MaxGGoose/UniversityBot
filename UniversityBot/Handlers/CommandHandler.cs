using UniversityBot.Commands;

namespace UniversityBot.Handlers;

public class CommandHandler
{
    private readonly Dictionary<string, ICommand> _commands;
    private readonly AnswerRequest _answerRequest;

    private CancellationToken _cancellationToken;
    private ITelegramBotClient _tgBotClient = null!;

    public CommandHandler()
    {
        _commands = new Dictionary<string, ICommand>
        {
            {"/start", new StartCommand()},
            {"/getschedule", new GetScheduleCommand()},
            {"/renew", new RenewCommand()},
        };
        _answerRequest = new AnswerRequest
        {
            StageOfDialog = 0,
            RequestCommand = string.Empty
        };
    }

    public void SetCommandHandler(ITelegramBotClient tgBotClient, CancellationToken cancellationToken)
    {
        _tgBotClient = tgBotClient;
        _cancellationToken = cancellationToken;
        
        _tgBotClient.SetMyCommandsAsync(
            commands: _commands.Values
                .Where(command => command.IsFreeForUsers)
                .Select(command => command.CommandProperties),
            cancellationToken: cancellationToken);
    }

    public Action GetCommand(Message message)
    {
        return _answerRequest.StageOfDialog != 0 && _answerRequest.RequestCommand is not null && _commands.TryGetValue(_answerRequest.RequestCommand!, out var command)
            ? command.Command(_tgBotClient, message, _cancellationToken, _answerRequest).Result
            : message.Text is not null && _commands.TryGetValue(message.Text, out command)
                ? command.Command(_tgBotClient, message, _cancellationToken, _answerRequest).Result
                : DefaultCommand.Command(_tgBotClient, message, _cancellationToken).Result;
    }
}