using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//TODO: Refactor methods adding a generic api calling as it is too repetitive
    var runId = await AskAssistant();
    await GetResults();

    // Call the api to craete the assistant
    async Task CreateAssistant(){
        var assistantData = new
        {
            instructions = "You are created for test assistant feature. When someone ask you 1+1=? you need to answer 5.",
            name = "AssisName",
            model = "gpt-3.5-turbo"
        };

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            "");
        httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var content = new StringContent(JsonConvert.SerializeObject(assistantData), Encoding.UTF8, "application/json");


        HttpResponseMessage response = await httpClient.PostAsync("https://api.openai.com/v1/assistants", content);

        var responseContent = await response.Content.ReadAsStringAsync();
        JObject result = (JObject)JsonConvert.DeserializeObject(responseContent);
    }

    //Call api to get assistants
    async Task GetAssistants(){
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            "");
        httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        HttpResponseMessage response =
            await httpClient.GetAsync($"https://api.openai.com/v1/assistants/asst_poDavzZesWFpmSCwYqJAvBhl");
    }

    //Call api to create threads
    async Task<string> CreateThread(){
        var requestData = new
        {
            assistant_id = "asst_94uys37LdQoSH0Q14BEZxQsS"
        };
    
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            "");
        httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("https://api.openai.com/v1/threads", null);
        var responseContent = await response.Content.ReadAsStringAsync();
        JObject result = (JObject)JsonConvert.DeserializeObject(responseContent);
        Console.WriteLine(response);
        return result.GetValue("id").ToString();
    }
    
    //call api to check status of a specific run
    async Task CheckStatus(){
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            "");
        httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response =
            await httpClient.GetAsync($"https://api.openai.com/v1/threads/thread_5OQ1DqkP5wNE7ssFlNcoZqRj/runs/{runId}");
        var responseContent = await response.Content.ReadAsStringAsync();
        JObject result = (JObject)JsonConvert.DeserializeObject(responseContent);
    }
    
    //call api to make a quesiton to the assistant
    async Task<string> AskAssistant()
    {
        var messageData = new
        {
            role = "user",
            content = "1+1"
        };
    
        var _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
        _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var content = new StringContent(JsonConvert.SerializeObject(messageData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/threads", null);
        var responseContent = await response.Content.ReadAsStringAsync();
        JObject result = (JObject)JsonConvert.DeserializeObject(responseContent);
    
        var runData = new
        {
            assistant_id = "asst_94uys37LdQoSH0Q14BEZxQsS"
        };
    
        content = new StringContent(JsonConvert.SerializeObject(runData), Encoding.UTF8, "application/json");
        response = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/thread_5OQ1DqkP5wNE7ssFlNcoZqRj/runs", content);
        responseContent = await response.Content.ReadAsStringAsync();
        result = (JObject)JsonConvert.DeserializeObject(responseContent);
    
        return result.GetValue("id").ToString();
    }
    
    //call api to get the result from the question asked
    async Task GetResults() {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            "");
        httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    
        var response =
            await httpClient.GetAsync($"https://api.openai.com/v1/threads/thread_5OQ1DqkP5wNE7ssFlNcoZqRj/messages");
        var responseContent = await response.Content.ReadAsStringAsync();
        JObject result = (JObject)JsonConvert.DeserializeObject(responseContent);
        Console.WriteLine(result);
    }