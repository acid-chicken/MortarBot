using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MortarBot
{
    [Group(""), Summary("The module containing common commands.")]
    public class CommonModule : ModuleBase
    {
        public CommandService Commands { get; set; }

        [Command("help"), Summary("Tells usage of the command.")]
        public Task HelpAsync([Summary("The name of the command.")] string name = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                return ReplyAsync(Context.User.Mention,
                    embed: new EmbedBuilder()
                    {
                        Fields = Commands.Commands.Select(x =>
                        {
                            var builder = new StringBuilder();
                            var module = x.Module;
                            while (!string.IsNullOrEmpty(module?.Name))
                            {
                                builder.Insert(0, ' ');
                                builder.Insert(0, module.Name);
                                module = module.Parent;
                            }
                            builder.Append(x.Name);
                            return new EmbedFieldBuilder()
                                .WithName(builder.ToString())
                                .WithValue(x.Summary)
                                .WithIsInline(true);
                        })
                        .ToList()
                    }
                        .WithTitle("Commands List")
                        .WithDescription("That's all you can use.")
                        .WithCurrentTimestamp()
                        .WithColor(Assets.Blue)
                        .WithFooter("MortarBot")
                        .WithAuthor(Context.User)
                        .Build());
            }
            else
            {
                var command = Commands.Commands.FirstOrDefault(x => x.Name == name || x.Aliases.Contains(name)) ?? throw new ArgumentException($"The command `{name}` not found.");
                return ReplyAsync(Context.User.Mention,
                    embed: new EmbedBuilder()
                    {
                        Fields = command.Parameters.Select(x => new EmbedFieldBuilder()
                            .WithName(x.Name)
                            .WithValue(x.Summary)
                            .WithIsInline(true))
                        .ToList()
                    }
                        .WithTitle($"Usage of `{command.Name}`")
                        .WithDescription($"{command.Summary}\n```zsh\n{string.Join(' ', command.Parameters.Select(x => $"{(x.IsOptional ? "[" : "")}{x.Name}: {x.Type.Name}{(x.IsOptional ? "]" : "")}{(x.IsMultiple ? "..." : "")}"))}\n```")
                        .WithCurrentTimestamp()
                        .WithColor(Assets.Blue)
                        .WithFooter("MortarBot")
                        .WithAuthor(Context.User)
                        .Build());
            }
        }

        [Command("echo"), Summary("Reply the same."), Alias("reply", "<")]
        public Task EchoAsync([Summary("Context"), Remainder] string context)
            => ReplyAsync($"{Context.User.Mention} {context}");

        [Command("calculate"), Summary("Calculates the MortarMath formula."), Alias("calc", "=")]
        public Task CalculateAsync([Summary("Formula"), Remainder] string formula)
            => ReplyAsync(Context.User.Mention,
                embed: new EmbedBuilder()
                    .WithTitle("Calculation Result")
                    .WithDescription(MortarMath.CalculateAsString(formula))
                    .WithCurrentTimestamp()
                    .WithColor(Assets.Blue)
                    .WithFooter("MortarMath - MortarBot")
                    .WithAuthor(Context.User)
                    .Build());
    }
}
