using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.AzureCognitiveSearch;
using Microsoft.SemanticKernel.Reliability;

namespace SKDemos
{
    public class OpenAISettings
    {
        public string OpenAIKey { get; set; }
        public string OpenAIEndpoint { get; set; }
        public string OpenAIDeploymentName { get; set; }

        public string ACS_API_KEY { get; set; }
        public string ACS_API_ENDPOINT { get; set; }

        public bool IsAzure { get; set;}

        public bool UseACS { get; set;}
        public IConfigurationRoot Config { get; private set; }

        public bool IsValid()
        {
            if (IsAzure)
                return !string.IsNullOrEmpty(OpenAIKey) &&
                    !string.IsNullOrEmpty(OpenAIEndpoint) &&
                    !string.IsNullOrEmpty(OpenAIDeploymentName);
            else
                return !string.IsNullOrEmpty(OpenAIKey);

        }

        public void ACSInit()
        {
            var secretProvider = Config.Providers.First();
            secretProvider.TryGet("ACS:Key", out var acsKey);
            ACS_API_KEY = acsKey;
            secretProvider.TryGet("ACS:Base", out var acsEndpoint);
            ACS_API_ENDPOINT = acsEndpoint;
        }

        public void AzureOpenAIInit()
        {
            Config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            
            var secretProvider = Config.Providers.First();
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
            Config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            
            var secretProvider = Config.Providers.First();
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

           IKernel kernel = null;

           if(UseACS)
           {
                ACSInit();
                kernel = Kernel.Builder.WithLogger(ConsoleLogger.Log).WithMemory(new AzureCognitiveSearchMemoryExtend(ACS_API_ENDPOINT, ACS_API_KEY)).Build();
           }
           else
               kernel = Kernel.Builder.WithLogger(ConsoleLogger.Log).Build();

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