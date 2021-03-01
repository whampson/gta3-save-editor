using System.Windows.Input;
using WHampson.ToolUI;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.Dialogs
{
    public class InputDialogVM : DialogViewModelBase
    {
        private string m_label;
        private string m_text;

        public string Text
        {
            get { return m_text; }
            set { m_text = value; OnPropertyChanged(); }
        }

        public string Label
        {
            get { return m_label; }
            set { m_label = value; OnPropertyChanged(); }
        }

        public ICommand OkCommand => new RelayCommand
        (
            () => Close(),
            () => /*!string.IsNullOrWhiteSpace(Text)*/ Text != null
        );
    }
}
