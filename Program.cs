using Microsoft.SemanticKernel;

var kernel = Kernel.Builder.Build();

var endpoint = Environment.GetEnvironmentVariable("AOAI_ENDPOINT");
var deployname = Environment.GetEnvironmentVariable("AOAI_DEPLOYNAME");
var apikey = Environment.GetEnvironmentVariable("AOAI_APIKEY");

// For Azure Open AI details please see
// https://learn.microsoft.com/azure/cognitive-services/openai/quickstart?pivots=rest-api
kernel.Config.AddAzureOpenAITextCompletionService(
    "davinci-azure",                     // Alias used by the kernel
    deployname,                  // Azure OpenAI *Deployment ID*
    endpoint, // Azure OpenAI *Endpoint*
    apikey        // Azure OpenAI *Key*
);

string summarizePrompt = @"
{{$input}}

Give me the a TLDR in 5 words.";

string haikuPrompt = @"
{{$input}}

Write a futuristic haiku about it.";

var summarize = kernel.CreateSemanticFunction(summarizePrompt);
var haikuWriter = kernel.CreateSemanticFunction(haikuPrompt);

string inputText = @"
1) A robot may not injure a human being or, through inaction,
allow a human being to come to harm.

2) A robot must obey orders given it by human beings except where
such orders would conflict with the First Law.

3) A robot must protect its own existence as long as such protection
does not conflict with the First or Second Law.";

var output = await kernel.RunAsync(inputText, summarize, haikuWriter);

Console.WriteLine(output);

// Output => Robots protect us all
//           No harm to humans they bring
//           Peaceful coexistence