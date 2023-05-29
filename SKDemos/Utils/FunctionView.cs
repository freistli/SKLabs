// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;

namespace SKDemos;

/// <summary>
/// Basic logger printing to console
/// </summary>
internal static class FunctionSummary
{
     private static void PrintFunction(FunctionView func)
    {
        Console.WriteLine($"   {func.Name}: {func.Description}");

        if (func.Parameters.Count > 0)
        {
            Console.WriteLine("      Params:");
            foreach (var p in func.Parameters)
            {
                Console.WriteLine($"      - {p.Name}: {p.Description}");
                Console.WriteLine($"        default: '{p.DefaultValue}'");
            }
        }

        Console.WriteLine();
    }
    public static async Task RunAsync(IKernel kernel)
    {
        Console.WriteLine("======== FunctionView ========");
        FunctionsView functions = kernel.Skills.GetFunctionsView();
        ConcurrentDictionary<string, List<FunctionView>> nativeFunctions = functions.NativeFunctions;
        ConcurrentDictionary<string, List<FunctionView>> semanticFunctions = functions.SemanticFunctions;

        Console.WriteLine("*****************************************");
        Console.WriteLine("****** Native skills and functions ******");
        Console.WriteLine("*****************************************");
        Console.WriteLine();

        foreach (KeyValuePair<string, List<FunctionView>> skill in nativeFunctions)
        {
            Console.WriteLine("Skill: " + skill.Key);
            foreach (FunctionView func in skill.Value) { PrintFunction(func); }
        }

        Console.WriteLine("*****************************************");
        Console.WriteLine("***** Semantic skills and functions *****");
        Console.WriteLine("*****************************************");
        Console.WriteLine();

        foreach (KeyValuePair<string, List<FunctionView>> skill in semanticFunctions)
        {
            Console.WriteLine("Skill: " + skill.Key);
            foreach (FunctionView func in skill.Value) { PrintFunction(func); }
        }
    }
}