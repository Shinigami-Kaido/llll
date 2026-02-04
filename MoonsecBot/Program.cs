using System;
using System.Threading.Tasks;

namespace MoonsecBot
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("Please set DISCORD_TOKEN environment variable.");
                return;
            }

            var bot = new Bot(token);
            await bot.StartAsync();

            // keep running
            await Task.Delay(-1);
        }
    }
}
