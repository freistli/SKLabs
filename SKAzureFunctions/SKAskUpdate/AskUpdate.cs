using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using Newtonsoft.Json;
using SKDemos.plugins;
using System.Runtime.Caching;
using System.Diagnostics;

namespace SKAzureFunctions
{
    public class AskUpdate
    {
        private readonly ILogger _logger;
        private ObjectCache _cache;
        private CacheItemPolicy _policy;
        private OpenAISettings _settings;
        IKernel _kernel;

        public AskUpdate(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AskUpdate>();
            _cache = MemoryCache.Default;
            _policy = new CacheItemPolicy();
            _policy.AbsoluteExpiration =
                    DateTimeOffset.Now.AddHours(24.0);
            _settings = new OpenAISettings();
            _kernel = _settings.BuildSKernel(_logger);
        }

        [Function("AskUpdate")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            _logger.LogDebug("OpenAIKey: " + _settings.OpenAIKey);
            _logger.LogDebug("OpenAIEndpoint: " + _settings.OpenAIEndpoint);
            _logger.LogDebug("OpenAIDeploymentName: " + _settings.OpenAIDeploymentName);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string webdataUri = data?.webdatauri;
            string deviceName = data?.devicename;

            if(deviceName == null || webdataUri == null) {

                var failResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                failResponse.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                failResponse.WriteString("Parameters are not correct");

                return failResponse;
            }


            var httpSkill = _kernel.ImportSkill(new HttpSkill());
            var skContext = new ContextVariables();

            skContext.Set("input", webdataUri);
            
            if (_cache.GetCacheItem(webdataUri) == null)
            {
                _logger.LogInformation("cache miss for "+webdataUri);
                var content = await _kernel.RunAsync(skContext, httpSkill["GetAsync"]);
                if(content?.Result != null)
                 _cache.Set(webdataUri, content.Result, _policy);                
            }
            else
                _logger.LogInformation("cache hit for "+webdataUri);

            skContext.Set("input", _cache.GetCacheItem(webdataUri).Value.ToString());

            skContext.Set("devicename", deviceName);

            var deviceVersionSkill = _kernel.ImportSkill(new DeviceVersionSkill(), "DeviceVersionSkill");

            var SemanticPlugins = _kernel.ImportSemanticSkillFromDirectory("Plugins", "SemanticPlugins");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var output = await _kernel.RunAsync(skContext, deviceVersionSkill["GetAvailableVersions"], SemanticPlugins["FindLatestVersion"]);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            _logger.LogInformation("Semantic Function Runtime " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                            ts.Hours, ts.Minutes, ts.Seconds,
                                            ts.Milliseconds / 10));

            var response = req.CreateResponse(HttpStatusCode.OK);

            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            response.WriteString(output.Result);

            return response;
        }
    }
}
