using System.Windows;
using System.Windows.Media;
using VsAssistant.Locations;

namespace VsAssistant
{
    public partial class App : System.Windows.Application
    {
        private NotifyIcon? trayIcon;
        protected override void OnStartup(StartupEventArgs e)
        {
            trayIcon = new NotifyIcon();
            trayIcon.Text = "ВС Ассистент";
            trayIcon.Icon = Resource.vs;
            trayIcon.Visible = true;
            trayIcon.ContextMenuStrip = CreateTrayMenu();

            base.OnStartup(e);
        }

        private ContextMenuStrip CreateTrayMenu()
        {
            var menu = new ContextMenuStrip();

            var loc = new ToolStripMenuItem("Локи");
            loc.Click += (s, e) => 
            {
                if (!Windows.OfType<LocWindow>().Any())
                {
                    new LocWindow().Show();
                }
            };
            menu.Items.Add(loc);

            menu.Items.Add(new ToolStripSeparator());

            var exit = new ToolStripMenuItem("Вьіход");
            exit.Click += (s, e) => Current.Shutdown();

            menu.Items.Add(exit);

            return menu;
        }
    }

}
