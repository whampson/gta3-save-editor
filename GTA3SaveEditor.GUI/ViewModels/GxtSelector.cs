using GTA3SaveEditor.Core;
using System.Collections.Generic;
using System.Windows.Input;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class GxtSelector : DialogViewModelBase
    {
        private Gxt m_gxtTable;
        private KeyValuePair<string, string> m_selectedItem;
        private int m_selectedIndex;

        public Gxt GxtTable
        {
            get { return m_gxtTable; }
            set { m_gxtTable = value; OnPropertyChanged(); }
        }

        public KeyValuePair<string, string> SelectedItem
        {
            get { return m_selectedItem; }
            set { m_selectedItem = value; OnPropertyChanged(); }
        }

        public int SelectedIndex
        {
            get { return m_selectedIndex; }
            set { m_selectedIndex = value; OnPropertyChanged(); }
        }

        public GxtSelector()
            : base()
        {
            SelectedIndex = -1;
        }

        public ICommand SelectCommand => new RelayCommand
        (
            () => CloseDialog(true),
            () => SelectedIndex != -1
        );
    }
}
