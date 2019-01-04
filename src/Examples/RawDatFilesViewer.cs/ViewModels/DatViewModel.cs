using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeSharp.Filetypes.Dat;

namespace RawDatFilesViewer.ViewModels
{
    public class DatViewModel : BaseViewModel
    {
        public RawDatFile File { get; }
        public IReadOnlyList<string> Rows { get; }

        public DatViewModel(RawDatFile file)
        {
            File = file;
            var rows = new List<string>();

            var rowsData = file.Rows.Span;
            
            foreach (var row in rowsData)
            {
                var data = row.ToArray();
                rows.Add(BitConverter.ToString(data).Replace('-', ' '));
            }

            Rows = rows;
        }

    }
}
