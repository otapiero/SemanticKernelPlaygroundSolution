using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using SemanticKernelPlayground.Plugins;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
    .Build();

var modelName = configuration["ModelName"] ?? throw new ApplicationException("ModelName not found");
var endpoint = configuration["Endpoint"] ?? throw new ApplicationException("Endpoint not found");
var apiKey = configuration["ApiKey"] ?? throw new ApplicationException("ApiKey not found");

var builder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(modelName, endpoint, apiKey);
builder.Plugins.AddFromType<GitPlugin>();
builder.Plugins.AddFromType<ReleaseStoragePlugin>();
string promptsDirectory =  Path.Combine(Directory.GetCurrentDirectory(), "Prompts");

builder.Plugins.AddFromPromptDirectory(promptsDirectory);

var kernel = builder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

AzureOpenAIPromptExecutionSettings openAiPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};
var systemPromptPath = Path.Combine(Directory.GetCurrentDirectory(), "Prompts", "system-prompt.txt");
string systemPrompt = File.ReadAllText(systemPromptPath);

var history = new ChatHistory(systemPrompt);

do
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("Me > ");
    Console.ResetColor();

    var userInput = Console.ReadLine();
    if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "exit")
    {
        break;
    }

    history.AddUserMessage(userInput);

    try
    {
        var streamingResponse =
            chatCompletionService.GetStreamingChatMessageContentsAsync(
                history,
                openAiPromptExecutionSettings,
                kernel);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Agent > ");
        Console.ResetColor();

        var fullResponse = "";
        await foreach (var chunk in streamingResponse)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(chunk.Content);
            Console.ResetColor();
            fullResponse += chunk.Content;
        }
        Console.WriteLine();

        history.AddMessage(AuthorRole.Assistant, fullResponse);

    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.ResetColor();

        if (ex.ToString().Contains("content_filter") ||
            ex.ToString().Contains("content management policy"))
        {
            string errorMessage = "I apologize, but your request triggered content filters. " +
                                 "Please rephrase your question or try a different topic.";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(errorMessage);
            Console.ResetColor();

            history.AddMessage(AuthorRole.Assistant, errorMessage);
        }
        else
        {
            string errorMessage = $"An error ocured pleas try again or explain your request.";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(errorMessage);
            Console.ResetColor();

            history.AddMessage(AuthorRole.Assistant, errorMessage);
        }
    }


    } while (true);