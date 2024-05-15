using System;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI.Assistants;

namespace OpenAIConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize Azure OpenAI service client
            var client = new AssistantsClient("sk-proj-UlvsEOSFMOyEsfMEEwAET3BlbkFJpcwFQmzY5OvSaT1khXkN");

            // Prompt the user to enter the file path
            Console.WriteLine("Please provide the path to the file:");
            string filePath = Console.ReadLine();

            // Check if a file path is provided
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("Invalid file path or file does not exist.");
                return;
            }

            // Define options for creating an assistant
            var assistantCreationOptions = new AssistantCreationOptions("gpt-3.5-turbo-0125")
            {
                Name = "Health assistant",
                Instructions = "You are a health assistant, answer questions from the user about the provided file. ",
                Tools = { new RetrievalToolDefinition() },
            };

            // Upload the file to the Azure OpenAI service
            var fileUploadResponse = await client.UploadFileAsync(filePath, OpenAIFilePurpose.Assistants);
            assistantCreationOptions.FileIds.Add(fileUploadResponse.Value.Id);
            Console.WriteLine($"Uploaded file {Path.GetFileName(filePath)}");


            // Update instructions to include information about the uploaded file
            assistantCreationOptions.Instructions += $" The file with id {fileUploadResponse.Value} " +
            $"has a original filename of {Path.GetFileName(filePath)} and is " +
            $" a {Path.GetExtension(filePath).Replace(".", string.Empty)} file.";

            // Create an assistant instance
            var assistant = await client.CreateAssistantAsync(assistantCreationOptions);

            // Create a new conversation thread
            var thread = await client.CreateThreadAsync();

            // Loop for handling user interactions until the user decides to quit
            while (true)
            {
                // Prompt user to ask questions about the file
                Console.WriteLine("Ask a question about the file (empty response to quit):");
                var question = Console.ReadLine();

                // Check if user wants to quit
                if (string.IsNullOrWhiteSpace(question))
                    break;

                string? lastMessageId = null;

                // Send user question to the assistant
                await client.CreateMessageAsync(thread.Value.Id, MessageRole.User, question);

                // Start a new run with the assistant
                var run = await client.CreateRunAsync(thread.Value.Id, new CreateRunOptions(assistant.Value.Id));
                Response<ThreadRun> runResponse;

                // Poll for the completion of the assistant's response
                do
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1000));
                    runResponse = await client.GetRunAsync(thread.Value.Id, run.Value.Id);
                    Console.Write(".");
                } while (runResponse.Value.Status == RunStatus.Queued
                        || runResponse.Value.Status == RunStatus.InProgress);

                Console.WriteLine(string.Empty);

                // Retrieve messages exchanged in the conversation thread
                var messageResponse = await client.GetMessagesAsync(thread.Value.Id, order: ListSortOrder.Ascending, after: lastMessageId);

                // Print assistant's response to the user's question
                foreach (var message in messageResponse.Value.Data)
                {
                    lastMessageId = message.Id;
                    foreach (var content in message.ContentItems)
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
            }

            // Clean up: Delete the uploaded file, conversation thread, and assistant
            Console.WriteLine("Cleaning up and exiting...");
            await client.DeleteFileAsync(fileUploadResponse.Value.Id);
            await client.DeleteThreadAsync(thread.Value.Id);
            await client.DeleteAssistantAsync(assistant.Value.Id);
        }
    }
}
