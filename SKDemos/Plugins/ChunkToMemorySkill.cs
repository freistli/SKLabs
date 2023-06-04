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
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace SKDemos;

public class ChunkToMemorySkill
{
    private const int MaxFileSize = 2000;
    public const int MaxLineTokens = 500;
     public const int MaxParagraphTokens = 2000;

    [SKFunction("Chunk content and emebedding to Memory Store ")]
    [SKFunctionInput(Description = "Content String")]
    [SKFunctionContextParameter(Name = "collection", Description = "Memory Collection Name")]
    [SKFunctionContextParameter(Name = "question", Description = "The question to be answered by memory skill recall")]
    public static async Task ChunkToMemoryAsync(string content, SKContext context)
    {
        if (content != null && content.Length > 0)
        {
            if (content.Length > MaxFileSize)
            {
                List<string> lines;
                List<string> paragraphs;

                lines = TextChunker.SplitMarkDownLines(content, MaxLineTokens);
                paragraphs = TextChunker.SplitMarkdownParagraphs(lines, MaxParagraphTokens);            

                for (int i = 0; i < paragraphs.Count; i++)
                {
                    await context.Memory.SaveInformationAsync(
                        $"{context["collection"]}",
                        text: $"{paragraphs[i]}",
                        description: $"File:{context["collection"]}",
                        id: $"{context["collection"]}_{i}");
                }
            }
            else
            {
                await context.Memory.SaveInformationAsync(
                    $"{context["collection"]}",
                    text: $"{content}",
                    description: $"File:{context["collection"]}",
                    id: $"{context["collection"]}_0");
            }

            context["input"] = context["question"];
        }
    }

    [SKFunction("Chunk content and emebedding to Memory Store ")]
    [SKFunctionInput(Description = "Content String")]
    [SKFunctionContextParameter(Name = "collection", Description = "Memory Collection Name")]
    [SKFunctionContextParameter(Name = "question", Description = "The question to be answered by memory skill recall")]
    public static async Task ChunkToQDrantAsync(string content, SKContext context)
    {
        if (content != null && content.Length > 0)
        {
            if (content.Length > MaxFileSize)
            {
                List<string> lines;
                List<string> paragraphs;

                lines = TextChunker.SplitMarkDownLines(content, MaxLineTokens);
                paragraphs = TextChunker.SplitMarkdownParagraphs(lines, MaxParagraphTokens);            

                for (int i = 0; i < paragraphs.Count; i++)
                {
                    await context.Memory.SaveInformationAsync(
                        $"{context["collection"]}",
                        text: $"{paragraphs[i]}",
                        description: $"File:{context["collection"]}",
                        id: Guid.NewGuid().ToString());
                }
            }
            else
            {
                var id1 = Guid.NewGuid().ToString();
                await context.Memory.SaveInformationAsync(
                    $"{context["collection"]}",
                    text: $"{content}",
                    description: $"File:{context["collection"]}",
                    id: id1);
            }

            context["input"] = context["question"];
        }
    }
    }
    