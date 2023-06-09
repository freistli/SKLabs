# SKDemos

## Test Locally

### Use Azure OpenAI
```
cd SKDemos
```

Modify Program.cs

```
OpenAISettings settings = new OpenAISettings();
settings.IsAzure = true;
IKernel kernel = settings.BuildSKernel();
```
Set User Secrets
```
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:Key" "b6351ddxxxxxxxxxxxxxxxxxe680a"
dotnet user-secrets set "AzureOpenAI:Base" "https://<youropenai>.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:Deployment" "<yourmodeldeploymentname>"
dotnet build
dotnet run
```

### Use OpenAI

```
cd SKDemos
```

Modify Program.cs

```
OpenAISettings settings = new OpenAISettings();
settings.IsAzure = false;
IKernel kernel = settings.BuildSKernel();
```
Set User Secrets
```
dotnet user-secrets init
dotnet user-secrets set "OpenAI:Key" "sk-b6351ddxxxxxxxxxxxxxxxxxe680a"
dotnet build
dotnet run
```

# SKAzureFunction

## Test Locally
```
cd SKAskUpdate
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:Key" "b6351ddxxxxxxxxxxxxxxxxxe680a"
dotnet user-secrets set "AzureOpenAI:Base" "https://<youropenai>.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:Deployment" "<yourmodeldeploymentname>"
func start
```
## Publish to Azure Function

Can use VS Code. If after publishing it to Azure Function, need to set the environmetn variables in Application Settings blade:

```
AzureOpenAI:Key
AzureOpenAI:Base
AzureOpenAI:Deployment
```
