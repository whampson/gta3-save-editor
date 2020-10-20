using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTA3SaveEditor.GUI
{
    public partial class App : Application
    {
        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }
    }
}
