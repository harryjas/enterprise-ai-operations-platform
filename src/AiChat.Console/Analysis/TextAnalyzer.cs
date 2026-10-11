using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;
namespace AiChat.Analysis;

public sealed class TextAnalyzer(IChatClient chatClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow // reject extra fields
    };

    private const string SystemPrompt = """
        Analyze the user's text. Reply with JSON only, no markdown, no commentary.
        Schema: {"summary": string, "sentiment": "Positive" | "Neutral" | "Negative", "confidence": number between 0 and 1}
        """;

    public async Task<TextAnalysis> AnalyzeAsync(string text, CancellationToken ct = default)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, text)
        };

        var response = await chatClient.GetResponseAsync(messages, cancellationToken: ct);

        try
        {
            return ParseAndValidate(response.Text);
        }
        catch (InvalidModelResponseException)
        {
            // One retry: show the model its own bad reply and ask for a correction
            messages.Add(new ChatMessage(ChatRole.Assistant, response.Text));
            messages.Add(new ChatMessage(ChatRole.User,
                "That reply was not valid for the schema. Reply with corrected JSON only."));

            response = await chatClient.GetResponseAsync(messages, cancellationToken: ct);
            return ParseAndValidate(response.Text);   // a second failure propagates to the caller
        }
    }

    // Pure function: no model needed, so you can unit test valid + invalid payloads
    public static TextAnalysis ParseAndValidate(string raw)
    {
        // Models sometimes wrap JSON in ```json fences; strip them
        var json = raw.Trim().Trim('`');
        if (json.StartsWith("json", StringComparison.OrdinalIgnoreCase)) json = json[4..];

        TextAnalysis? result;
        try
        {
            result = JsonSerializer.Deserialize<TextAnalysis>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidModelResponseException("Response is not valid JSON for TextAnalysis.", ex);
        }

        if (result is null)
            throw new InvalidModelResponseException("Response deserialized to null.");
        if (string.IsNullOrWhiteSpace(result.Summary))
            throw new InvalidModelResponseException("Summary is empty.");
        if (result.Confidence is < 0 or > 1)
            throw new InvalidModelResponseException($"Confidence {result.Confidence} is outside 0..1.");

        return result;
    }
}