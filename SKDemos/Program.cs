using SKDemos;
using Microsoft.SemanticKernel;
using System.Threading.Tasks;

OpenAISettings settings = new OpenAISettings();
settings.IsAzure = false;
settings.UseACSMemoryStore = true;
settings.UseMemoryStore = false;
settings.UseQDrant = false;
IKernel kernel = settings.BuildSKernel();

//await RawSummparyPrompt.DemoAsync(kernel);

//await ChainRawPrompts.DemoAsync(kernel);
//await CoreSkills.DemoTextSkillAsync(kernel);
//await CoreSkills.DemoHttpSkillAsync(kernel);
//await ChainCoreAndMySkills.DemoChainNativeFunctionAsync(kernel);
//await ChainCoreAndMySkills.DemoChainSemanticFunctionAsync(kernel);
//await ChainCoreAndMySkills.DemoSummarizeURL(kernel,"");
//await Planner.DemoPlannerAsync(kernel);

//6
//require UseMemoryStore as true or UseQDrant as true

//await SKConnectors.DemoConnectorsAsync(kernel);

//await WebSearchUrl.RunAsync();
//await ConversationSummary.RunAsync(kernel);

await SemanticMemory.DemoACSDocIndexQueryAsync(kernel,"");

//await SemanticMemory.DemoEmbeddingyMemorySearchAsync(settings);

//Console.ReadKey();

//var customSearch = new AzureCognitiveSearchMemoryExtend(settings.ACS_API_ENDPOINT, settings.ACS_API_KEY);
//customSearch.Base_GetSearchClient("docindex");

//await ChatGPT.RunAsync(kernel);

//await FunctionSummary.RunAsync(kernel);


