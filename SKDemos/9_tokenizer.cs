using System;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.Tokenizers;

public class SKTokens
{
    public static async Task DemoAsync()
    {
         string sentence = "The language we used was an early version of Fortran. You had to type programs on punch cards, then stack them in the card reader and press a button to load the program into memory and run it. The result would ordinarily be to print something on the spectacularly loud printer.";
        int tokenCount = GPT3Tokenizer.Encode(sentence).Count;

        Console.WriteLine("---");
        Console.WriteLine(sentence);
        Console.WriteLine("Tokens: " + tokenCount);
        Console.WriteLine("---\n\n");
    }
}