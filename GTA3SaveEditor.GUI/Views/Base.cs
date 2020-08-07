using GTA3SaveEditor.Core;
using GTA3SaveEditor.GUI.ViewModels;
using GTASaveData.GTA3;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GTA3SaveEditor.GUI.Views
{
    public class TabPageBase<T> : UserControl
        where T : BaseTabPage
    {
        public T ViewModel
        {
            get { return (T) DataContext; }
            set { DataContext = value; }
        }

        public TabPageBase()
            : base()
        {
            Loaded += View_Loaded;
        }

        public SaveEditor TheEditor => ViewModel.MainViewModel.TheEditor;
        public GTA3Save TheSave => ViewModel.MainViewModel.TheSave;

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            // Changing terminology here to make event flow consistent with view models
            OnInitialize();
        }

        private void ViewModel_ShuttingDown(object sender, EventArgs e)
        {
            OnShutdown();
        }

        private void ViewModel_Loading(object sender, EventArgs e)
        {
            OnLoad();
        }

        private void ViewModel_Unloading(object sender, EventArgs e)
        {
            OnUnload();
            RaiseEvent(new RoutedEventArgs(UnloadedEvent));
        }

        private void ViewModel_Updating(object sender, EventArgs e)
        {
            OnUpdate();
        }

        protected virtual void OnInitialize()
        {
            ViewModel.ShuttingDown += ViewModel_ShuttingDown;
            ViewModel.Loading += ViewModel_Loading;
            ViewModel.Unloading += ViewModel_Unloading;
            ViewModel.Updating += ViewModel_Updating;
        }

        protected virtual void OnShutdown()
        {
            ViewModel.Updating -= ViewModel_Updating;
            ViewModel.Unloading -= ViewModel_Unloading;
            ViewModel.Loading -= ViewModel_Loading;
            ViewModel.ShuttingDown -= ViewModel_ShuttingDown;
            Loaded -= View_Loaded;
        }

        protected virtual void OnLoad()
        { }

        protected virtual void OnUnload()
        { }

        protected virtual void OnUpdate()
        { }
    }

    public class DialogBase<T> : Window
        where T : DialogViewModelBase
    {
        public T ViewModel
        {
            get { return (T) DataContext; }
            set { DataContext = value; }
        }

        public DialogBase()
            : base()
        { }
    }
}
