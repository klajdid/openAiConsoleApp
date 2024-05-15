using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAIConsole;

//TODO: Refactor methods adding a generic api calling as it is too repetitive

var assistantApiClient = new AssistantApiClient("enter-your-api-key");
var runId = await assistantApiClient.AskAssistant("thread_5OQ1DqkP5wNE7ssFlNcoZqRj", "asst_94uys37LdQoSH0Q14BEZxQsS", "1+1");
await assistantApiClient.GetResults("thread_5OQ1DqkP5wNE7ssFlNcoZqRj");
