using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GTA3SaveEditor.GUI.Events;
using GTASaveData.GTA3;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI
{
    public class MainWindowVM : WindowVMBase
    {
        private const int NumSaveSlots = 8;

        public event EventHandler LogWindowRequest;

        private ObservableCollection<SaveSlot> m_saveSlots;
        private bool m_isDirty;

        public ObservableCollection<SaveSlot> SaveSlots
        {
            get { return m_saveSlots; }
            private set { m_saveSlots = value; OnPropertyChanged(); }
        }

        public bool IsDirty
        {
            get { return m_isDirty; }
            private set { m_isDirty = value; OnPropertyChanged(); }
        }

        public MainWindowVM()
        {
            var slots = Enumerable.Range(0, NumSaveSlots).Select(slot => new SaveSlot());
            SaveSlots = new ObservableCollection<SaveSlot>(slots);
        }

        public override void Init()
        {
            base.Init();

            OpenFileRequest += OpenFileRequest_Handler;
            CloseFileRequest += CloseFileRequest_Handler;
            SaveFileRequest += SaveFileRequest_Handler;
            RevertFileRequest += RevertFileRequest_Handler;

            RefreshSaveSlots();
        }

        public override void Load()
        {
            base.Load();
            SetStatusText("Ready.");
        }

        private void SetDirty()
        {
            if (!IsDirty)
            {
                IsDirty = true;
                UpdateTitle();
            }
        }

        private void ClearDirty()
        {
            IsDirty = false;
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            string title = App.Name;
            if (Editor.IsEditingFile)
            {
                title += " - " + Editor.ActiveFilePath;
            }
            if (IsDirty)
            {
                title = "*" + title;
            }

            Title = title;
        }

        private void RefreshSaveSlots()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string gta3UserFiles = Path.Combine(documentsPath, "GTA3 User Files");

            int slotNum = 1;
            foreach (var slot in SaveSlots)
            {
                slot.Path = Path.Combine(gta3UserFiles, $"GTA3sf{slotNum}.b");
                slot.Name = $"(slot is free)";
                slot.InUse = false;

                if (File.Exists(slot.Path))
                {
                    using GTA3Save save = GTA3Save.Load(slot.Path);
                    slot.Name = save?.Name ?? $"(slot is corrupt)";
                    slot.InUse = true;
                }

                slotNum++;
            }
        }

        public bool DoCloseFileRoutine(Action onFileClosed = null)
        {
            if (TheSave != null)
            {
                if (IsDirty)
                {
                    bool retval = false;
                    PromptYesNoCancel("Would you like to save your changes?", "Save Changes?",
                        yesAction: () =>
                        {
                            SaveFile();
                            retval = DoCloseFileRoutine(onFileClosed);
                        },
                        noAction: () =>
                        {
                            ClearDirty();
                            retval = DoCloseFileRoutine(onFileClosed);
                        },
                        cancelAction: () =>
                        {
                            retval = false;
                        });
                    return retval;
                }
            }

            CloseFile();
            onFileClosed?.Invoke();
            return true;
        }

        public void OpenFileRequest_Handler(object sender, FileIOEventArgs e)
        {
            ShowInfo("Open File!");
            throw new NotImplementedException();
        }

        public void CloseFileRequest_Handler(object sender, FileIOEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void SaveFileRequest_Handler(object sender, FileIOEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void RevertFileRequest_Handler(object sender, FileIOEventArgs e)
        {
            throw new NotImplementedException();
        }

        public ICommand ViewLogCommand => new RelayCommand
        (
            () => LogWindowRequest?.Invoke(this, EventArgs.Empty)
        );

        public ICommand ExitCommand => new RelayCommand
        (
            () => Application.Current.MainWindow.Close()
        );
    }
}
