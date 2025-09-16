using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Codezerg.OpenRouter.Examples.Core
{
    /// <summary>
    /// Manages example discovery and execution
    /// </summary>
    public class ExampleRunner
    {
        private readonly List<ExampleInfo> _examples;
        private readonly OpenRouterClientOptions _config;
        private readonly bool _verboseMode;
        private readonly bool _stepMode;

        public ExampleRunner(OpenRouterClientOptions config, bool verboseMode = false, bool stepMode = false)
        {
            _config = config;
            _verboseMode = verboseMode;
            _stepMode = stepMode;
            _examples = DiscoverExamples();
        }

        /// <summary>
        /// Discovers all examples in the assembly using reflection
        /// </summary>
        private List<ExampleInfo> DiscoverExamples()
        {
            var examples = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IExample).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => new ExampleInfo
                {
                    Type = t,
                    Attribute = t.GetCustomAttribute<ExampleAttribute>(),
                    Instance = null
                })
                .Where(e => e.Attribute != null)
                .OrderBy(e => e.Attribute.Name)
                .ToList();

            return examples;
        }

        /// <summary>
        /// Runs the interactive menu
        /// </summary>
        public async Task RunInteractiveMenu()
        {
            while (true)
            {
                Console.Clear();
                ConsoleHelper.Header("OpenRouter Examples");

                if (_verboseMode)
                    ConsoleHelper.Info("Verbose mode enabled - raw requests/responses will be shown");
                if (_stepMode)
                    ConsoleHelper.Info("Step mode enabled - press key between operations");

                Console.WriteLine("\nSelect an example to run:\n");

                for (int i = 0; i < _examples.Count; i++)
                {
                    var example = _examples[i];
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"  {i + 1}) ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{example.Attribute.Name}");
                    Console.WriteLine($"       {example.Attribute.Description}");
                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"  0) ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Exit");
                Console.WriteLine();
                Console.ResetColor();

                var input = ConsoleHelper.Prompt("Enter your choice");

                if (!int.TryParse(input, out var choice))
                {
                    ConsoleHelper.Error("Invalid input. Please enter a number.");
                    ConsoleHelper.WaitForKey();
                    continue;
                }

                if (choice == 0)
                {
                    Console.WriteLine("\nGoodbye!");
                    break;
                }

                if (choice < 1 || choice > _examples.Count)
                {
                    ConsoleHelper.Error($"Invalid choice. Please enter a number between 0 and {_examples.Count}.");
                    ConsoleHelper.WaitForKey();
                    continue;
                }

                await RunExample(_examples[choice - 1]);
            }
        }

        /// <summary>
        /// Runs a specific example by name
        /// </summary>
        public async Task RunByName(string name)
        {
            var example = _examples.FirstOrDefault(e =>
                e.Attribute.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (example == null)
            {
                ConsoleHelper.Error($"Example '{name}' not found.");
                Console.WriteLine("\nAvailable examples:");
                foreach (var e in _examples)
                {
                    Console.WriteLine($"  - {e.Attribute.Name}");
                }
                return;
            }

            await RunExample(example);
        }

        /// <summary>
        /// Lists all available examples
        /// </summary>
        public void ListExamples()
        {
            ConsoleHelper.Header("Available Examples");

            foreach (var example in _examples)
            {
                Console.WriteLine($"{example.Attribute.Name}");
                Console.WriteLine($"  {example.Attribute.Description}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Runs a specific example
        /// </summary>
        private async Task RunExample(ExampleInfo exampleInfo)
        {
            Console.Clear();
            ConsoleHelper.Header(exampleInfo.Attribute.Name);
            Console.WriteLine(exampleInfo.Attribute.Description);
            Console.WriteLine();

            try
            {
                // Create instance if not already created
                if (exampleInfo.Instance == null)
                {
                    exampleInfo.Instance = (IExample)Activator.CreateInstance(exampleInfo.Type);
                }

                // Configure for verbose/step modes if supported
                if (_verboseMode && exampleInfo.Instance is IVerboseExample verboseExample)
                {
                    verboseExample.VerboseMode = true;
                }

                if (_stepMode && exampleInfo.Instance is ISteppableExample steppableExample)
                {
                    steppableExample.StepMode = true;
                }

                // Run the example
                await exampleInfo.Instance.RunAsync(_config);

                ConsoleHelper.Success("Example completed successfully!");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Error($"Example failed: {ex.Message}");

                if (_verboseMode)
                {
                    Console.WriteLine();
                    Console.WriteLine("Stack trace:");
                    Console.WriteLine(ex.StackTrace);
                }
            }

            Console.WriteLine();
            ConsoleHelper.WaitForKey("Press any key to return to menu...");
        }

        /// <summary>
        /// Information about a discovered example
        /// </summary>
        private class ExampleInfo
        {
            public Type Type { get; set; }
            public ExampleAttribute Attribute { get; set; }
            public IExample Instance { get; set; }
        }
    }

    /// <summary>
    /// Interface for examples that support verbose mode
    /// </summary>
    public interface IVerboseExample : IExample
    {
        bool VerboseMode { get; set; }
    }

    /// <summary>
    /// Interface for examples that support step mode
    /// </summary>
    public interface ISteppableExample : IExample
    {
        bool StepMode { get; set; }
    }
}