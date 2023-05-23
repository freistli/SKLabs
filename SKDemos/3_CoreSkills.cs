using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using SKDemos.plugins;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SKDemos
{
    class CoreSkills
    {
        public static async Task DemoTextSkillAsync(IKernel kernel)
        {
            var textSkill = kernel.ImportSkill(new TextSkill());
            var skContext = new ContextVariables();
            skContext.Set("input", " hello world  ");
            var output = await kernel.RunAsync(skContext, textSkill["Trim"], textSkill["Uppercase"]);
            Console.WriteLine(output);
        }

        public static async Task DemoHttpSkillAsync(IKernel kernel)
        {
            var httpSkill = kernel.ImportSkill(new HttpSkill());
            var skContext = new ContextVariables();
            skContext.Set("input", "https://support.microsoft.com/en-us/surface/surface-book-update-history-3c36b18d-1261-2cfa-4ae8-67e1a84bb175");
            var output = await kernel.RunAsync(skContext, httpSkill["GetAsync"]);
            
            Console.WriteLine(output);
        }


 
    }
}