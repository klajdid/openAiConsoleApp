using Newtonsoft.Json.Linq;
using OpenAIConsole;

String name = "testAssist";
// String assistantId = "asst_KQLFmyDt3GE1JFFpYHp9GUsM"; mauro assistant
String assistantId = "asst_t5PHtvieZ4hRqxtOWcTMnZVN";
String threadId = "thread_quGXKsRp7ThHayDRheQMkZS2";
string question = Console.ReadLine() ?? "";
string status = "";

var assistantApiClient = new AssistantApiClient("ENTER-YOUR-API-KEY-HERE");

// var assId = await assistantApiClient.CreateAssistant("when i ask you questions add to your answer in the end three dots and one question mark only when the question is in italian.", "testAssistant", "gpt-3.5-turbo");
// Console.WriteLine(assId);
// var threadId = await assistantApiClient.CreateThread(assistantId);
// Console.WriteLine(threadId);
var runId = await assistantApiClient.AskAssistant(threadId, assistantId, question);
do
{
   status = await assistantApiClient.CheckStatus(threadId, runId);
   // Console.WriteLine(status);
} while (!status.Equals("completed") && !status.Equals("failed"));

if (status.Equals("completed"))
{
   var result = await assistantApiClient.GetResults(threadId);//quando mi devo vaccinare per l'epatite?
   var jsonObject = JObject.Parse(result.ToString());
   
   string value = (string)jsonObject["data"][0]["content"][0]["text"]["value"];
   Console.WriteLine(value); 
}
else
{
   Console.WriteLine("Something went wrong!");
}

#region FileUpload
// string key = "";
// string endpoint = "https://api.openai.com/v1/files"; //(Looks like: "https://Cazton.openai.azure.com/" )
// string engine = "gpt-3.5-turbo";
// // Configure OpenAI API client
// OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key)); 
//
// string pdfFilePath = "C:\\Users\\User\\Downloads\\21578-101367-4-PB.pdf";
// StringBuilder fullText = new StringBuilder();
//  
// using (PdfDocument pdf = PdfDocument.Open(pdfFilePath))
// {
//     for (int i = 0; i < pdf.NumberOfPages; i++)
//     {
//         Page page = pdf.GetPage(i + 1);
//         fullText.Append(page.Text);
//     }
// }
//  
// // Set up GPT-3 model and prompt
// string modelEngine = "davinci";
// string prompt = $"What is the answer to the following question regarding the PDF document?\n\n{fullText}\n\n";
//  
// // Ask questions and get answers
// string[] questions = { "What is the document about?", "Who wrote the document?", "What is the main idea of the document?" };
//  
// foreach (string question in questions)
// {
//     string fullPrompt = question;
//     CompletionsOptions completionsOptions = new()
//     {
//         MaxTokens = 100
//     };
//    
// }
// CompletionsOptions completionRequest = new CompletionsOptions();
// completionRequest.DeploymentName = "davinci";
// completionRequest.MaxTokens = 1024;
// Response<Completions> completionsResponse = await client.GetCompletionsAsync(completionRequest);
//  
// string answer = completionsResponse.Value.Choices[0].Text;
// Console.WriteLine($"{questions}\n{answer}\n");

#endregion

