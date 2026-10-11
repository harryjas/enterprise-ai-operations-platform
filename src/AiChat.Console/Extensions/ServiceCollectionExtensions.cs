using AiChat.Analysis;
using AiChat.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;

namespace AiChat.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAiChat(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<OpenAIOptions>()
            .Bind(config.GetSection(OpenAIOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IChatClient>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<OpenAIOptions>>().Value;
            return new OpenAIClient(opts.ApiKey)
                .GetChatClient(opts.ModelName)
                .AsIChatClient();
        });

        services.AddTransient<ChatApp>();
        services.AddTransient<TextAnalyzer>();

        return services;
    }
}