using Newtonsoft.Json;
using System.Text;

public class Searcher
{
    private static readonly HttpClient client = new HttpClient();
    private readonly string apiKey;
    private readonly Maritaca maritaca;

    public Searcher(string api_key, Maritaca maritacaInstance)
    {
        apiKey = api_key;
        client.BaseAddress = new Uri("https://api.tavily.com/");
        maritaca = maritacaInstance;
    }

    public async Task<string> search(string query, bool includeImages = false, string searchDepth = "basic")
    {
        var url = "/search";

        var requestBody = new
        {
            api_key = apiKey,
            query = query,
            include_images = includeImages,
            search_depth = searchDepth
        };

        var jsonBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            return JsonConvert.SerializeObject(new { error = response.StatusCode.ToString(), details = responseContent });
        }

        var result = JsonConvert.DeserializeObject<QueryResponse>(responseContent);
        if (result == null || result.results == null || !result.results.Any())
        {
            return JsonConvert.SerializeObject(new { error = "NoResults", details = "No results found or API returned empty response" });
        }

        var summarizationPrompt = $"Summarize the following search results for the query '{query}':\n\n{JsonConvert.SerializeObject(result, Formatting.Indented)}";
        return await maritaca.getCompletion(summarizationPrompt);
    }
}

public class Maritaca
{
    private static readonly HttpClient client = new HttpClient();
    public static int tokenLimit;

    public Maritaca(string apiKey, int limit)
    {
        var key = apiKey;
        tokenLimit = limit;
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
    }

    public async Task<string> getCompletion(string userMessage)
    {
        var url = "https://chat.maritaca.ai/api/chat/completions";

        var requestBody = new
        {
            model = "sabia-3",
            messages = new[] {
                new {role = "user", content = userMessage}
            },
            max_tokens = tokenLimit
        };

        var jsonBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Response>(responseContent);
        return result?.choices?[0]?.message?.content ?? "error: response incomplete";
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        string[] keys = File.ReadAllLines("config.txt");
        string maritacaKey = keys[0];
        string searcherKey = keys[1];
        var maritaca = new Maritaca(maritacaKey, 2048);
        var searcher = new Searcher(searcherKey, maritaca);

        while (true)
        {
            Console.WriteLine("Send your message (.search to search, .exit to exit): ");
            string? prompt = Console.ReadLine();
            if (prompt == ".exit" || string.IsNullOrEmpty(prompt)) break;

            if (prompt.StartsWith(".search"))
            {
                string searchQuery = prompt.Substring(7).Trim();
                var searchResult = await searcher.search(searchQuery);
                Console.WriteLine("Search results summary:");
                Console.WriteLine(searchResult);
            }
            else
            {
                var response = await maritaca.getCompletion(prompt);
                Console.WriteLine(response);
            }
        }
    }
}

public class Response
{
    public Choice[]? choices { get; set; }
}

public class Choice
{
    public Message? message { get; set; }
}

public class Message
{
    public string? content { get; set; }
}

public class QueryResponse
{
    public string? query { get; set; }
    public List<SearchResult>? results { get; set; }
    public string? topic { get; set; }
}

public class SearchResult
{
    public string? title { get; set; }
    public string? url { get; set; }
    public string? content { get; set; }
    public double? score { get; set; }
    public string? published_date { get; set; }
    public string? author { get; set; }
    public string? image_url { get; set; }
}
