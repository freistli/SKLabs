using SKDemos;
using Microsoft.SemanticKernel;
using System.Threading.Tasks;

OpenAISettings settings = new OpenAISettings();
settings.IsAzure = false;
settings.UseACS = false;
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
//await ConversationSummary.RunAsync(kernel);

 //await SemanticMemory.RunACSDocIndexQueryAsync(kernel,"How to apply vacation leave?");

await SemanticMemory.RunEmbeddingySearchAsync(settings);
//Console.ReadKey();

//var customSearch = new AzureCognitiveSearchMemoryExtend(settings.ACS_API_ENDPOINT, settings.ACS_API_KEY);
//customSearch.Base_GetSearchClient("docindex");


//await FunctionSummary.RunAsync(kernel);


