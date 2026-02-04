using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MoonsecDeobfuscator.Deobfuscation;
using MoonsecDeobfuscator.Deobfuscation.Bytecode;

namespace MoonsecBot
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly string _token;
        private readonly HttpClient _http = new();

        public Bot(string token)
        {
            _token = token;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds
                                 | GatewayIntents.GuildMessages
                                 | GatewayIntents.DirectMessages
            });

            _client.Log += LogAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task StartAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
        }

        private Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            try
            {
                if (message.Author.IsBot) return;
                var content = message.Content.Trim();
                if (!content.StartsWith("!deob")) return;

                var tokens = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var mode = tokens.Length >= 2 && (tokens[1] == "-dis" || tokens[1] == "-dev") ? tokens[1] : "-dis";

                await message.Channel.TriggerTypingAsync();

                // Get input from attachment first, otherwise from code block or trailing text
                string input;
                if (message.Attachments.Any())
                {
                    var url = message.Attachments.First().Url;
                    input = await _http.GetStringAsync(url);
                }
                else
                {
                    var m = Regex.Match(content, "```(?:lua\\n)?([\\s\\S]*?)```", RegexOptions.IgnoreCase);
                    if (m.Success)
                        input = m.Groups[1].Value;
                    else
                    {
                        input = content.Substring("!deob".Length).Trim();
                        if (string.IsNullOrEmpty(input))
                        {
                            await message.Channel.SendMessageAsync("No attachment or code block found. Attach the obfuscated script or paste it in a code block.");
                            return;
                        }
                    }
                }

                Function resultFunction;
                try
                {
                    // Consider using Task.Run if deobfuscation is CPU-bound
                    var deob = new Deobfuscator();
                    resultFunction = deob.Deobfuscate(input);
                }
                catch (Exception ex)
                {
                    await message.Channel.SendMessageAsync($"Deobfuscation failed: {ex.Message}");
                    return;
                }

                if (mode == "-dis")
                {
                    var generator = new OptimizedLuaGenerator(resultFunction);
                    var clean = generator.Generate();
                    if (string.IsNullOrWhiteSpace(clean))
                        clean = "-- (empty result)";

                    if (clean.Length < 1900)
                        await message.Channel.SendMessageAsync($"```lua\n{clean}\n```");
                    else
                    {
                        using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(clean));
                        ms.Position = 0;
                        await message.Channel.SendFileAsync(ms, "clean.lua", $"Deobfuscated by MoonBot (requested by {message.Author.Username})");
                    }
                }
                else // -dev
                {
                    using var ms = new MemoryStream();
                    var serializer = new Serializer(ms);
                    serializer.Serialize(resultFunction);
                    ms.Position = 0;
                    await message.Channel.SendFileAsync(ms, "devirtualized.luac", $"Serialized bytecode (requested by {message.Author.Username})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling message: {ex}");
            }
        }
    }
}
