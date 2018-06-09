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
                .AddLogging(configure => configure.SetMinimumLevel(int.TryParse(Configuration["Log:Level"], out int result) ? (LogLevel)result : LogLevel.Trace)
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

            await client.LoginAsync(TokenType.Bot, Configuration["Discord:Token"]);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> cacheableMessage, ISocketMessageChannel channel, SocketReaction reaction, DiscordSocketClient client)
        {
            var message = await cacheableMessage.GetOrDownloadAsync();
            if (message.Author == client.CurrentUser &&
                message.MentionedUserIds.Contains(reaction.UserId) &&
                reaction.Emote.Name == "\ud83d\udeab")
                await message.DeleteAsync();
        }

        private Task StartClockAsync(DiscordSocketClient client)
            => Task.WhenAny(Task.CompletedTask,
                Task.Run(async () =>
                {
                    var interval = int.Parse(Configuration["Discord:Clock:Interval"]);
                    foreach (var timeZone in new List<(string name, TimeZoneInfo info)>()
                    {
                        ("\ud83c\udde6\ud83c\uddf7 Buenos Aires, Argentina", TZConvert.TryGetTimeZoneInfo("America/Argentina/Buenos_Aires", out var buenosAires) ? buenosAires : null),
                        ("\ud83c\udde6\ud83c\uddfa Sydney, Australia",       TZConvert.TryGetTimeZoneInfo("Australia/Sydney",               out var sydney)      ? sydney      : null),
                        ("\ud83c\udde7\ud83c\uddfe Brussels, Belarus",       TZConvert.TryGetTimeZoneInfo("Europe/Minsk",                   out var minsk)       ? minsk       : null),
                        ("\ud83c\udde7\ud83c\uddea Brussels, Belgium",       TZConvert.TryGetTimeZoneInfo("Europe/Brussels",                out var brussels)    ? brussels    : null),
                        ("\ud83c\udde7\ud83c\uddf7 Sao Paulo, Brazil",       TZConvert.TryGetTimeZoneInfo("America/Sao_Paulo",              out var saoPaulo)    ? saoPaulo    : null),
                        ("\ud83c\udde8\ud83c\udde6 Toronto, Canada",         TZConvert.TryGetTimeZoneInfo("America/Toronto",                out var toronto)     ? toronto     : null),
                        ("\ud83c\udde8\ud83c\uddf3 Shanghai, China",         TZConvert.TryGetTimeZoneInfo("Asia/Shanghai",                  out var shanghai)    ? shanghai    : null),
                        ("\ud83c\udde9\ud83c\uddf0 Copenhagen, Denmark",     TZConvert.TryGetTimeZoneInfo("Europe/Copenhagen",              out var copenhagen)  ? copenhagen  : null),
                        ("\ud83c\uddeb\ud83c\uddf7 Paris, France",           TZConvert.TryGetTimeZoneInfo("Europe/Paris",                   out var paris)       ? paris       : null),
                        ("\ud83c\udde9\ud83c\uddea Berlin, Germany",         TZConvert.TryGetTimeZoneInfo("Europe/Berlin",                  out var berlin)      ? berlin      : null),
                        ("\ud83c\uddec\ud83c\udde7 London, Great Britain",   TZConvert.TryGetTimeZoneInfo("Europe/London",                  out var london)      ? london      : null),
                        ("\ud83c\uddf0\ud83c\uddff Almaty, Kazakhstan",      TZConvert.TryGetTimeZoneInfo("Asia/Almaty",                    out var almaty)      ? almaty      : null),
                        ("\ud83c\uddee\ud83c\uddf8 Reykjavik, Iceland",      TZConvert.TryGetTimeZoneInfo("Atlantic/Reykjavik",             out var reykjavik)   ? reykjavik   : null),
                        ("\ud83c\uddee\ud83c\uddea Dublin, Ireland",         TZConvert.TryGetTimeZoneInfo("Europe/Dublin",                  out var dublin)      ? dublin      : null),
                        ("\ud83c\uddee\ud83c\uddf9 Rome, Italy",             TZConvert.TryGetTimeZoneInfo("Europe/Rome",                    out var rome)        ? rome        : null),
                        ("\ud83c\uddef\ud83c\uddf5 Tokyo, Japan",            TZConvert.TryGetTimeZoneInfo("Asia/Tokyo",                     out var tokyo)       ? tokyo       : null),
                        ("\ud83c\uddf3\ud83c\uddf1 Amsterdam, Netherlands",  TZConvert.TryGetTimeZoneInfo("Europe/Amsterdam",               out var amsterdam)   ? amsterdam   : null),
                        ("\ud83c\uddf3\ud83c\uddff Auckland, New Zealand",   TZConvert.TryGetTimeZoneInfo("Pacific/Auckland",               out var auckland)    ? auckland    : null),
                        ("\ud83c\uddf5\ud83c\uddf1 Warsaw, Poland",          TZConvert.TryGetTimeZoneInfo("Europe/Warsaw",                  out var warsaw)      ? warsaw      : null),
                        ("\ud83c\uddf0\ud83c\uddf7 Seoul, South Korea",      TZConvert.TryGetTimeZoneInfo("Asia/Seoul",                     out var seoul)       ? seoul       : null),
                        ("\ud83c\uddf7\ud83c\uddfa Moscow, Russia",          TZConvert.TryGetTimeZoneInfo("Europe/Moscow",                  out var moscow)      ? moscow      : null),
                        ("\ud83c\uddea\ud83c\uddf8 Madrid, Spain",           TZConvert.TryGetTimeZoneInfo("Europe/Madrid",                  out var madrid)      ? madrid      : null),
                        ("\ud83c\uddf8\ud83c\uddea Stockholm, Sweden",       TZConvert.TryGetTimeZoneInfo("Europe/Stockholm",               out var stockholm)   ? stockholm   : null),
                        ("\ud83c\uddf9\ud83c\uddf7 Istanbul, Turkish",       TZConvert.TryGetTimeZoneInfo("Europe/Istanbul",                out var istanbul)    ? istanbul    : null),
                        ("\ud83c\uddfa\ud83c\udde6 Kiev, Ukraine",           TZConvert.TryGetTimeZoneInfo("Europe/Kiev",                    out var kiev)        ? kiev        : null),
                        ("\ud83c\uddfa\ud83c\uddf8 New York, USA",           TZConvert.TryGetTimeZoneInfo("America/New_York",               out var newYork)     ? newYork     : null)
                    }.ToInfinite())
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
                case IGuildChannel _ when (received.HasMentionPrefix(client.CurrentUser, ref position) || received.HasStringPrefix(Configuration["Discord:Command:Prefix"], ref position)):
                case IGroupChannel _ when (received.HasMentionPrefix(client.CurrentUser, ref position) || received.HasStringPrefix(Configuration["Discord:Command:Prefix"], ref position)):
                case IDMChannel    _ when (received.HasMentionPrefix(client.CurrentUser, ref position) || received.HasStringPrefix(Configuration["Discord:Command:Prefix"], ref position) || true): break;
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
