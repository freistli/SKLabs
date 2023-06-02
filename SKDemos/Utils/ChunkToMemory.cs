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
using Microsoft.SemanticKernel.Text;

namespace SKDemos;

public class ChunkToMemory
{
    private const int MaxFileSize = 2048;
    public const int MaxTokens = 1000;

    public static async Task RunAsync(IKernel kernel, string content, string fileUri)
    {
        if (content != null && content.Length > 0)
        {
            if (content.Length > MaxFileSize)
            {
                List<string> lines;
                List<string> paragraphs;

                lines = TextChunker.SplitMarkDownLines(content, MaxTokens);
                paragraphs = TextChunker.SplitMarkdownParagraphs(lines, MaxTokens);            

                for (int i = 0; i < paragraphs.Count; i++)
                {
                    await kernel.Memory.SaveInformationAsync(
                        $"{fileUri}",
                        text: $"{paragraphs[i]}",
                        description: $"File:{fileUri}",
                        id: $"{fileUri}_{i}");
                }
            }
            else
            {
                await kernel.Memory.SaveInformationAsync(
                    $"{fileUri}",
                    text: $"{content}",
                    description: $"File:{fileUri}",
                    id: $"{fileUri}");
            }
        }
    }
    }
 