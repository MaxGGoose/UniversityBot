using PuppeteerSharp;
using ScheduleFilesDownloader;

namespace UniversityBot.Commands;

public class RenewCommand : ICommand
{
    public string CommandText => "/renew";

    public Task<Action> Command(ITelegramBotClient tgBotClient, Message message, CancellationToken cancellationToken, AnswerRequest answerRequest)
    {
        async void Action()
        {
            string answer;
            if (message.Chat.Username == "max_gus")
            {
                var success = await ScheduleFilesRenewer.Renew();
            
                answer = success 
                    ? "File successfully renewed"
                    : "File doesn't need a renew";
            }
            else
            {
                answer = "Недостаточно прав для использования этой команды";
            }

            await tgBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: answer,
                cancellationToken: cancellationToken);
        }

        return Task.FromResult(Action);
    }
}