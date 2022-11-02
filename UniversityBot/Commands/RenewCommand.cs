using PuppeteerSharp;
using ScheduleFilesDownloader;

namespace UniversityBot.Commands;

public class RenewCommand : ICommand
{
    public bool IsFreeForUsers => false;

    public BotCommand CommandProperties => new()
    {
        Command = "/renew",
        Description = "Обновить файлы с расписанием"
    };

    public Task<Action> Command(ITelegramBotClient tgBotClient, Message message, CancellationToken cancellationToken, AnswerRequest answerRequest)
    {
        async void Action()
        {
            switch (answerRequest.StageOfDialog)
            {
                case 0:
                    string answer;
                    if (message.Chat.Username == "max_gus")
                        answer = await ScheduleFilesRenewer.Renew()
                            ? "File successfully renewed"
                            : "File doesn't need a renew";
                    else
                        answer = "Недостаточно прав для использования этой команды";

                    await tgBotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: answer,
                        cancellationToken: cancellationToken);        
                    break;
            }
        }
        return Task.FromResult(Action);
    }
}