using GTASaveData;
using GTASaveData.GTA3;
using GTASaveData.Types.Interfaces;
using System;
using System.Collections.Generic;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class JsonViewer : TabPageViewModelBase
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

        public JsonViewer(Main mainViewModel)
            : base("JSON Viewer", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        { }

        public override void Load()
        {
            base.Load();
            SelectedBlockIndex = 0;
            UpdateTextBox();
        }

        public override void Unload()
        {
            base.Unload();
            SelectedBlockIndex = -1;
            UpdateTextBox();
        }

        public override void Refresh()
        {
            base.Refresh();
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
