namespace VsAssistant.Helpers
{
    internal static class TrayHelper
    {
        #region Methods
        /// <summary>
        /// Creates a visible tray icon with a context menu made of the given items.
        /// The caller must dispose the icon on exit, otherwise it stays in the tray until the mouse moves over it.
        /// </summary>
        public static NotifyIcon CreateTrayIcon(string text, Icon icon, params ToolStripItem[] menuItems)
        {
            var menu = new ContextMenuStrip();
            menu.Items.AddRange(menuItems);

            var trayIcon = new NotifyIcon();
            trayIcon.Text = text;
            trayIcon.Icon = icon;
            trayIcon.ContextMenuStrip = menu;
            trayIcon.Visible = true;

            return trayIcon;
        }
        #endregion
    }
}
