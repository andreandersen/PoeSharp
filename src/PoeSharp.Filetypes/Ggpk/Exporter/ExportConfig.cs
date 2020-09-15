using System;

namespace PoeSharp.Filetypes.Ggpk.Exporter
{
    public class ExportConfig
    {
        private static readonly int DefaultEnumTasks = Math.Max(1, Environment.ProcessorCount - 2);
        private static readonly int DefaultExportTasks = 64;

        public ExportConfig(string exportPath, bool shallow = false)
            : this(exportPath, DefaultEnumTasks, DefaultExportTasks, shallow) {}

        public ExportConfig(
            string exportPath,
            int enumerationTasks,
            int exportTasks,
            bool shallow = false)
        {
            EnumerationTasks =
                enumerationTasks >= 1
                && enumerationTasks <= Environment.ProcessorCount ?
                enumerationTasks :
                throw new ArgumentException(
                    "Enumeration tasks must be at least 1, and " +
                    "should not be more than total threads/cores",
                    nameof(enumerationTasks));

            ExportTasks =
                exportTasks >= 1 ?
                ExportTasks = exportTasks :
                throw new ArgumentException(
                    "Export tasks needs to be at least 1",
                    nameof(exportTasks));

            ExportPath = exportPath;
            Shallow = shallow;
        }

        public int EnumerationTasks { get; }
        public int ExportTasks { get; }
        public string ExportPath { get; }
        public bool Shallow { get; set; }
    }
}
