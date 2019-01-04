using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using PoeSharp.Filetypes.Dat;
using PoeSharp.Filetypes.Ggpk;

namespace RawDatFilesViewer.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand OpenGgpkCommand { get; }

        private GgpkFileSystem _ggpk;
        private RawDatFile _selectedFile;

        public ObservableCollection<RawDatFile> DatFiles { get; set; }
        public DatViewModel SelectedFileModel { get; set; }

        public RawDatFile SelectedFile
        {
            get => _selectedFile; set
            {
                _selectedFile = value;
                SelectedFileModel = new DatViewModel(_selectedFile);
            }
        }

        public string OpenedGgpkFileName { get; set; }

        public MainViewModel()
        {
            OpenGgpkCommand = new RelayCommand(o => ShowOpenDialog());
            DatFiles = new ObservableCollection<RawDatFile>();
            string file = @"C:\Program Files (x86)\Grinding Gear Games\Path of Exile\Content.ggpk";
            if (File.Exists(file))
            {
                OpenGgpkFile(file);
            }
        }

        private bool ShowOpenDialog()
        {
            var dialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".ggpk",
                Filter = "GGPK (.ggpk)|*.ggpk",
                FileName = "Content.ggpk",
                InitialDirectory = @"C:\Program Files (x86)\Grinding Gear Games\Path of Exile",
                Title = "Locate the GGPK file"
            };

            if (dialog.ShowDialog() == true)
            {
                OpenGgpkFile(dialog.FileName);
                return true;
            }

            return false;
        }

        private void OpenGgpkFile(string fileName)
        {
            _ggpk = new GgpkFileSystem(fileName);
            OpenedGgpkFileName = fileName;

            Task.Factory.StartNew(() =>
            {
                var datFiles = _ggpk
                    .Directories["Data"]
                    .Files
                    .Where(c => c.Key.EndsWith(".dat"))
                    .OrderBy(c => c.Key)
                    .Select(f => RawDatFile.CreateFromGgpkFile(f.Value))
                    .ToList();

                DatFiles = new ObservableCollection<RawDatFile>(datFiles);
            });
        }
    }
}
