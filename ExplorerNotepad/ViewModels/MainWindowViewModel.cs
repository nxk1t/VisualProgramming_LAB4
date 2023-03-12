using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Styling;
using ExplorerNotepad.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace ExplorerNotepad.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<Explorer> ExplorerCollection;

        public ObservableCollection<Explorer> ExplorerCollectionProperties { get => ExplorerCollection; set => this.RaiseAndSetIfChanged(ref ExplorerCollection, value); }

        private int currentIndex;
        public int currentIndexProperties { get => currentIndex; set 
            {
                this.RaiseAndSetIfChanged(ref currentIndex, value);
                if (VisibilityOpenButton == true && VisibilitySaveButton == false)
                {
                    if (ExplorerCollection[currentIndex] is Files) outTextFolderProperties = ExplorerCollection[currentIndex].Header;
                    else outTextFolderProperties = "";
                    saveButtonTextProperties = "Открыть";
                }
                else if (VisibilityOpenButton == false && VisibilitySaveButton == true)
                {
                    if (ExplorerCollection[currentIndex] is Files)
                    {
                        saveButtonTextProperties = "Сохранить";
                        outTextFolderProperties = ExplorerCollection[currentIndex].Header;
                    }
                    else
                    {
                        saveButtonTextProperties = "Открыть";
                        outTextFolderProperties = "";
                    }
                }
            } }

        private bool VisibilityNotePad;
        public bool VisibilityNotePadProperties { get => VisibilityNotePad; set => this.RaiseAndSetIfChanged(ref VisibilityNotePad, value); }

        private bool VisibilityExplorer;
        public bool VisibilityExplorerProperties { get => VisibilityExplorer; set => this.RaiseAndSetIfChanged(ref VisibilityExplorer, value); }

        private bool VisibilitySaveButton;
        public bool VisibilitySaveButtonProperties { get => VisibilitySaveButton; set => this.RaiseAndSetIfChanged(ref VisibilitySaveButton, value); }

        private bool VisibilityOpenButton;
        public bool VisibilityOpenButtonProperties { get => VisibilityOpenButton; set => this.RaiseAndSetIfChanged(ref VisibilityOpenButton, value); }

        private string outTextBox;
        public string outTextBoxProperties { get => outTextBox; set => this.RaiseAndSetIfChanged(ref outTextBox, value); }

        private string outTextFolder;
        public string outTextFolderProperties { get => outTextFolder; set { this.RaiseAndSetIfChanged(ref outTextFolder, value); if (outTextFolder != "") saveButtonTextProperties = "Сохранить"; } }

        private string saveButtonText;
        public string saveButtonTextProperties { get => saveButtonText; set => this.RaiseAndSetIfChanged(ref saveButtonText, value); }

        private string Path = Directory.GetCurrentDirectory();

        public MainWindowViewModel()
        {
            VisibilityNotePad = true;
            VisibilityExplorer = false;
            VisibilitySaveButton = false;
            VisibilityOpenButton = false;

            outTextBox = string.Empty;
            outTextFolder = string.Empty;

            saveButtonText = "Открыть";

            ExplorerCollection = new ObservableCollection<Explorer>();

            fillCollection(Path);
        }
        public void returnBack()
        {
            outTextFolder = string.Empty;
            VisibilityNotePadProperties = true;
            VisibilityExplorerProperties = false;
            VisibilitySaveButton = false;
            VisibilityOpenButton = false;
        }

        public void openExplorer()
        {
            outTextFolder = string.Empty;
            VisibilityNotePadProperties = false;
            VisibilityExplorerProperties = true;
            VisibilitySaveButton = false;
            VisibilityOpenButton = true;
        }

        public void saveExplorer()
        {
            outTextFolder = "";
            VisibilityNotePadProperties = false;
            VisibilityExplorerProperties = true;
            VisibilitySaveButton = true;
            VisibilityOpenButton = false;
            currentIndex = 0;
        }

        public void clickButton()
        {
            if (VisibilityOpenButtonProperties == true) openButton_openRegime();
            else if (VisibilitySaveButtonProperties == true) openButton_saveRegime();
        }

        public void openButton_openRegime()
        {
            if (ExplorerCollection[currentIndexProperties] is Directories)
            {
                if (ExplorerCollection[currentIndexProperties].Header == "..")
                {
                    var temp_path_first = Directory.GetParent(Path);
                    if (temp_path_first != null)
                    {
                        fillCollection(temp_path_first.FullName);
                        Path = temp_path_first.FullName;
                    }
                    else if (temp_path_first == null) fillCollection("");
                }
                else
                {
                    var temp_path_second = ExplorerCollection[currentIndex].SourceName;
                    fillCollection(ExplorerCollection[currentIndexProperties].SourceName);
                    Path = temp_path_second;
                }
            }
            else
            {
                loadFile(ExplorerCollection[currentIndex].SourceName);
                returnBack();
            }
        }

        public void openButton_saveRegime()
        {
            if (ExplorerCollection[currentIndexProperties] is Directories && outTextFolderProperties == "")
            {
                saveButtonTextProperties = "Открыть";
                if (ExplorerCollection[currentIndexProperties].Header == "..")
                {
                    var temp_path_first = Directory.GetParent(Path);
                    if (temp_path_first != null) fillCollection(temp_path_first.FullName);
                    else if (temp_path_first == null) fillCollection("");
                    Path = temp_path_first.FullName;
                }
                else
                {
                    var temp_path_second = ExplorerCollection[currentIndex].SourceName;
                    fillCollection(ExplorerCollection[currentIndexProperties].SourceName);
                    Path = temp_path_second;
                }
            }
            else if (ExplorerCollection[currentIndexProperties] is Files || outTextFolderProperties != "")
            {
                if (outTextFolderProperties == ExplorerCollection[currentIndex].Header) saveFile(ExplorerCollection[currentIndexProperties].SourceName, 0);
                else
                {
                    var temp_path_third = Path;
                    temp_path_third += "\\" + outTextFolderProperties;
                    saveFile(temp_path_third, 1);
                }
                returnBack();
            }
        }

        public void loadFile(string temp_path)
        {
            string newText = String.Empty;
            StreamReader read = new StreamReader(ExplorerCollection[currentIndex].SourceName);
            while (read.EndOfStream != true)
            {
                newText += read.ReadLine() + "\n";
            }
            outTextBoxProperties = newText;
        }

        public async void saveFile(string temp_path, int flag)
        {
            if (flag == 0)
            {
                using (StreamWriter write = new StreamWriter(temp_path))
                {
                    write.Write(outTextBoxProperties);
                }
            }
            else
            {
                using (FileStream fileStream = new FileStream(temp_path, FileMode.OpenOrCreate))
                {
                    byte[] buffer = Encoding.Default.GetBytes(outTextBoxProperties);
                    await fileStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }

        public void fillCollection(string varPath)
        {
            ExplorerCollection.Clear();
            if (varPath != "")
            {
                var DirectoryInformation = new DirectoryInfo(varPath);
                ExplorerCollection.Add(new Directories(".."));
                foreach (var directory in DirectoryInformation.GetDirectories())
                {
                    ExplorerCollection.Add(new Directories(directory));
                }
                foreach (var fileinfo in DirectoryInformation.GetFiles())
                {
                    ExplorerCollection.Add(new Files(fileinfo));
                }
            }
            else if (varPath == "")
            {
                foreach (var disk in Directory.GetLogicalDrives())
                {
                    ExplorerCollection.Add(new Directories(disk));
                }
            }
            currentIndexProperties = 0;
        }

        public void DoubleTap()
        {
            if (VisibilityOpenButtonProperties == true) openButton_openRegime();
            else if (VisibilitySaveButtonProperties == true) openButton_saveRegime();
        }

    }
}