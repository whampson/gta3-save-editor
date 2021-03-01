using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WHampson.ToolUI;

namespace GTA3SaveEditor.GUI.Dialogs
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : DialogBase
    {
        public new InputDialogVM ViewModel
        {
            get { return (InputDialogVM) DataContext; }
            set { DataContext = value; }
        }

        public InputDialog()
        {
            InitializeComponent();
        }
    }
}
