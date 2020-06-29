using GTA3SaveEditor.GUI.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GTA3SaveEditor.GUI.Views
{
    public abstract class TabPageBase<T> : UserControl
        where T : TabPageViewModelBase
    {
        public T ViewModel
        {
            get { return (T) DataContext; }
            set { DataContext = value; }
        }

        protected TabPageBase()
            : base()
        {
            Loaded += View_Loaded;
        }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            // Changing terminology here to make event flow consistent with view models
            OnInitialize();
        }

        private void ViewModel_ShuttingDown(object sender, EventArgs e)
        {
            OnShutdown();
        }

        protected virtual void OnInitialize()
        {
            ViewModel.ShuttingDown += ViewModel_ShuttingDown;
        }

        protected virtual void OnShutdown()
        {
            ViewModel.ShuttingDown -= ViewModel_ShuttingDown;
            Loaded -= View_Loaded;        }
    }
}
