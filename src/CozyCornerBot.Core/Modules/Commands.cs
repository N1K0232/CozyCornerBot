using Discord.Commands;

namespace CozyCornerBot.Core.Modules;

public class Commands : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    public async Task Ping()
    {
        await ReplyAsync("pong");
    }
}