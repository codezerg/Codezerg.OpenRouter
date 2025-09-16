using System.Threading.Tasks;
using Codezerg.OpenRouter;

namespace Codezerg.OpenRouter.Examples.Core
{
    /// <summary>
    /// Base interface for all examples in the framework
    /// </summary>
    public interface IExample
    {
        /// <summary>
        /// Runs the example with the provided configuration
        /// </summary>
        /// <param name="config">OpenRouter client configuration</param>
        Task RunAsync(OpenRouterClientOptions config);
    }
}