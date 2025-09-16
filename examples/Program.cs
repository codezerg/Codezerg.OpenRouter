using System;
using System.Linq;
using System.Threading.Tasks;
using Codezerg.OpenRouter.Examples.Core;

namespace Codezerg.OpenRouter.Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Parse command line arguments
            var verboseMode = args.Contains("--verbose") || args.Contains("-v");
            var stepMode = args.Contains("--step") || args.Contains("-s");
            var listMode = args.Contains("--list") || args.Contains("-l");
            var helpMode = args.Contains("--help") || args.Contains("-h");
            var specificExample = args.FirstOrDefault(a => !a.StartsWith("-"));

            if (helpMode)
            {
                ShowHelp();
                return;
            }

            // Validate API key
            if (!ErrorHandler.ValidateApiKey())
            {
                return;
            }

            try
            {
                // Create configuration
                var config = OpenRouterFactory.CreateDefaultConfig();

                if (verboseMode)
                {
                    config.EnableDebugLogging = true;
                }

                // Create and run the example runner
                var runner = new ExampleRunner(config, verboseMode, stepMode);

                if (listMode)
                {
                    runner.ListExamples();
                }
                else if (!string.IsNullOrEmpty(specificExample))
                {
                    await runner.RunByName(specificExample);
                }
                else
                {
                    await runner.RunInteractiveMenu();
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.Error($"Fatal error: {ex.Message}");
                if (verboseMode)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        static void ShowHelp()
        {
            ConsoleHelper.Header("Codezerg.OpenRouter Examples");

            Console.WriteLine("Usage: dotnet run [options] [example-name]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --help, -h      Show this help message");
            Console.WriteLine("  --list, -l      List all available examples");
            Console.WriteLine("  --verbose, -v   Enable verbose mode (show raw requests/responses)");
            Console.WriteLine("  --step, -s      Enable step mode (pause between operations)");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run                    Run interactive menu");
            Console.WriteLine("  dotnet run \"Simple Chat\"      Run specific example by name");
            Console.WriteLine("  dotnet run --list             List all examples");
            Console.WriteLine("  dotnet run -v -s              Run menu with verbose and step modes");
            Console.WriteLine();
            Console.WriteLine("Environment:");
            Console.WriteLine("  OPENROUTER_API_KEY    Your OpenRouter API key (required)");
            Console.WriteLine();
            Console.WriteLine("Get your API key at: https://openrouter.ai/keys");
        }
    }
}