// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Skills.Web; 

namespace SKDemos;

//Use WebSearchUrl to search for a URL. A simple example of a native skill.
public class WebSearchUrl
{
    public static async Task RunAsync()
    {
        Console.WriteLine("======== WebSearchQueries ========");

        IKernel kernel = Kernel.Builder.WithLogger(ConsoleLogger.Log).Build();

        // Load native skills
        var skill = new SearchUrlSkill();
        var bing = kernel.ImportSkill(skill, "search");

        // Run
        var ask = "What's the tallest building in Europe?";
        var result = await kernel.RunAsync(
            ask,
            bing["BingSearchUrl"]
        );

        Console.WriteLine(ask + "\n");
        Console.WriteLine(result);
    }
}