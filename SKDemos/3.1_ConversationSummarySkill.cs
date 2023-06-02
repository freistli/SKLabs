// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition; 

namespace SKDemos;

internal static class ConversationSummary
{
    private const string ChatTranscript =
        @"
John: Hello, I need to buy a book from Amazon. Can you help me?
Jane: Sure, what book are you looking for?
John: I'm looking for a book about the history of the United States.
Jane: I found a book called ""A People's History of the United States"" by Howard Zinn. Is that the one you're looking for?
John: Yes, that's the one. How much does it cost?
Jane: It costs $15.99.
John: Great, I'll buy it. Thanks for your help.
";
    private static IKernel kernel;
    public static async Task RunAsync(IKernel kn)
    {
        kernel = kn;
        await ConversationSummarySkillAsync();
        await GetConversationActionItemsAsync();
        await GetConversationTopicsAsync();
    }

    private static async Task ConversationSummarySkillAsync()
    {
        Console.WriteLine("======== SampleSkills - Conversation Summary Skill - Summarize ========");      

        IDictionary<string, ISKFunction> conversationSummarySkill =
            kernel.ImportSkill(new ConversationSummarySkill(kernel));

        SKContext summary = await kernel.RunAsync(
            ChatTranscript,
            conversationSummarySkill["SummarizeConversation"]);

        Console.WriteLine("Generated Summary:");
        Console.WriteLine(summary.Result);

        var fileOutput = kernel.ImportSkill(new FileIOSkill());

        var skContext = new ContextVariables();
        skContext.Set("content", summary.Result);
        skContext.Set("path", @"c:\testtemp\conversationsummary.txt");

        await kernel.RunAsync(skContext, fileOutput["WriteAsync"]);
    }

    private static async Task GetConversationActionItemsAsync()
    {
        Console.WriteLine("======== SampleSkills - Conversation Summary Skill - Action Items ========");
       
        IDictionary<string, ISKFunction> conversationSummarySkill =
            kernel.ImportSkill(new ConversationSummarySkill(kernel));

        SKContext summary = await kernel.RunAsync(
            ChatTranscript,
            conversationSummarySkill["GetConversationActionItems"]);

        Console.WriteLine("Generated Action Items:");
        Console.WriteLine(summary.Result);

        var fileOutput = kernel.ImportSkill(new FileIOSkill());

        var skContext = new ContextVariables();
        skContext.Set("content", summary.Result);
        skContext.Set("path", @"c:\testtemp\actionitems.txt");

        await kernel.RunAsync(skContext, fileOutput["WriteAsync"]);

        
    }

    private static async Task GetConversationTopicsAsync()
    {
        Console.WriteLine("======== SampleSkills - Conversation Summary Skill - Topics ========");
        
        IDictionary<string, ISKFunction> conversationSummarySkill =
            kernel.ImportSkill(new ConversationSummarySkill(kernel));

        SKContext summary = await kernel.RunAsync(
            ChatTranscript,
            conversationSummarySkill["GetConversationTopics"]);

        Console.WriteLine("Generated Topics:");
        Console.WriteLine(summary.Result);

        var fileOutput = kernel.ImportSkill(new FileIOSkill());

        var skContext = new ContextVariables();
        skContext.Set("content", summary.Result);
        skContext.Set("path", @"c:\testtemp\conversationtopic.txt");

        await kernel.RunAsync(skContext, fileOutput["WriteAsync"]);
    }

}

// ReSharper disable CommentTypo
/* Example Output:

======== SampleSkills - Conversation Summary Skill - Summarize ========
Generated Summary:

A possible summary is:

- John and Jane are both writing chatbots in different languages and share their links and poems.
- John's chatbot has a problem with writing repetitive poems and Jane helps him debug his code.
- Jane is writing a bot to summarize conversations and needs to generate a long conversation with John to test it.
- They use CoPilot to do most of the talking for them and comment on its limitations.
- They estimate the max length of the conversation to be 4096 characters.

A possible summary is:

- John and Jane are trying to generate a long conversation for some purpose.
- They are getting tired and bored of talking and look for ways to fill up the text.
- They use a Lorem Ipsum generator, but it repeats itself after a while.
- They sing the national anthems of Canada and the United States, and then talk about their favorite Seattle Kraken hockey players.
- They finally reach their desired length of text and say goodbye to each other.
======== SampleSkills - Conversation Summary Skill - Action Items ========
Generated Action Items:

{
    "actionItems": [
        {
            "owner": "John",
            "actionItem": "Improve chatbot's poem generation",
            "dueDate": "",
            "status": "In Progress",
            "notes": "Using GPT-3 model"
        },
        {
            "owner": "Jane",
            "actionItem": "Write a bot to summarize conversations",
            "dueDate": "",
            "status": "In Progress",
            "notes": "Testing with long conversations"
        }
    ]
}

{
    "action_items": []
}
======== SampleSkills - Conversation Summary Skill - Topics ========
Generated Topics:

{
  "topics": [
    "Chatbot",
    "Code",
    "Poem",
    "Model",
    "GPT-3",
    "GPT-2",
    "Bug",
    "Parameters",
    "Summary",
    "CoPilot",
    "Tokens",
    "Characters"
  ]
}

{
  "topics": [
    "Long conversation",
    "Lorem Ipsum",
    "O Canada",
    "Star-Spangled Banner",
    "Seattle Kraken",
    "Matty Beniers",
    "Jaden Schwartz",
    "Adam Larsson"
  ]
}

*/
// ReSharper restore CommentTypo