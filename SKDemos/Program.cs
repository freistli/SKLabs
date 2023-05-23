using SKDemos;
using Microsoft.SemanticKernel;
using System.Threading.Tasks;

OpenAISettings settings = new OpenAISettings();

IKernel kernel = settings.BuildSKernel();

//await RawSummparyPrompt.DemoAsync(kernel);

//await ChainRawPrompts.DemoAsync(kernel);

await CoreSkills.DemoTextSkillAsync(kernel);
await CoreSkills.DemoHttpSkillAsync(kernel);
await ChainCoreAndMySkills.DemoAsync(kernel);
