using System.ComponentModel.DataAnnotations;

namespace AiChat.Options;

public sealed class OpenAIOptions
{
    public const string SectionName = "OpenAI";

    [Required] public string ModelName { get; init; } = "";
    [Required] public string ApiKey { get; init; } = "";
}