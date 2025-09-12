namespace Codezerg.OpenRouter.Models;

public static class ModelConstants
{
    public static class OpenAI
    {
        public const string Gpt4o = "openai/gpt-4o";
        public const string Gpt4oMini = "openai/gpt-4o-mini";
        public const string Gpt4Turbo = "openai/gpt-4-turbo";
        public const string Gpt35Turbo = "openai/gpt-3.5-turbo";
        public const string O1Preview = "openai/o1-preview";
        public const string O1Mini = "openai/o1-mini";
    }

    public static class Anthropic
    {
        public const string Claude35Sonnet = "anthropic/claude-3.5-sonnet";
        public const string Claude35Haiku = "anthropic/claude-3.5-haiku";
        public const string Claude3Opus = "anthropic/claude-3-opus";
        public const string Claude3Sonnet = "anthropic/claude-3-sonnet";
        public const string Claude3Haiku = "anthropic/claude-3-haiku";
    }

    public static class Google
    {
        public const string Gemini20FlashThinking = "google/gemini-2.0-flash-thinking";
        public const string Gemini20Flash = "google/gemini-2.0-flash-001";
        public const string Gemini15Pro = "google/gemini-1.5-pro";
        public const string Gemini15Flash = "google/gemini-1.5-flash";
        public const string Gemini25FlashImagePreview = "google/gemini-2.5-flash-image-preview";
    }

    public static class DeepSeek
    {
        public const string DeepSeekChatV3 = "deepseek/deepseek-chat-v3";
        public const string DeepSeekChatV3Free = "deepseek/deepseek-chat-v3.1:free";
        public const string DeepSeekR1 = "deepseek/deepseek-r1";
    }

    public static class Meta
    {
        public const string Llama3170B = "meta-llama/llama-3.1-70b-instruct";
        public const string Llama318B = "meta-llama/llama-3.1-8b-instruct";
        public const string Llama3270B = "meta-llama/llama-3.2-70b-instruct";
        public const string Llama323B = "meta-llama/llama-3.2-3b-instruct";
    }

    public static class Mistral
    {
        public const string MistralLarge = "mistral/mistral-large";
        public const string MistralMedium = "mistral/mistral-medium";
        public const string MistralSmall = "mistral/mistral-small";
        public const string Mixtral8x7B = "mistral/mixtral-8x7b-instruct";
    }

    public static class Cohere
    {
        public const string CommandRPlus = "cohere/command-r-plus";
        public const string CommandR = "cohere/command-r";
        public const string Command = "cohere/command";
    }

    public static class XAI
    {
        public const string Grok2 = "x-ai/grok-2";
        public const string Grok2Mini = "x-ai/grok-2-mini";
        public const string GrokBeta = "x-ai/grok-beta";
    }
}

public static class FinishReasons
{
    public const string Stop = "stop";
    public const string Length = "length";
    public const string ToolCalls = "tool_calls";
    public const string ContentFilter = "content_filter";
    public const string Error = "error";
}

public static class ObjectTypes
{
    public const string ChatCompletion = "chat.completion";
    public const string ChatCompletionChunk = "chat.completion.chunk";
}

public static class Modalities
{
    public const string Text = "text";
    public const string Image = "image";
    public const string Audio = "audio";
}

public static class ToolChoiceOptions
{
    public const string Auto = "auto";
    public const string None = "none";
    public const string Required = "required";
}

public static class VerbosityLevels
{
    public const string Low = "low";
    public const string Medium = "medium";
    public const string High = "high";
}

public static class DataCollectionOptions
{
    public const string Allow = "allow";
    public const string Deny = "deny";
}