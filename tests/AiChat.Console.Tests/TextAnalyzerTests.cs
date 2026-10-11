using AiChat.Analysis;

namespace AiChat.Console.Tests;

public class TextAnalyzerTests
{
    private const string Valid = """{"summary": "ok", "sentiment": "Positive", "confidence": 0.9}""";
    private const string BadEnum = """{"summary": "ok", "sentiment": "Happy", "confidence": 0.9}""";

    [Fact]
    public void ParseAndValidate_ValidJson_ReturnsAnalysis()
    {
        var result = TextAnalyzer.ParseAndValidate(Valid);

        Assert.Equal("ok", result.Summary);
        Assert.Equal(Sentiment.Positive, result.Sentiment);
        Assert.Equal(0.9, result.Confidence);
    }

    [Fact]
    public void ParseAndValidate_JsonInMarkdownFence_ReturnsAnalysis()
    {
        var result = TextAnalyzer.ParseAndValidate("```json\n" + Valid + "\n```");

        Assert.Equal(Sentiment.Positive, result.Sentiment);
    }

    [Theory]
    [InlineData("""{"summary": "ok", "sentiment": "Happy", "confidence": 0.9}""")]     // bad enum
    [InlineData("""{"summary": "ok", "sentiment": "Positive", "confidence": 7}""")]    // out of range
    [InlineData("""{"summary": "ok", "sentiment": "Positive" """)]                     // truncated
    [InlineData("""{"summary": "", "sentiment": "Positive", "confidence": 0.5}""")]    // empty summary
    [InlineData("""{"summary": "ok", "sentiment": "Positive", "confidence": 0.5, "x": 1}""")] // extra field
    [InlineData("null")]
    public void ParseAndValidate_InvalidPayload_Throws(string payload)
    {
        Assert.Throws<InvalidModelResponseException>(() => TextAnalyzer.ParseAndValidate(payload));
    }

    [Fact]
    public async Task AnalyzeAsync_ValidFirstReply_DoesNotRetry()
    {
        var client = new FakeChatClient(Valid);

        var result = await new TextAnalyzer(client).AnalyzeAsync("text");

        Assert.Equal(Sentiment.Positive, result.Sentiment);
        Assert.Equal(1, client.Calls);
    }

    [Fact]
    public async Task AnalyzeAsync_InvalidThenValid_RetriesOnce()
    {
        var client = new FakeChatClient(BadEnum, Valid);

        var result = await new TextAnalyzer(client).AnalyzeAsync("text");

        Assert.Equal(Sentiment.Positive, result.Sentiment);
        Assert.Equal(2, client.Calls);
    }

    [Fact]
    public async Task AnalyzeAsync_InvalidTwice_ThrowsAfterOneRetry()
    {
        var client = new FakeChatClient(BadEnum, BadEnum);

        await Assert.ThrowsAsync<InvalidModelResponseException>(
            () => new TextAnalyzer(client).AnalyzeAsync("text"));

        Assert.Equal(2, client.Calls);
    }
}
