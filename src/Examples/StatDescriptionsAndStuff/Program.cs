using System;
using System.Diagnostics;
using System.IO;

using PoeSharp.Filetypes.StatDescriptions;

namespace StatDescriptionsAndStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();

            // Point to your stat_description.txt
            const string filePath = 
                @"C:\noindex\3124\Bundles2\Bundles2\Metadata\" + 
                @"StatDescriptions\stat_descriptions.txt";

            var instructions = StatDescriptionParser
                .ParseInstructions(File.ReadAllText(filePath));

            var elapsed = sw.Elapsed;
            Console.WriteLine(elapsed);
        }
    }
}
