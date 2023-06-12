using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.AzureCognitiveSearch;
using Microsoft.SemanticKernel.Connectors.Memory.Qdrant;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Reliability;

namespace SKDemos
{
    public enum MemoryStoreType
{
    ACSExtend,
    ACSDefault,
    Volatile,
    QDrant
}
    public class OpenAISettings
    {
        public string OpenAIKey { get; set; }
        public string OpenAIEndpoint { get; set; }
        public string OpenAIDeploymentName { get; set; }

        public string ACS_API_KEY { get; set; }
        public string ACS_API_ENDPOINT { get; set; }

        public bool IsAzure { get; set;}

        public IConfigurationRoot Config { get; private set; }
        public MemoryStoreType MyMemoryStoreType { get; set; }

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
           KernelBuilder builder = null;

           builder = Kernel.Builder.WithLogger(ConsoleLogger.Log);

           switch (MyMemoryStoreType)
            {
                case MemoryStoreType.ACSExtend:
                    ACSInit();
                    builder.WithMemory(new AzureCognitiveSearchMemoryExtend(ACS_API_ENDPOINT, ACS_API_KEY));
                    break;
                case MemoryStoreType.ACSDefault:
                    ACSInit();
                    builder.WithMemory(new AzureCognitiveSearchMemory(ACS_API_ENDPOINT, ACS_API_KEY));
                    break;
                case MemoryStoreType.Volatile:
                    builder.WithMemoryStorage(new VolatileMemoryStore());
                    break;
                case MemoryStoreType.QDrant:
                    QdrantMemoryStore memoryStore = new("http://localhost", 6333, vectorSize: 1536, ConsoleLogger.Log);
                    builder.WithMemoryStorage(memoryStore);
                    break;
                default:
                    throw new ArgumentException("Invalid memory store type specified.");
            }

            if (IsValid())
            {
                if(IsAzure)
                   builder.WithAzureTextCompletionService(
                    OpenAIDeploymentName,
                    OpenAIEndpoint,
                    OpenAIKey)
                    .WithAzureChatCompletionService("chatgpt",
                    OpenAIEndpoint,
                    OpenAIKey)
                    .WithAzureTextEmbeddingGenerationService("text-embedding-ada-002",
                    OpenAIEndpoint,
                    OpenAIKey);
                else
                    builder.WithOpenAITextCompletionService("text-davinci-003",
                    OpenAIKey)
                    .WithOpenAIChatCompletionService("gpt-3.5-turbo",OpenAIKey)
                    .WithOpenAITextEmbeddingGenerationService("text-embedding-ada-002",OpenAIKey);
                
                kernel = builder.Build();
                kernel.Config.SetDefaultHttpRetryConfig(retryConfig);
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