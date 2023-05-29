using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;
using SKDemos.plugins;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SKDemos
{
    public class Planner
    {
        private static async Task<Plan> ExecutePlanAsync(
        IKernel kernel,
        Plan plan,
        ContextVariables skContext,
        int maxSteps = 10)
        {
            Stopwatch sw = new();
            sw.Start();

            // loop until complete or at most N steps
            try
            {
                for (int step = 1; plan.HasNextStep && step < maxSteps; step++)
                {
                    if (skContext == null)
                    {
                        await plan.InvokeNextStepAsync(kernel.CreateNewContext());
                        // or await kernel.StepAsync(plan);
                    }
                    else
                    {
                        plan = await kernel.StepAsync(skContext, plan);
                    }

                    if (!plan.HasNextStep)
                    {
                        Console.WriteLine($"Step {step} - COMPLETE!");
                        Console.WriteLine(plan.State.ToString());
                        break;
                    }

                    Console.WriteLine($"Step {step} - Results so far:");
                    var s = plan.State.ToString();
                    Console.WriteLine((s.Length < 100) ? s : s.Substring(0, 100)+"...");
                }
            }
            catch (KernelException e)
            {
                Console.WriteLine($"Step - Execution failed:");
                Console.WriteLine(e.Message);
            }

            sw.Stop();
            Console.WriteLine($"Execution complete in {sw.ElapsedMilliseconds} ms!");
            return plan;
        }

        public static async Task DemoPlannerAsync(IKernel kernel)
        {
            Console.WriteLine("======== Sequential Planner - Check Device Update Version ========");

            var deviceVersionSkill = kernel.ImportSkill(new DeviceVersionSkill(), "DeviceVersionSkill");
            var SemanticPlugins = kernel.ImportSemanticSkillFromDirectory("Plugins", "SemanticPlugins");
            var httpSkill = kernel.ImportSkill(new HttpSkill(),"HttpSkill");

            var skContext = new ContextVariables();

            skContext.Set("input", "https://support.microsoft.com/en-us/surface/surface-book-update-history-3c36b18d-1261-2cfa-4ae8-67e1a84bb175");
            skContext.Set("devicename", "Intel(R) ICLS Client - Software devices");

            var planner = new SequentialPlanner(kernel);

            var plan = await planner.CreatePlanAsync("Read the web content from input, "+
                                                    "extract all device versions based on the devicename varaible, and then return the "+
                                                    "largest version number");

            Console.WriteLine("Original plan:");
            foreach (var step in plan.Steps)
            {
                Console.WriteLine(step.SkillName);
            }

            await Planner.ExecutePlanAsync(kernel, plan, skContext, 10);
        }
    }
}