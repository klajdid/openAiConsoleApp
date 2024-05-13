// See https://aka.ms/new-console-template for more information
using OpenAI_API;
using OpenAI_API.Completions;

string openAiApiKey = "";
string message = Console.ReadLine();
string aiResponse = await OpenAi(message);
Console.WriteLine(aiResponse);
async Task<string> OpenAi(string userMessage)
{
    string outputResult = "";
    var openai = new OpenAIAPI(openAiApiKey);
    CompletionRequest completionRequest = new CompletionRequest();
    completionRequest.Prompt = userMessage;
    completionRequest.MaxTokens = 1024;

    var completions = await openai.Completions.CreateCompletionAsync(completionRequest);

    foreach (var completion in completions.Completions)
    {
        outputResult += completion.Text;
    }
    return outputResult;
}