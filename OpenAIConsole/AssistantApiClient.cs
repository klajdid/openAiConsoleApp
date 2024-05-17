namespace OpenAIConsole;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class AssistantApiClient
{
    private readonly HttpClient _httpClient;

    public AssistantApiClient(string apiKey)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> CreateAssistant(string instructions, string name, string model)
    {
        var assistantData = new
        {
            instructions,
            name,
            model
        };

        var content = new StringContent(JsonConvert.SerializeObject(assistantData), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/assistants", content);

        var responseContent = await response.Content.ReadAsStringAsync();
        JObject result = (JObject)JsonConvert.DeserializeObject(responseContent);

        return result.GetValue("id").ToString();
    }

    public async Task<JObject> GetAssistants()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("https://api.openai.com/v1/assistants");
        var responseContent = await response.Content.ReadAsStringAsync();
        return (JObject)JsonConvert.DeserializeObject(responseContent);
    }
    public async Task<JObject> GetAssistantById(string assistantId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"https://api.openai.com/v1/assistants/{assistantId}");
        var responseContent = await response.Content.ReadAsStringAsync();
        return (JObject)JsonConvert.DeserializeObject(responseContent);
    }

    public async Task<string> CreateThread(string assistantId)
    {
        var requestData = new
        {
            assistant_id = assistantId
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/threads", null);

        var responseContent = await response.Content.ReadAsStringAsync();
        JObject result = (JObject)JsonConvert.DeserializeObject(responseContent);

        return result.GetValue("id").ToString();
    }
    
    public async Task<string> CheckStatus(string threadId, string runId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/runs/{runId}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = (JObject)JsonConvert.DeserializeObject(responseContent);
        return result.GetValue("status").ToString();

    }

    public async Task<string> AskAssistant(string threadId, string assistantId, string question)
    {
        var messageData = new
        {
            role = "user",
            content = question
        };

        var content = new StringContent(JsonConvert.SerializeObject(messageData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{threadId}/messages", content);
        // var response = await _httpClient.PostAsync("https://api.openai.com/v1/threads", null);
        var responseContent = await response.Content.ReadAsStringAsync();
        JObject result = (JObject)JsonConvert.DeserializeObject(responseContent);

        var runData = new
        {
            assistant_id = assistantId
        };

        content = new StringContent(JsonConvert.SerializeObject(runData), Encoding.UTF8, "application/json");
        response = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{threadId}/runs", content);
        responseContent = await response.Content.ReadAsStringAsync();
        result = (JObject)JsonConvert.DeserializeObject(responseContent);

        return result.GetValue("id").ToString();
    }

    public async Task<JObject> GetResults(string threadId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages");
        var responseContent = await response.Content.ReadAsStringAsync();
        // Console.WriteLine(responseContent);
        return (JObject)JsonConvert.DeserializeObject(responseContent);
    }
}
