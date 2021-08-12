using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using GTA3SaveEditor.Core.Game;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI.Tabs
{
    /// <summary>
    /// Interaction logic for GangsTab.xaml
    /// </summary>
    public partial class PedsTab : TabPage<PedsVM>
    {
        public PedsTab()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null) return;

            PedTypeId t = ViewModel.SelectedPedType;
            int i = ViewModel.SelectedPedIndex;

            ViewModel.SelectedPed = (i > -1)
                ? ViewModel.TheSave.PedTypeInfo.PedTypes[i]
                : null;
            ViewModel.SelectedGang = (ViewModel.IsSelectedPedTypeGang)
                ? ViewModel.TheSave.Gangs[GTA3.GetGangType(t)]
                : null;

            ViewModel.Update();

            Debug.Assert(i == -1 || (PedTypeId) i == ViewModel.SelectedPedType);
        }
    }
}
