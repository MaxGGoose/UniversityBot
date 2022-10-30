using Telegram.Bot.Types.InputFiles;
using File = System.IO.File;

namespace UniversityBot.Commands;

public class GetScheduleCommand : ICommand
{
    public Task<Action> Command(ITelegramBotClient tgBotClient, Message message, CancellationToken cancellationToken, AnswerRequest answerRequest)
    {
        async void Action()
        {
            if (!answerRequest.IsRequested)
            {
                await tgBotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Введите номер вашей группы\n\n" +
                          "Например: МПМ-121, ДГ-122в, КДК-11-721\n\n" +
                          "Пока что бот не очень умный, поэтому он не поймет, если вы напишете мпм121, ДГ-122В, или кдк11-721",
                    cancellationToken: cancellationToken);
                answerRequest.IsRequested = true;
                answerRequest.RequestCommand = message.Text;
            }
            else if (answerRequest.IsRequested)
            {
                var filePath = Directory.GetCurrentDirectory()
                               + $"\\{Environment.GetEnvironmentVariable("DIVIDED_SCHEDULE_DIRECTORY")}"
                               + $"\\{message.Text}.xlsx";

                if (File.Exists(filePath))
                {
                    await using Stream stream = File.OpenRead(filePath);
                    await tgBotClient.SendDocumentAsync(
                        chatId: message.Chat.Id,
                        document: new InputOnlineFile(stream, "schedule.xlsx"),
                        caption: "Ваше расписание.\n\n" +
                                 "Если это не ваше расписание, проверьте что вы указали группу верно и попробуйте ещё раз.\n" +
                                 "Если бот всё ещё присылает вам не ваше расписание, напишите @max_gus",
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await tgBotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Файл с расписанием этой группы не найден\n\n" +
                              "Если вы уверены, что вы ввели группу правильно, " +
                              "напишите @max_gus, чтобы я смог исправить ошибку.",
                        cancellationToken: cancellationToken);
                }
                
                answerRequest.IsRequested = false;
                answerRequest.RequestCommand = string.Empty;    
            }
        }
        return Task.FromResult(Action);
    }
}