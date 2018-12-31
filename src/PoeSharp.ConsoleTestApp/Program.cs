using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PoeSharp.ConsoleTestApp
{
    class Program
    {

        static async Task Main(string[] args)
        {
            // Set this path to where your Content.ggpk path is:
            var ggpkPath = @"C:\Program Files (x86)\Grinding Gear Games\Path of Exile\Content.ggpk";

            // Set this path to where you want your files to be exported to:
            var exportPath = @"C:\noindex\ggpk6";

            await Examples.ExportGgpkExample.RunExporter(ggpkPath, exportPath, exportTasks: 64);

            if (Debugger.IsAttached)
                Console.ReadKey(true);
        }
    }
}
