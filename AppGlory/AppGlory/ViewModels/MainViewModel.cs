using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AppGlory.Commands;
using AppGlory.Models;
using AppGlory.Services;
using Microsoft.Win32;

namespace AppGlory.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _filePaths = "";
        private string _status    = "Seleccione uno o varios archivos de log para comenzar.";
        private bool   _analyzing;

        public string FilePaths
        {
            get => _filePaths;
            set { _filePaths = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public bool Analyzing
        {
            get => _analyzing;
            set { _analyzing = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LogRecord> Records { get; } = new();

        public ICommand SelectFilesCommand { get; }
        public ICommand AnalyzeCommand     { get; }
        public ICommand ExportCommand      { get; }

        public MainViewModel()
        {
            SelectFilesCommand = new RelayCommand(SelectFiles);
            AnalyzeCommand     = new AsyncRelayCommand(AnalyzeAsync, () => !string.IsNullOrWhiteSpace(FilePaths) && !Analyzing);
            ExportCommand      = new RelayCommand(Export, () => Records.Count > 0);
        }

        private void SelectFiles()
        {
            var dlg = new OpenFileDialog
            {
                Title       = "Seleccionar archivos de log",
                Filter      = "Archivos de log (*.txt;*.log)|*.txt;*.log|Todos (*.*)|*.*",
                Multiselect = true
            };

            if (dlg.ShowDialog() == true)
                FilePaths = string.Join("|", dlg.FileNames);
        }

        private async Task AnalyzeAsync()
        {
            var files = FilePaths.Split('|')
                                 .Select(f => f.Trim())
                                 .Where(File.Exists)
                                 .ToList();

            if (files.Count == 0)
            {
                Status = "No se encontraron archivos válidos.";
                return;
            }

            Analyzing = true;
            Status    = $"Analizando {files.Count} archivo(s)...";
            Records.Clear();

            var results = await Task.Run(() =>
            {
                var all = new List<LogRecord>();
                foreach (var f in files)
                    all.AddRange(LogParser.Parse(f));
                return all;
            });

            foreach (var r in results)
                Records.Add(r);

            Status    = $"Completado: {Records.Count} registro(s) encontrado(s) en {files.Count} archivo(s).";
            Analyzing = false;
        }

        private void Export()
        {
            var dlg = new SaveFileDialog
            {
                Title      = "Exportar a Excel",
                Filter     = "Excel (*.xlsx)|*.xlsx",
                FileName   = $"AnalisisLogs_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
                DefaultExt = ".xlsx"
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                ExcelExporter.Export(Records, dlg.FileName);
                Status = $"Exportado correctamente: {dlg.FileName}";
            }
            catch (Exception ex)
            {
                Status = $"Error al exportar: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
