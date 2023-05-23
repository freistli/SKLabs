using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SKDemos.plugins
{
    public class DeviceVersionSkill
    {
        [SKFunction("Find device version number from official Surface Web Page")]
        [SKFunctionInput(Description = "First HTML table string input")]
        [SKFunctionContextParameter(Name = "devicename", Description = "Second device name")]
        public string GetAvailableVersions(string input, SKContext context)
        {
            List<string> result = new List<string>();
            string pattern = string.Format(@"<p>(.*?)<\/p>[^<(.?)*]*<\/td>[^<(.?)*]*<td>[^<(.?)*]*<p>{0}<\/p>", Regex.Escape(context["devicename"]));
            foreach (Match match in Regex.Matches(input, pattern, RegexOptions.IgnoreCase))
            {
                Console.WriteLine("{0}", match.Groups[1].Value);
                result.Add(match.Groups[1].Value);
            }
            return string.Join("\r\n", result);
        }

    }
}
