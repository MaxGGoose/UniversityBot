using UniversityBot.Commands;

namespace UniversityBot.Handlers;

public class CommandHandler
{
    private readonly Dictionary<string, ICommand> _commands;

    private CancellationToken _cancellationToken;
    private ITelegramBotClient _tgBotClient;
    private AnswerRequest _answerRequest;

    public CommandHandler()
    {
        _commands = new Dictionary<string, ICommand>
        {
            {"/getschedule", new GetScheduleCommand()},
            {"/renew", new RenewCommand()},
        };
        _answerRequest = new AnswerRequest
        {
            IsRequested = false,
            RequestCommand = string.Empty
        };
    }

    public void SetCommandHandler(ITelegramBotClient tgBotClient, CancellationToken cancellationToken)
    {
        _tgBotClient = tgBotClient;
        _cancellationToken = cancellationToken;
    }

    public Action GetCommand(Message message)
    {
        return _answerRequest.IsRequested && _answerRequest.RequestCommand is not null && _commands.TryGetValue(_answerRequest.RequestCommand!, out var command)
            ? command.Command(_tgBotClient, message, _cancellationToken, _answerRequest).Result
            : message.Text is not null && _commands.TryGetValue(message.Text, out command)
                ? command.Command(_tgBotClient, message, _cancellationToken, _answerRequest).Result
                : DefaultCommand.Command(_tgBotClient, message, _cancellationToken).Result;
    }
}