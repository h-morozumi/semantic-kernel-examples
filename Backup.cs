using Microsoft.SemanticKernel;

class Backup
{
    public async void Sample()
    {
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

        // string summarizePrompt = @"
        // {{$input}}

        // Give me the a TLDR in 5 words.";

        string summarizePrompt = @"
{{$input}}

5つの単語で要約してください。";

        // string haikuPrompt = @"
        // {{$input}}

        // Write a futuristic haiku about it.";

        string haikuPrompt = @"
{{$input}}

未来的な俳句を書いてください。";

        var summarize = kernel.CreateSemanticFunction(summarizePrompt);
        var haikuWriter = kernel.CreateSemanticFunction(haikuPrompt);

        // string inputText = @"
        // 1) A robot may not injure a human being or, through inaction,
        // allow a human being to come to harm.

        // 2) A robot must obey orders given it by human beings except where
        // such orders would conflict with the First Law.

        // 3) A robot must protect its own existence as long as such protection
        // does not conflict with the First or Second Law.";

        string inputText = @"
1) ロボットは人間を傷つけてはならず、また、何もしないことによって人間が危害を被ることを許してはなりません。

2) ロボットは、その命令が第一法則と矛盾しない限り、人間によって与えられた命令に従わなければなりません。

3) ロボットは、その保護が第一法則または第二法則と矛盾しない限り、自己の存在を守らなければなりません。";

        var output = await kernel.RunAsync(inputText, summarize, haikuWriter);

        Console.WriteLine(output);

        // Output => Robots protect us all
        //           No harm to humans they bring
        //           Peaceful coexistence
    }
}



