using SKDemos;
using Microsoft.SemanticKernel;
using System.Threading.Tasks;

OpenAISettings settings = new OpenAISettings();
settings.IsAzure = false;
 
settings.MyMemoryStoreType = MemoryStoreType.Volatile;

IKernel kernel = settings.BuildSKernel();

//await RawSummparyPrompt.DemoAsync(kernel);
//await ChainRawPrompts.DemoAsync(kernel);
//await CoreSkills.DemoTextSkillAsync(kernel);
//await CoreSkills.DemoHttpSkillAsync(kernel);
//await ChainCoreAndMySkills.DemoChainNativeFunctionAsync(kernel);
//await ChainCoreAndMySkills.DemoChainSemanticFunctionAsync(kernel);
//await ChainCoreAndMySkills.DemoSummarizeURL(kernel,"");

//5.
//await Planner.DemoPlannerAsync(kernel);

//6.
//require UseMemoryStore as true or UseQDrant as true
//await SKConnectors.DemoConnectorsAsync(kernel,"ChunkToQDrantAsync");
//await SKConnectors.DemoConnectorsAsync(kernel,"ChunkToMemoryAsync");

//await WebSearchUrl.RunAsync();
//await ConversationSummary.RunAsync(kernel);

//7.
//require UseACSMemoryStore as true
//Console.InputEncoding = System.Text.Encoding.Unicode;
//Console.OutputEncoding = System.Text.Encoding.Unicode;
//await SemanticMemory.DemoACSDocIndexQueryAsync(kernel,"","");
await SemanticMemory.DemoEmbeddingyMemorySearchAsync(settings);

//7
//await SemanticMemory.DemoEmbeddingyMemorySearchAsync(settings);

//Console.ReadKey();

//var customSearch = new AzureCognitiveSearchMemoryExtend(settings.ACS_API_ENDPOINT, settings.ACS_API_KEY);
//customSearch.Base_GetSearchClient("docindex");

//await ChatGPT.RunAsync(kernel);

//await FunctionSummary.RunAsync(kernel);

//await SKTokens.DemoAsync();


