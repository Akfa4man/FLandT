using FLandT_laba1_ver4.UI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace FLandT_laba1_ver4.UI.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel? VM => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (_, __) =>
            {
                try { MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); }
                catch { }
            };
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (VM is null) return;

            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            { if (VM.RunCommand?.CanExecute(null) == true) VM.RunCommand.Execute(null); e.Handled = true; return; }

            else if (e.Key == Key.L && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            { if (VM.ClearCommand?.CanExecute(null) == true) VM.ClearCommand.Execute(null); e.Handled = true; return; }

            else if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            { if (VM.CopyReportCommand?.CanExecute(null) == true) VM.CopyReportCommand.Execute(null); e.Handled = true; return; }
        }
    }
}