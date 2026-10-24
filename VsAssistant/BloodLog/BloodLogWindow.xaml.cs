using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using VsAssistant.Controls;
using VsAssistant.Settings;
using Point = System.Windows.Point;

namespace VsAssistant.BloodLog
{
    public partial class BloodLogWindow : BaseWindow
    {
        private BloodLogViewModel ViewModel => (BloodLogViewModel)this.Root.DataContext;

        internal BloodLogWindow(SettingsService settingsService)
            : base(settingsService, settingsService.Current.BloodLogWindow)
        {
            InitializeComponent();

            this.Root.DataContext = new BloodLogViewModel();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.ViewModel.Dispose();

            base.OnClosed(e);
        }

        protected override Point GetDefaultPosition(Rect workArea)
        {
            return new Point(workArea.Left + (workArea.Width - this.Width) / 2, workArea.Top + (workArea.Height - this.Height) / 2);
        }

        private void Link_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Opens the default browser; WPF does not navigate by itself outside a Frame
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
