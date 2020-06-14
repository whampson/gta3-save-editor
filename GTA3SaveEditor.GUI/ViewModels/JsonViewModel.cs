using GTA3SaveEditor.GUI.Events;
using GTASaveData;
using GTASaveData.GTA3;
using GTASaveData.Types.Interfaces;
using System;
using System.Collections.Generic;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class JsonViewModel : TabPageViewModelBase
    {
        private int m_selectedBlockIndex;
        private string m_text;

        public string[] BlockNames
        {
            get { return Enum.GetNames(typeof(DataBlock)); }
        }

        public int SelectedBlockIndex
        {
            get { return m_selectedBlockIndex; }
            set { m_selectedBlockIndex = value; OnPropertyChanged(); }
        }

        public string Text
        {
            get { return m_text; }
            set { m_text = value; OnPropertyChanged(); }
        }

        public JsonViewModel(MainViewModel mainViewModel)
            : base("JSON Viewer", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        {
            MainViewModel.TabRefresh += MainViewModel_TabRefresh;
        }

        private void MainViewModel_TabRefresh(object sender, TabRefreshEventArgs e)
        {
            switch (e.Trigger)
            {
                case TabRefreshTrigger.WindowLoaded:
                case TabRefreshTrigger.FileClosed:
                    SelectedBlockIndex = -1;
                    break;
                case TabRefreshTrigger.FileOpened:
                    SelectedBlockIndex = 0;
                    break;
            }

            UpdateTextBox();
        }

        public void UpdateTextBox()
        {
            if (!MainViewModel.TheEditor.IsFileOpen || SelectedBlockIndex < 0)
            {
                Text = "";
                return;
            }

            IReadOnlyList<ISaveDataObject> blocks = (MainViewModel.TheSave as ISaveData).Blocks;
            Text = (blocks[SelectedBlockIndex] as SaveDataObject).ToJsonString();
        }
    }
}
