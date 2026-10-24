using System;
using System.Windows;
using VsAssistant.Controls;
using VsAssistant.Data.Services;
using VsAssistant.Settings;
using Point = System.Windows.Point;

namespace VsAssistant.Castles
{
    public partial class CastleWindow : BaseWindow
    {
        private CastlesViewModel ViewModel => (CastlesViewModel)this.Root.DataContext;

        internal CastleWindow(LocationDataService dataService, SettingsService settingsService)
            : base(settingsService, settingsService.Current.CastleWindow)
        {
            InitializeComponent();

            this.Root.DataContext = new CastlesViewModel(dataService);
        }

        protected override void OnClosed(EventArgs e)
        {
            this.ViewModel.Dispose();

            base.OnClosed(e);
        }

        protected override Point GetDefaultPosition(Rect workArea)
        {
            return new Point(workArea.Right - this.Width, workArea.Top);
        }
    }
}
