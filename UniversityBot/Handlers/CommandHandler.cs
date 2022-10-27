using UniversityBot.Commands;

namespace UniversityBot.Handlers;

public class CommandHandler
{
    private readonly ITelegramBotClient _tgBotClient;
    private readonly Dictionary<string, ICommand> _commands;
    private readonly CancellationToken _cancellationToken;
    
    private static IsAnswerRequested _isAnswerRequsted = new() { IsRequested = false, RequestCommand = string.Empty };

    public CommandHandler(ITelegramBotClient tgBotClient, CancellationToken cancellationToken)
    {
        _tgBotClient = tgBotClient;
        _cancellationToken = cancellationToken;
        _commands = new Dictionary<string, ICommand>
        {
            {"/debugcommand", new DebugCommand()}
        };
    }

    public Action? GetCommand(Message? message)
    {
        
        if (!_isAnswerRequsted.IsRequested)
            return message?.Text is not null && _commands.TryGetValue(message.Text, out var command)
                ? command.Command(_tgBotClient, message, _cancellationToken, _isAnswerRequsted).Result
                : DefaultCommand.Command(_tgBotClient, message, _cancellationToken).Result;
        return _commands[_isAnswerRequsted.RequestCommand].Command(_tgBotClient, message, _cancellationToken, _isAnswerRequsted).Result;
    }
}