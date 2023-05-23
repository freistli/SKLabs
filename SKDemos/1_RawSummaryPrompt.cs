using Microsoft.SemanticKernel;
using System.Threading.Tasks;

namespace SKDemos
{
    class RawSummparyPrompt{
        public static async Task DemoAsync(IKernel kernel)
        {
            var prompt = @"{{$input}}

            One line TLDR with the fewest words.";

            var summarize = kernel.CreateSemanticFunction(prompt);

            string text1 = @"
            1st Law of Thermodynamics - Energy cannot be created or destroyed.
            2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
            3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

            Console.WriteLine(await summarize.InvokeAsync(text1)); 

            // Output:
            //   Energy conserved, entropy increases, zero entropy at 0K.
        }
 
    }
}