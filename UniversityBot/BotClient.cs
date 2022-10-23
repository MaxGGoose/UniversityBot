using Telegram.Bot;
using Telegram.Bot.Types;
using UniversityBot.TelegramHandlers;

namespace UniversityBot;

public class BotClient
{
    private readonly TelegramBotClient _tgBotClient;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public required string TgBotToken { private get; init; }
    
    public BotClient()
    {
        _tgBotClient = new TelegramBotClient(TgBotToken!);
        _cancellationTokenSource = new CancellationTokenSource();
    }
    
    public async Task<User> StartBot()
    {
        _tgBotClient.StartReceiving<UpdateHandler>(
            cancellationToken: _cancellationTokenSource.Token);

        return await _tgBotClient.GetMeAsync();
    }

    public void StopBot() => _cancellationTokenSource.Cancel();
}