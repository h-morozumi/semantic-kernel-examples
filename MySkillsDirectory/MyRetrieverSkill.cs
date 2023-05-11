using Microsoft.SemanticKernel.SkillDefinition;
using Newtonsoft.Json;
using System.Text;
using System.Net;

namespace MySkillsDirectory;

// Add your Bing Search V7 subscription key and endpoint to your environment variables


public class MyRetrieverSkill
{
    static string subscriptionKey = Environment.GetEnvironmentVariable("BING_SEARCH_V7_SUBSCRIPTION_KEY");
    static string endpoint = Environment.GetEnvironmentVariable("BING_SEARCH_V7_ENDPOINT") + "/v7.0/search";

    [SKFunction("Search for documents in Japanese relevant to your input and return 1 document")]
    public string Search(string input)
    {
        var json = BingSearch(input);
        Bing bing = JsonConvert.DeserializeObject<Bing>(json);
        Entity document = bing.WebPages.Value[0];
        string result = document.Snippet;
        Console.WriteLine("★★" + result);
        return result.Substring(0,1000);
    }

    String BingSearch(String query)
    {
        // Create a dictionary to store relevant headers
        Dictionary<String, String> relevantHeaders = new Dictionary<String, String>();

        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Searching the Web for: " + query);

        // Construct the URI of the search request
        var uriQuery = endpoint + "?q=" + Uri.EscapeDataString(query) + "&count=3&setLang=jp";

        // Perform the Web request and get the response
        WebRequest request = HttpWebRequest.Create(uriQuery);
        request.Headers["Ocp-Apim-Subscription-Key"] = subscriptionKey;
        HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
        string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

        // Extract Bing HTTP headers
        foreach (String header in response.Headers)
        {
            if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                relevantHeaders[header] = response.Headers[header];
        }

        // Show headers
        Console.WriteLine("Relevant HTTP Headers:");
        foreach (var header in relevantHeaders)
            Console.WriteLine(header.Key + ": " + header.Value);

        Console.WriteLine("JSON Response:");
        dynamic parsedJson = JsonConvert.DeserializeObject(json);
        Console.WriteLine(JsonConvert.SerializeObject(parsedJson, Formatting.Indented));

        return json;
    }

}

public class Bing
{
    public WebPages? WebPages { get; set; }
}
public class WebPages
{
    public string? WebSearchUrl { get; set; }
    public int TotalEstimatedMatches { get; set; }
    public List<Entity> Value { get; set; }
}

public class Entity
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public string? Snippet { get; set; }
}

