using System;
using System.Collections.Generic;
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
using TimeZoneConverter;

namespace MortarBot
{
    public class Program
    {
        private Program(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<Secrets>()
                .AddCommandLine(args)
                .Build();

            var client = new DiscordSocketClient();
            var commands = new CommandService();

            Services = new ServiceCollection()
                .Configure<Secrets>(_ => Configuration.GetSection(nameof(Secrets)))
                .AddOptions()
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddLogging(configure => configure.SetMinimumLevel(Configuration.GetValue("Log:Level", LogLevel.Trace))
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
            client.Log += message => LogAsync(message, logger);
            commands.Log += message => LogAsync(message, logger);

            client.ReactionAdded += (message, channel, reaction) => HandleReactionAsync(message, channel, reaction, client);
            client.Ready += () => StartClockAsync(client);
            client.MessageReceived += message => HandleCommandAsync(message, commands, client);
            client.MessageUpdated += (_, message, __) => HandleCommandAsync(message, commands, client);

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());

            var token = Configuration.GetValue("Discord:Token", default(string));
            if (token is null)
                throw new InvalidConfigurationException("The Discord token isn't set. Please execute `dotnet user-secrets set 'Discord:Token' '> YOUR BOT TOKEN GOES HERE <'`");
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> cacheableMessage, ISocketMessageChannel channel, SocketReaction reaction, DiscordSocketClient client)
        {
            var message = await cacheableMessage.GetOrDownloadAsync();
            var reactions = new List<string>() as IEnumerable<string>;
            Configuration.GetSection("Discord:Delete:Reaction").Bind(reactions);
            if (message.Author.Id == client.CurrentUser.Id &&
                message.MentionedUserIds.Contains(reaction.UserId) &&
                reactions.Contains(reaction.Emote.Name.Replace("\ufe0f", "")))
                await message.DeleteAsync();
        }

        private Task StartClockAsync(DiscordSocketClient client)
            => Task.WhenAny(Task.CompletedTask,
                Task.Run(async () =>
                {
                    var interval = Configuration.GetValue("Discord:Clock:Interval", 15000);
                    var timezones = new Dictionary<string, string>();
                    Configuration.GetSection("Discord:Clock:TimeZone").Bind(timezones);
                    foreach (var timeZone in timezones
                        .Select(x => (name: x.Value, info: TZConvert.TryGetTimeZoneInfo(x.Key, out var result) ? result : null))
                        .ToInfinite())
                    {
                        if (!(timeZone.info is null))
                            await Task.WhenAll(Task.Delay(interval),
                                client.SetGameAsync($"{TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone.info):HH:mm} {timeZone.name}", type: ActivityType.Watching));
                    }
                }));

        private async Task HandleCommandAsync(SocketMessage message, CommandService commands, DiscordSocketClient client)
        {
            if (!(message is SocketUserMessage received) ||
                received.Author.IsBot) return;

            var position = 0;
            switch (received.Channel)
            {
                case IGuildChannel _ when (received.HasMentionPrefix(client.CurrentUser, ref position) || received.HasStringPrefix(Configuration.GetValue("Discord:Command:Prefix", ""), ref position)):
                case IGroupChannel _ when (received.HasMentionPrefix(client.CurrentUser, ref position) || received.HasStringPrefix(Configuration.GetValue("Discord:Command:Prefix", ""), ref position)):
                case IDMChannel    _ when (received.HasMentionPrefix(client.CurrentUser, ref position) || received.HasStringPrefix(Configuration.GetValue("Discord:Command:Prefix", ""), ref position) || true): break;
                default: return;
            }

            var context = new CommandContext(client, received);
            using (var typing = context.Channel.EnterTypingState())
            {
                var result = await commands.ExecuteAsync(context, position);
                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(context.User.Mention,
                        embed: new EmbedBuilder()
                            .WithTitle("Command Error")
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
                case LogSeverity.Critical: logger.LogCritical(   message.Exception, message.Message); break;
                case LogSeverity.Error:    logger.LogError(      message.Exception, message.Message); break;
                case LogSeverity.Warning:  logger.LogWarning(    message.Exception, message.Message); break;
                case LogSeverity.Info:     logger.LogInformation(message.Exception, message.Message); break;
                case LogSeverity.Verbose:  logger.LogTrace(      message.Exception, message.Message); break;
                case LogSeverity.Debug:    logger.LogDebug(      message.Exception, message.Message); break;
            }
            return Task.CompletedTask;
        }
    }
}
