using AiChat;
using AiChat.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddAiChat(builder.Configuration);

using var host = builder.Build();
await host.StartAsync(); // runs ValidateOnStart so bad config fails here, not on first prompt

await host.Services.GetRequiredService<ChatApp>().RunAsync();

await host.StopAsync();
