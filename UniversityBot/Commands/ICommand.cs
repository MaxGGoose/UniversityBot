using Telegram.Bot;
using Telegram.Bot.Types;

namespace UniversityBot.Commands;

internal interface ICommand
{
    Task<Action?> Command(ITelegramBotClient tgBotClient, Message message, CancellationToken cancellationToken);
}