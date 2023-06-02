using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using SKDemos.plugins;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SKDemos
{
    class ChainCoreAndMySkills
    {
        public static async Task DemoChainNativeFunctionAsync(IKernel kernel)
        {
            var httpSkill = kernel.ImportSkill(new HttpSkill());
            var skContext = new ContextVariables();

            skContext.Set("input", "https://support.microsoft.com/en-us/surface/surface-book-update-history-3c36b18d-1261-2cfa-4ae8-67e1a84bb175");
            skContext.Set("devicename", "Intel(R) ICLS Client - Software devices");

            var deviceVersionSkill = kernel.ImportSkill(new DeviceVersionSkill(), "DeviceVersionSkill");
            var output = await kernel.RunAsync(skContext, httpSkill["GetAsync"], deviceVersionSkill["GetAvailableVersions"]);
            
            Console.WriteLine(output);
        }
        public static async Task DemoChainSemanticFunctionAsync(IKernel kernel)
        {
            var httpSkill = kernel.ImportSkill(new HttpSkill());
            var skContext = new ContextVariables();

            skContext.Set("input", "https://support.microsoft.com/en-us/surface/surface-book-update-history-3c36b18d-1261-2cfa-4ae8-67e1a84bb175");
            skContext.Set("devicename", "Intel(R) ICLS Client - Software devices");

            var deviceVersionSkill = kernel.ImportSkill(new DeviceVersionSkill(), "DeviceVersionSkill");
            var SemanticPlugins = kernel.ImportSemanticSkillFromDirectory("Plugins", "SemanticPlugins");

            var output = await kernel.RunAsync(skContext, httpSkill["GetAsync"], deviceVersionSkill["GetAvailableVersions"], SemanticPlugins["FindLatestVersion"]);

            Console.WriteLine(output);
        }

        public static async Task DemoSummarizeURL(IKernel kernel, string uri)
        {
            var httpSkill = kernel.ImportSkill(new HttpSkill());
            var SemanticPlugins = kernel.ImportSemanticSkillFromDirectory("Plugins", "SemanticPlugins");
            var skContext = new ContextVariables();
            skContext.Set("input", uri);

            var output = await kernel.RunAsync(skContext, httpSkill["GetAsync"], SemanticPlugins["SummarizeContent"]);

            Console.WriteLine(output);

        }


    }
}