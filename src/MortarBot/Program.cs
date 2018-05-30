using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace MortarBot
{
    public class Program
    {
        private Program(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets<Secrets>()
                .Build();

            var client = new DiscordSocketClient();
            var commands = new CommandService();

            Services = new ServiceCollection()
                .Configure<Secrets>(_ => Configuration.GetSection(nameof(Secrets)))
                .AddOptions()
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddLogging(configure => configure.SetMinimumLevel(LogLevel.Trace)
                    .AddConsole())
                .BuildServiceProvider();
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider Services { get; }

        private static Task Main(string[] args)
            => new Program(args).RunAsync();

        private async Task RunAsync()
        {
            var client = Services.GetService<DiscordSocketClient>();
            var commands = Services.GetService<CommandService>();

            var logger = Services.GetService<ILogger<Program>>();
            client.Log += e => LogAsync(e, logger);
            commands.Log += e => LogAsync(e, logger);

            client.MessageReceived += e => HandleCommandAsync(e, commands, client);

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());

            await client.LoginAsync(TokenType.Bot, Configuration["Discord:Token"]);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task HandleCommandAsync(SocketMessage message, CommandService commands, DiscordSocketClient client)
        {
            var received = message as SocketUserMessage;
            if (received is null ||
                received.Author == client.CurrentUser ||
                received.Author.IsBot) return;

            var position = 0;
            switch (received.Channel)
            {
                case IGuildChannel _ when (received.HasMentionPrefix(client.CurrentUser, ref position) || received.HasStringPrefix(Configuration["Discord:CommandPrefix"], ref position)):
                case IGroupChannel _ when (received.HasMentionPrefix(client.CurrentUser, ref position) || received.HasStringPrefix(Configuration["Discord:CommandPrefix"], ref position)):
                case IDMChannel    _ when (received.HasMentionPrefix(client.CurrentUser, ref position) || received.HasStringPrefix(Configuration["Discord:CommandPrefix"], ref position) || true): break;
                default: return;
            }

            var context = new CommandContext(client, received);
            using (var typing = context.Channel.EnterTypingState())
            {
                var result = await commands.ExecuteAsync(context, position);
                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(context.User.Mention,
                        embed: new EmbedBuilder().WithTitle("Command Error")
                            .WithDescription(result.ErrorReason)
                            .WithCurrentTimestamp()
                            .WithColor(Assets.Red)
                            .WithFooter("MortarBot")
                            .WithAuthor(context.User)
                            .Build());
                }
            }
        }

        private Task LogAsync(LogMessage message, ILogger logger)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical: logger.LogCritical(message.Exception, message.Message);    break;
                case LogSeverity.Error:    logger.LogError(message.Exception, message.Message);       break;
                case LogSeverity.Warning:  logger.LogWarning(message.Exception, message.Message);     break;
                case LogSeverity.Info:     logger.LogInformation(message.Exception, message.Message); break;
                case LogSeverity.Verbose:  logger.LogTrace(message.Exception, message.Message);       break;
                case LogSeverity.Debug:    logger.LogDebug(message.Exception, message.Message);       break;
            }
            return Task.CompletedTask;
        }
    }
}
