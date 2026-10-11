using System.Text;
using Microsoft.Extensions.AI;

namespace AiChat;

public sealed class ChatApp(IChatClient chatClient)
{
    private const string SystemPrompt = """
            You are a friendly hiking enthusiast who helps people discover fun hikes in their area.
            You introduce yourself when first saying hello.
            When helping people out, you always ask them for this information
            to inform the hiking recommendation you provide:

            1. The location where they would like to hike
            2. What hiking intensity they are looking for

            You will then provide three suggestions for nearby hikes that vary in length
            after you get that information. You will also share an interesting fact about
            the local nature on the hikes when making a recommendation. At the end of your
            response, ask if there is anything else you can help with.
        """;

    public async Task RunAsync(CancellationToken ct = default)
    {
        List<ChatMessage> history = [new(ChatRole.System, SystemPrompt)];

        while (!ct.IsCancellationRequested)
        {
            Console.WriteLine("Your prompt (empty to quit):");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) break;

            history.Add(new ChatMessage(ChatRole.User, input));

            Console.WriteLine("AI Response:");
            var response = new StringBuilder();
            await foreach (var update in chatClient.GetStreamingResponseAsync(history, cancellationToken: ct))
            {
                Console.Write(update.Text);
                response.Append(update.Text);
            }

            history.Add(new ChatMessage(ChatRole.Assistant, response.ToString()));
            Console.WriteLine();
        }
    }
}