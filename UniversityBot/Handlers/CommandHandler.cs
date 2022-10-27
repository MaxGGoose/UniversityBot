using UniversityBot.Commands;

namespace UniversityBot.Handlers;

public class CommandHandler
{
    private readonly ITelegramBotClient _tgBotClient;
    private readonly Dictionary<string, ICommand> _commands;
    private readonly CancellationToken _cancellationToken;
    
    private Message? _currentMessage;

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
        return message?.Text is not null && _commands.TryGetValue(message.Text, out var command)
            ? command.Command(_tgBotClient, message, _cancellationToken).Result
            : DefaultCommand.Command(_tgBotClient, message, _cancellationToken).Result;
    }
}