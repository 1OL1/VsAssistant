using System;
using VsAssistant.Controls;
using VsAssistant.Data.Services;
using VsAssistant.Settings;

namespace VsAssistant.Locations
{
    /// <summary>
    /// Interaction logic for LocWindow.xaml
    /// </summary>
    public partial class LocWindow : BaseWindow
    {
        private LocViewModel ViewModel => (LocViewModel)this.Root.DataContext;

        internal LocWindow(LocationDataService dataService, SettingsService settingsService)
            : base(settingsService, settingsService.Current.LocWindow)
        {
            InitializeComponent();

            this.Root.DataContext = new LocViewModel(dataService);
        }

        protected override void OnClosed(EventArgs e)
        {
            // The service outlives the window, so the view model must unsubscribe from it
            this.ViewModel.Dispose();

            base.OnClosed(e);
        }
    }
}
