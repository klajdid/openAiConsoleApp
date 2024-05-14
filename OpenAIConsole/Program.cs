// Import necessary namespaces
using Azure;
using Azure.AI.OpenAI.Assistants;

// Initialize Azure OpenAI service client
var client = new AssistantsClient(new Uri("https://api.openai.com/v1/assistants"), 
new AzureKeyCredential("openai-api-key"));

// Check if a file path is provided as an argument
if (args.Length < 1) {
    Console.WriteLine("Provide a file path as the first argument");
    return;
}

// Define options for creating an assistant
var assistantCreationOptions = new AssistantCreationOptions("gpt-4")
{
    Name = "File question answerer",
    Instructions = "Answer questions from the user about the provided file. " +
    "For PDF files, immediately use PyPDF2 to extract the text contents and answer quesions based on that.",
    Tools = { new CodeInterpreterToolDefinition() },
};

// Upload the file to the Azure OpenAI service
var fileUploadResponse = await client.UploadFileAsync(args[0], OpenAIFilePurpose.Assistants);
assistantCreationOptions.FileIds.Add(fileUploadResponse.Value.Id);
Console.WriteLine($"Uploaded file {fileUploadResponse.Value.Filename}");

// Update instructions to include information about the uploaded file
assistantCreationOptions.Instructions += $" The file with id {fileUploadResponse.Value.Id} " +
$"has a original filename of {Path.GetFileName(args[0])} and is " +
$" a {Path.GetExtension(args[0]).Replace(".",string.Empty)} file.";

// Create an assistant instance
var assistant = await client.CreateAssistantAsync(assistantCreationOptions);

// Create a new conversation thread
var thread = await client.CreateThreadAsync();

// Prompt user to ask questions about the file
Console.WriteLine("Ask a question about the file (empty response to quit):");
var question = Console.ReadLine();

// Loop for handling user interactions until the user decides to quit
while (!string.IsNullOrWhiteSpace(question))
{
    string? lastMessageId = null;

    // Send user question to the assistant
    await client.CreateMessageAsync(thread.Value.Id, MessageRole.User, question);
    
    // Start a new run with the assistant
    var run = await client.CreateRunAsync(thread.Value.Id, new CreateRunOptions(assistant.Value.Id));
    Response<ThreadRun> runResponse;

    // Poll for the completion of the assistant's response
    do {
        await Task.Delay(TimeSpan.FromMilliseconds(1000));
        runResponse = await client.GetRunAsync(thread.Value.Id, run.Value.Id);
        Console.Write(".");
    } while (runResponse.Value.Status == RunStatus.Queued
            || runResponse.Value.Status == RunStatus.InProgress);
    
    Console.WriteLine(string.Empty);

    // Retrieve messages exchanged in the conversation thread
    var messageResponse = await client.GetMessagesAsync(thread.Value.Id, order: ListSortOrder.Ascending, after: lastMessageId);

    // Print assistant's response to the user's question
    foreach(var message in messageResponse.Value.Data)
    {
        lastMessageId = message.Id;
        foreach(var content in message.ContentItems)
        {
            if (content is MessageTextContent textContent)
            {
                if (textContent.Text != question)
                {
                    Console.WriteLine(textContent.Text);
                }
                
            }
        }
    }

    // Prompt user for another question or to quit
    Console.WriteLine("Your response: (leave empty to quit)");
    question = Console.ReadLine();

}

// Clean up: Delete the uploaded file, conversation thread, and assistant
Console.WriteLine("Cleaning up and exiting...");
await client.DeleteFileAsync(fileUploadResponse.Value.Id);
await client.DeleteThreadAsync(thread.Value.Id);
await client.DeleteAssistantAsync(assistant.Value.Id);
