using System;
using System.Threading.Tasks;

namespace MoonsecBot
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var token = Environment.GetEnvironmentVariable("MTQ2ODU4NjIwOTk0MDA3ODY0NQ.GlleuE.UX6KkUXr9kIm4-Qa_pEVaj8yIBZWZk1geDsJeI");
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
