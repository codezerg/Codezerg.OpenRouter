using System;

namespace Codezerg.OpenRouter.Examples.Core
{
    /// <summary>
    /// Helper class for standardized console output
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Prints a main header
        /// </summary>
        public static void Header(string text)
        {
            Console.WriteLine($"\n{"".PadRight(50, '=')}");
            Console.WriteLine($"=== {text}");
            Console.WriteLine($"{"".PadRight(50, '=')}\n");
        }

        /// <summary>
        /// Prints a section divider
        /// </summary>
        public static void Section(string text)
        {
            Console.WriteLine($"\n--- {text} ---");
        }

        /// <summary>
        /// Prints an info message
        /// </summary>
        public static void Info(string text)
        {
            Console.WriteLine($"[INFO] {text}");
        }

        /// <summary>
        /// Prints a warning message
        /// </summary>
        public static void Warn(string text)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN] {text}");
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Prints an error message
        /// </summary>
        public static void Error(string text)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {text}");
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Prints a success message
        /// </summary>
        public static void Success(string text)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SUCCESS] {text}");
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Prints a prompt and waits for user input
        /// </summary>
        public static string Prompt(string text)
        {
            Console.Write($"{text}: ");
            return Console.ReadLine() ?? string.Empty;
        }

        /// <summary>
        /// Waits for user to press any key
        /// </summary>
        public static void WaitForKey(string message = "Press any key to continue...")
        {
            Console.WriteLine(message);
            Console.ReadKey(true);
        }

        /// <summary>
        /// Clears the console
        /// </summary>
        public static void Clear()
        {
            Console.Clear();
        }
    }
}