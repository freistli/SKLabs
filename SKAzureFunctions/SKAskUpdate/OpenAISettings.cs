using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace SKAzureFunctions
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
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().AddEnvironmentVariables().Build();
            
            var secretProvider = config.Providers.First();
            secretProvider.TryGet("AzureOpenAI:Key", out var openAIKey);
            OpenAIKey = openAIKey;
            secretProvider.TryGet("AzureOpenAI:Base", out var openAIEndpoint);
            OpenAIEndpoint = openAIEndpoint;
            secretProvider.TryGet("AzureOpenAI:Deployment", out var openAIDeploymentName);
            OpenAIDeploymentName = openAIDeploymentName;

            if (!IsValid())
            {
                OpenAIKey = Environment.GetEnvironmentVariable("AzureOpenAI:Key");
                OpenAIEndpoint = Environment.GetEnvironmentVariable("AzureOpenAI:Base");
                OpenAIDeploymentName = Environment.GetEnvironmentVariable("AzureOpenAI:Deployment");
            }
        }
        public IKernel BuildSKernel(ILogger logger)
        {
            AzureOpenAIInit();
            var kernel = Kernel.Builder.Build();

            if (IsValid())
            {
                kernel.Config.AddAzureTextCompletionService(
                    OpenAIDeploymentName,
                    OpenAIEndpoint,
                    OpenAIKey);
                logger.LogInformation("OpenAI settings are valid.");
            }
            else
            {
                logger.LogError("OpenAI settings are not valid.");
            }

            return kernel;
        }

    }
}