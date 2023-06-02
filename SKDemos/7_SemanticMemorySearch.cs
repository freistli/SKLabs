using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.AzureCognitiveSearch;
using Microsoft.SemanticKernel.Memory; 
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

using SKDemos;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextEmbedding;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Reliability;

/* The files contains two examples about SK Semantic Memory.
 *
 * 1. Memory using Azure Cognitive Search.
 * 2. Memory using a custom embedding generator and vector engine.
 *
 * Semantic Memory allows to store your data like traditional DBs,
 * adding the ability to query it using natural language.
 */

public static class SemanticMemory
{
    //when use ACS< this is the index name
    private const string MemoryCollectionName = "SKGithubSample";

    public static async Task DemoACSDocIndexQueryAsync(IKernel kernel, string query)
    {
        Console.WriteLine("==============================================================");
        Console.WriteLine("======== Semantic Memory using Azure Cognitive Search ========");
        Console.WriteLine("==============================================================");

        //The docindex has been built by other code, here we just query it
        await RunDocIndexAsync(kernel, query);  


        //Change AzureCognitiveSearchMemoryExtend ==> AzureCognitiveSearchMemory to use the default ACS connector
        //await RunSampleDataUpdateQueryAsync(kernel);

    }

    public static async Task DemoACSSampleDataQueryAsync(IKernel kernel)
    {
        Console.WriteLine("==============================================================");
        Console.WriteLine("======== Semantic Memory using Azure Cognitive Search ========");
        Console.WriteLine("==============================================================");

        //Use ACS, embedding is not required. Use MemoryRecordMetadata to create and query indexes on ACS
        await RunSampleDataUpdateQueryAsync(kernel);

    }

    public static async Task DemoEmbeddingyMemorySearchAsync(OpenAISettings settings)
    {
        var embeddingGenerator = new OpenAITextEmbeddingGeneration("text-embedding-ada-002", settings.OpenAIKey);

        var kernel = Kernel.Builder
            .WithLogger(ConsoleLogger.Log)
            .WithOpenAITextCompletionService("text-davinci-003", settings.OpenAIKey)
            .WithMemoryStorageAndTextEmbeddingGeneration(new VolatileMemoryStore(), embeddingGenerator)
            .Build();
        
        var retryConfig = new HttpRetryConfig() { MaxRetryCount = 8 };

        kernel.Config.SetDefaultHttpRetryConfig(retryConfig);        

        await RunWebContentUpdateQueryAsync(kernel);
    }

    public static async Task RunSampleDataUpdateQueryAsync(IKernel kernel)
    {
        await StoreMemoryAsync(kernel);

        await SearchMemoryAsync(kernel, MemoryCollectionName, "give me a summary");

     }

      public static async Task RunWebContentUpdateQueryAsync(IKernel kernel)
    {
        Console.WriteLine("\nAdding Web Content descriptions to the semantic memory.");
       var httpSkill = kernel.ImportSkill(new HttpSkill()); 
       var skContext = new ContextVariables();
       skContext.Set("input", "https://raw.githubusercontent.com/microsoft/semantic-kernel/main/README.md");

       var output = await kernel.RunAsync(skContext, httpSkill["GetAsync"]);

        await ChunkToMemory.RunAsync(kernel, output.Result, "rootbotReadMe");
        kernel.ImportSkill(new TextMemorySkill(), nameof(TextMemorySkill));
        var memoryQuerySkill = kernel.ImportSemanticSkillFromDirectory("Plugins", "SemanticPlugins");

        skContext.Set("input", "what's this file talking about?");
        skContext.Set(TextMemorySkill.CollectionParam, "rootbotReadMe");
        var result = await kernel.RunAsync(skContext, memoryQuerySkill["MemoryQuery"]);

        Console.WriteLine(result);

     }

     public static async Task RunDocIndexAsync(IKernel kernel, string query)
    {
        await SearchDocIndexMemoryAsync(kernel, "docindex", query);
     }

    private static async Task SearchDocIndexMemoryAsync(IKernel kernel, string index, string query)
    {
        Console.WriteLine("\nQuery: " + query + "\n");

        var memories = ((AzureCognitiveSearchMemoryExtend)kernel.Memory).SearchDocIndexAsync(index, query, limit: 2, minRelevanceScore: 0.5);

        int i = 0;
        await foreach (MemoryQueryResult memory in memories)
        {
            Console.WriteLine($"Result {++i}:");
            Console.WriteLine("  Page:     : " + memory.Metadata.Description);
            Console.WriteLine("  Content    : " + memory.Metadata.Text);
            Console.WriteLine();
        }

        Console.WriteLine("----------------------");
    }
    private static async Task SearchMemoryAsync(IKernel kernel, string index, string query)
    {
        Console.WriteLine("\nQuery: " + query + "\n");

        var memories = kernel.Memory.SearchAsync(index, query, limit: 2, minRelevanceScore: 0.5);

        int i = 0;
        await foreach (MemoryQueryResult memory in memories)
        {
            Console.WriteLine($"Result {++i}:");
            Console.WriteLine("  URL:     : " + memory.Metadata.Id);
            Console.WriteLine("  Title    : " + memory.Metadata.Description);
            Console.WriteLine();
        }

        Console.WriteLine("----------------------");
    }

    private static async Task StoreMemoryAsync(IKernel kernel)
    {
        /* Store some data in the semantic memory.
         *
         * When using Azure Cognitive Search the data is automatically indexed on write.
         *
         * When using the combination of VolatileStore and Embedding generation, SK takes
         * care of creating and storing the index
         */

        Console.WriteLine("\nAdding some GitHub file URLs and their descriptions to the semantic memory.");
        var githubFiles = SampleData();
        var i = 0;
        foreach (var entry in githubFiles)
        {
            await kernel.Memory.SaveReferenceAsync(
                collection: MemoryCollectionName,
                description: entry.Value,
                text: entry.Value,
                externalId: entry.Key,
                externalSourceName: "SampleGitHub"
            );
            Console.Write($" #{++i} saved.");
        }

        Console.WriteLine("\n----------------------");
    }

    private static Dictionary<string, string> SampleData()
    {
        return new Dictionary<string, string>
        {
            ["https://github.com/microsoft/semantic-kernel/blob/main/README.md"]
                = "README: Installation, getting started, and how to contribute",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/02-running-prompts-from-file.ipynb"]
                = "Jupyter notebook describing how to pass prompts from a file to a semantic skill or function",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/00-getting-started.ipynb"]
                = "Jupyter notebook describing how to get started with the Semantic Kernel",
            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/ChatSkill/ChatGPT"]
                = "Sample demonstrating how to create a chat skill interfacing with ChatGPT",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/SemanticKernel/Memory/VolatileMemoryStore.cs"]
                = "C# class that defines a volatile embedding store",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/dotnet/KernelHttpServer/README.md"]
                = "README: How to set up a Semantic Kernel Service API using Azure Function Runtime v4",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/chat-summary-webapp-react/README.md"]
                = "README: README associated with a sample chat summary react-based webapp",
        };
    }
}