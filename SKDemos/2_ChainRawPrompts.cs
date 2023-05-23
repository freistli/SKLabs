using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SKDemos
{
    class ChainRawPrompts
    {
        public static async Task DemoAsync(IKernel kernel)
        {
            string myJokePrompt = """
            Tell a short joke about {{$INPUT}}.
            """;

            string myPoemPrompt = """
            Take this "{{$INPUT}}" and convert it to a nursery rhyme.
            """;

            string myMenuPrompt = """
            Make this poem "{{$INPUT}}" influence the three items in a coffee shop menu. 
            The menu reads in enumerated form:
            1.
            """;

            var myJokeFunction = kernel.CreateSemanticFunction(myJokePrompt, maxTokens: 500);
            var myPoemFunction = kernel.CreateSemanticFunction(myPoemPrompt, maxTokens: 500);
            var myMenuFunction = kernel.CreateSemanticFunction(myMenuPrompt, maxTokens: 500);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var myOutput = await kernel.RunAsync(
                new ContextVariables("Charlie Brown"),
                myJokeFunction,
                myPoemFunction,
                myMenuFunction);
            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;

            Console.WriteLine(myOutput + "\r\n Runtime " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                            ts.Hours, ts.Minutes, ts.Seconds,
                                            ts.Milliseconds / 10));
 
        }

    }
}