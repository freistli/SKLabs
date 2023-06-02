using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Skills.Document;
using Microsoft.SemanticKernel.Skills.Document.FileSystem;
using Microsoft.SemanticKernel.Skills.Document.OpenXml;
using SKDemos.plugins;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SKDemos
{
    //Use Word Document Connector to read text from a document, and summarize it.
    
    public class SKConnectors
    {
        public static async Task DemoConnectorsAsync(IKernel kernel)
        {
            DocumentSkill documentSkill = new(new WordDocumentConnector(), new LocalFileSystemConnector());
            var skill = kernel.ImportSkill(documentSkill, nameof(DocumentSkill));

            var chunkToMemorySkill = kernel.ImportSkill(new ChunkToMemorySkill(), nameof(ChunkToMemorySkill));

            var textMemorySkill = kernel.ImportSkill(new TextMemorySkill(), nameof(TextMemorySkill));

            var prompt = @"{{$input}}
            One line TLDR with the fewest words.";
            var summarize = kernel.CreateSemanticFunction(prompt);

            var skContext = new ContextVariables();
            skContext.Set("input", "c:\\testtemp\\data\\info.docx");
            skContext.Set(TextMemorySkill.CollectionParam, "localworddoc");
            skContext.Set("question", "give me a summary of the content");

            var result = await kernel.RunAsync( skContext,
            skill["ReadTextAsync"],
            chunkToMemorySkill["ChunkToMemoryAsync"],
            textMemorySkill["Recall"],
            summarize
            );

            Console.WriteLine(result);
        }
    }
}