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
        { }

        protected override void Initialize()
        {
            base.Initialize();

            SelectedBlockIndex = 0;
            UpdateTextBox();
        }

        protected override void Shutdown()
        {
            base.Shutdown();

            SelectedBlockIndex = -1;
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
