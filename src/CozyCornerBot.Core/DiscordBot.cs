using CozyCornerBot.Core.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace CozyCornerBot.Core;

public class DiscordBot
{
    private DiscordSocketClient socketClient;
    private CommandService commandService;
    private IServiceProvider serviceProvider;

    private readonly string accessToken;

    public DiscordBot(string accessToken)
    {
        this.accessToken = accessToken;
    }

    public async Task RunAsync()
    {
        socketClient = new DiscordSocketClient();
        commandService = new CommandService();
        var services = new ServiceCollection()
            .AddSingleton(socketClient)
            .AddSingleton(commandService);

        serviceProvider = services.BuildServiceProvider();

        socketClient.Log += SocketClient_Log;

        await RegisterCommandsAsync();
        await socketClient.LoginAsync(TokenType.Bot, accessToken);
        await socketClient.StartAsync();
    }

    private Task SocketClient_Log(LogMessage logMessage)
    {
        Console.WriteLine(logMessage);
        return Task.CompletedTask;
    }

    public async Task RegisterCommandsAsync()
    {
        socketClient.MessageReceived += HandleCommandAsync;

        await commandService.AddModulesAsync(typeof(Commands).Assembly, serviceProvider);
    }

    private async Task HandleCommandAsync(SocketMessage socketMessage)
    {
        var userMessage = socketMessage as SocketUserMessage;
        var context = new SocketCommandContext(socketClient, userMessage);
        if (userMessage.Author.IsBot) return;

        int argPos = 0;

        if (userMessage.HasStringPrefix("!", ref argPos))
        {
            var result = await commandService.ExecuteAsync(context, argPos, serviceProvider);
            if (!result.IsSuccess)
            {
                Console.WriteLine(result.ErrorReason);
            }
        }
    }
}