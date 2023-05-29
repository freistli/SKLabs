using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Reliability;

namespace SKDemos
{
    public class OpenAISettings
    {
        public string OpenAIKey { get; set; }
        public string OpenAIEndpoint { get; set; }
        public string OpenAIDeploymentName { get; set; }

        public bool IsAzure { get; set;}

        public bool IsValid()
        {
            if (IsAzure)
                return !string.IsNullOrEmpty(OpenAIKey) &&
                    !string.IsNullOrEmpty(OpenAIEndpoint) &&
                    !string.IsNullOrEmpty(OpenAIDeploymentName);
            else
                return !string.IsNullOrEmpty(OpenAIKey);

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

            if (!IsValid())
            {
                OpenAIKey = Environment.GetEnvironmentVariable("AzureOpenAI:Key");
                OpenAIEndpoint = Environment.GetEnvironmentVariable("AzureOpenAI:Base");
                OpenAIDeploymentName = Environment.GetEnvironmentVariable("AzureOpenAI:Deployment");
            }
        }

        public void OpenAIInit()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            
            var secretProvider = config.Providers.First();
            secretProvider.TryGet("OpenAI:Key", out var openAIKey);
            OpenAIKey = openAIKey;             

            if (!IsValid())
            {
                OpenAIKey = Environment.GetEnvironmentVariable("OpenAI:Key");
            }
        }

        public void DisplayHttpRetryConfig(IKernel kernel)
        {            
            Console.WriteLine("MinRetryDelay {0}", kernel.Config.DefaultHttpRetryConfig.MinRetryDelay);
            Console.WriteLine("MaxRetryDelay {0}", kernel.Config.DefaultHttpRetryConfig.MaxRetryDelay);
            Console.WriteLine("MaxRetryCount {0}", kernel.Config.DefaultHttpRetryConfig.MaxRetryCount);
            Console.WriteLine("UseExponentialBackoff {0}", kernel.Config.DefaultHttpRetryConfig.UseExponentialBackoff);
        }

        public IKernel BuildSKernel()
        {

            if (IsAzure)
                AzureOpenAIInit();
            else
                OpenAIInit();

           var retryConfig = new HttpRetryConfig() { MaxRetryCount = 5, UseExponentialBackoff = true };
                       
           var kernel = Kernel.Builder.WithLogger(ConsoleLogger.Log).Build();

           DisplayHttpRetryConfig(kernel);

            if (IsValid())
            {
                if(IsAzure)
                    kernel.Config.SetDefaultHttpRetryConfig(retryConfig).AddAzureTextCompletionService(
                    OpenAIDeploymentName,
                    OpenAIEndpoint,
                    OpenAIKey);
                else
                    kernel.Config.SetDefaultHttpRetryConfig(retryConfig).AddOpenAITextCompletionService("text-davinci-003",
                    OpenAIKey);
                
                DisplayHttpRetryConfig(kernel);
            }
            else
            {
                Console.WriteLine("OpenAI settings are not valid.");
            }

            return kernel;
        }

    }
}