using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.KernelExtensions;

using MySkillsDirectory;

var endpoint = Environment.GetEnvironmentVariable("AOAI_ENDPOINT");
var deployname = Environment.GetEnvironmentVariable("AOAI_DEPLOYNAME");
var apikey = Environment.GetEnvironmentVariable("AOAI_APIKEY");

IKernel kernel = Microsoft.SemanticKernel.Kernel.Builder.Build();
// kernel.Config.AddOpenAITextCompletionService(
//     "davinci", "text-davinci-003", ""
// );
// For Azure Open AI details please see
// https://learn.microsoft.com/azure/cognitive-services/openai/quickstart?pivots=rest-api
kernel.Config.AddAzureOpenAITextCompletionService(
    "davinci-azure",                     // Alias used by the kernel
    deployname,                  // Azure OpenAI *Deployment ID*
    endpoint, // Azure OpenAI *Endpoint*
    apikey        // Azure OpenAI *Key*
);

// Plannerスキルを使用できるようにする。
var planner = kernel.ImportSkill(new PlannerSkill(kernel));

var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "MySkillsDirectory");
// Semantic Functionをインポート
kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "MyRetrieverSkill");
// Native Functionをインポート
kernel.ImportSkill(new MyRetrieverSkill(), "MyRetrieverSkill");

// var ask = "GPTをアシスタントとして使うための情報があるか検索して要約してください";
// var ask = "GPTに関して、アシスタントとして使う方法と、出力が事実か確かめる方法についてそれぞれ検索して結果をまとめて要約してください";
var ask = "織田信長について教えてください。";
var originalPlan = await kernel.RunAsync(ask, planner["CreatePlan"]);

Console.WriteLine("Original plan:\n");
Console.WriteLine(originalPlan.Variables.ToPlan().PlanString);

var executionResults = originalPlan;
int step = 1;
int maxSteps = 10;
while (!executionResults.Variables.ToPlan().IsComplete && step < maxSteps)
{
    var results = await kernel.RunAsync(executionResults.Variables, planner["ExecutePlan"]);
    if (results.Variables.ToPlan().IsSuccessful)
    {
        Console.WriteLine($"Step {step} - Execution results:\n");
        Console.WriteLine(results.Variables.ToPlan().PlanString);

        if (results.Variables.ToPlan().IsComplete)
        {
            Console.WriteLine($"Step {step} - COMPLETE!");
            Console.WriteLine(results.Variables.ToPlan().Result);
            break;
        }
    }
    else
    {
        Console.WriteLine($"Step {step} - Execution failed:");
        Console.WriteLine(results.Variables.ToPlan().Result);
        break;
    }
    
    executionResults = results;
    step++;
    Console.WriteLine("");
}