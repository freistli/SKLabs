using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.AzureCognitiveSearch;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using SKDemos;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextEmbedding;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using System.Reflection;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure;

namespace SKDemos;

public class DocIndexRecord
{
    [SimpleField(IsKey = true, IsFilterable = false)]
    public string id { get; set; }
    public string content { get; set; }
    public object category { get; set; }
    public string sourcepage { get; set; }
    public string sourcefile { get; set; }

}
public class AzureCognitiveSearchMemoryExtend : AzureCognitiveSearchMemory
{
    public AzureCognitiveSearchMemoryExtend(string endpoint, string apiKey) : base(endpoint, apiKey)
    {
    }

    public string Base_NormalizeIndexName(string collection)
    {
        MethodInfo mI = null;
        Type baseType = this.GetType().BaseType;
        mI = baseType.GetMethod("NormalizeIndexName", BindingFlags.NonPublic | BindingFlags.Static);
        Console.WriteLine(mI.Name);
        var result = mI.Invoke(this, new object[] { collection });
        Console.WriteLine(result);
        return result.ToString();
    }

    public SearchClient Base_GetSearchClient(string collection)
    {
        MethodInfo mI = null;
        Type baseType = this.GetType().BaseType;
        mI = baseType.GetMethod("GetSearchClient", BindingFlags.NonPublic | BindingFlags.Instance);
        Console.WriteLine(mI.Name);
        var result = mI.Invoke(this, new object[] { collection });
        Console.WriteLine(result);
        return result as SearchClient;
    }
    private MemoryRecordMetadata ToMemoryRecordMetadata(DocIndexRecord data)
    {
        return new MemoryRecordMetadata(
            isReference: true,
            id: data.id,
            text: data.content ?? string.Empty,
            description: data.sourcepage ?? string.Empty,
            externalSourceName: data.sourcefile,
            additionalMetadata: Newtonsoft.Json.JsonConvert.SerializeObject(data) ?? string.Empty);
    }
    public async IAsyncEnumerable<MemoryQueryResult> SearchDocIndexAsync(string collection, string query,
        int limit = 1,
        double minRelevanceScore = 0.7,
        bool withEmbeddings = false,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        collection = Base_NormalizeIndexName(collection);

        var client = Base_GetSearchClient(collection);

        // TODO: use vectors
        var options = new SearchOptions
        {
            QueryType = SearchQueryType.Semantic,
            SemanticConfigurationName = "default",
            QueryLanguage = "en-us",
            Size = limit,
        };

        Response<SearchResults<DocIndexRecord>>? searchResult = null;
        try
        {
            searchResult = await client
                .SearchAsync<DocIndexRecord>(query, options, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (RequestFailedException e) when (e.Status == 404)
        {
            // Index not found, no data to return
        }

        if (searchResult != null)
        {
            await foreach (SearchResult<DocIndexRecord>? doc in searchResult.Value.GetResultsAsync())
            {
                if (doc.RerankerScore < minRelevanceScore) { break; }

                yield return new MemoryQueryResult(ToMemoryRecordMetadata(doc.Document), doc.RerankerScore ?? 1, null);
            }
        }


    }
}
