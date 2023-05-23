using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace SKDemos
{
    public class OpenAISettings
    {
        public string OpenAIKey { get; set; }
        public string OpenAIEndpoint { get; set; }
        public string OpenAIDeploymentName { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(OpenAIKey) &&
                !string.IsNullOrEmpty(OpenAIEndpoint) &&
                !string.IsNullOrEmpty(OpenAIDeploymentName);
        }

        public void AzureOpenAIInit()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            
            var secretProvider = config.Providers.First();
            secretProvider.TryGet("AzureOpenAI:Key", out var openAIKey);
            OpenAIKey = openAIKey;
            secretProvider.TryGet("AzureOpenAI:Base", out var openAIEndpoint);
            OpenAIEndpoint = openAIEndpoint;
            secretProvider.TryGet("AzureOpenAI:Deployment", out var openAIDeploymentName);
            OpenAIDeploymentName = openAIDeploymentName;
        }

        public IKernel BuildSKernel()
        {
            AzureOpenAIInit();
            
            var kernel = Kernel.Builder.Build();

            if (IsValid())
            {
                kernel.Config.AddAzureTextCompletionService(
                    OpenAIDeploymentName,
                    OpenAIEndpoint,
                    OpenAIKey);
            }
            else
            {
                Console.WriteLine("OpenAI settings are not valid.");
            }

            return kernel;
        }

    }
}