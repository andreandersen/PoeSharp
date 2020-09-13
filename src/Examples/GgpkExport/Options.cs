using System.Collections.Generic;
using System.CommandLine;

using CommandLine;
using CommandLine.Text;

namespace GgpkExport
{
    public class Options
    {
        [Value(0, MetaName = "Source", HelpText = "Source GGPK File", Required = true)]
        public string SourceFile { get; set; }

        [Value(1, MetaName = "Destination", HelpText = "Destination Folder", Required = true)]
        public string DestinationFolder { get; set; }

        [Option('e', "exporttasks", Default = 64, HelpText = "Number of concurrent export tasks.\nTune down for HDDs")]
        public int ExporterTasks { get; set; }

        [Option('n', "enumtasks", Default = 2, HelpText = "Number of file enumeration tasks.")]
        public int EnumerationTasks { get; set; }

        [Usage(ApplicationAlias = "GgpkExport")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Normal Usage", new Options
                {
                    SourceFile = @"D:\Games\Path of Exile\Content.ggpk",
                    DestinationFolder = @"C:\GgpkExport\"
                });

                yield return new Example("HDD Usage", new Options
                {
                    SourceFile = @"D:\Games\Path of Exile\Content.ggpk",
                    DestinationFolder = @"C:\GgpkExport\",
                    ExporterTasks = 1
                });
            }
        }

    }
}
