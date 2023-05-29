using SKDemos;
using Microsoft.SemanticKernel;
using System.Threading.Tasks;

OpenAISettings settings = new OpenAISettings();
settings.IsAzure = false;
IKernel kernel = settings.BuildSKernel();

//await RawSummparyPrompt.DemoAsync(kernel);

//await ChainRawPrompts.DemoAsync(kernel);
//await CoreSkills.DemoTextSkillAsync(kernel);
//await CoreSkills.DemoHttpSkillAsync(kernel);
//await ChainCoreAndMySkills.DemoChainNativeFunctionAsync(kernel);
//await ChainCoreAndMySkills.DemoChainSemanticFunctionAsync(kernel);
//await Planner.DemoPlannerAsync(kernel);
//await SKConnectors.DemoConnectorsAsync(kernel);
//await WebSearchUrl.RunAsync();
await ConversationSummary.RunAsync(kernel);

//Console.ReadKey();

//await FunctionSummary.RunAsync(kernel);


