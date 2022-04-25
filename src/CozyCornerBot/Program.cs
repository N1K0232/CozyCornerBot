using CozyCornerBot.Core;

var accessToken = Environment.GetEnvironmentVariable("BotToken");
var bot = new DiscordBot(accessToken);
await bot.RunAsync();