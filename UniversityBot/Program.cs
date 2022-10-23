using UniversityBot;

var tgBotClient = new BotClient(Environment.GetEnvironmentVariable("BOT_TOKEN")!);

var user = tgBotClient.StartBot().Result;

Console.WriteLine($"Start listening for @{user.Username}");
Console.ReadKey();

tgBotClient.StopBot();