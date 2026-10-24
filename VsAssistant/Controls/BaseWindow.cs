using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using VsAssistant.Settings;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using Point = System.Windows.Point;

namespace VsAssistant.Controls
{
    public class BaseWindow : Window
    {
        #region Constants
        private static readonly int[] OpacityLevels = { 100, 80, 60, 40, 20 };
        #endregion

        #region Members
        private readonly SettingsService settingsService;
        private readonly WindowSettings settings;
        private readonly ContextMenu opacityMenu;
        #endregion

        internal BaseWindow(SettingsService settingsService, WindowSettings settings)
        {
            this.settingsService = settingsService;
            this.settings = settings;

            SetResourceReference(StyleProperty, typeof(BaseWindow));

            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (_, _) => Close()));

            opacityMenu = new ContextMenu { Placement = PlacementMode.Bottom };
            opacityMenu.SetResourceReference(StyleProperty, "BaseContextMenu");
            opacityMenu.SetResourceReference(ItemsControl.ItemContainerStyleProperty, "BaseMenuItem");
            foreach (var percent in OpacityLevels)
            {
                var item = new MenuItem { Header = $"{percent}%", Tag = percent / 100.0 };
                item.Click += OpacityMenuItem_Click;
                opacityMenu.Items.Add(item);
            }
        }

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_OpacityButton") is ButtonBase opacityButton)
            {
                opacityButton.Click += OpacityButton_Click;
                opacityButton.ContextMenu = opacityMenu;
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            ApplyOpacity(settings.Opacity);
            RestorePosition();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();

                settings.Left = Left;
                settings.Top = Top;
                settingsService.Save();
            }
        }
        #endregion

        #region Implementation
        protected virtual Point GetDefaultPosition(Rect workArea)
        {
            return new Point(workArea.Right - Width, workArea.Bottom - Height);
        }

        private void ApplyOpacity(double opacity)
        {
            Opacity = Math.Clamp(opacity, 0.2, 1);

            foreach (MenuItem item in opacityMenu.Items)
            {
                item.IsChecked = (double)item.Tag == Opacity;
            }
        }

        private void RestorePosition()
        {
            var position = GetDefaultPosition(SystemParameters.WorkArea);

            if (settings.Left is double left && settings.Top is double top)
            {
                var saved = new Rect(left, top, Width, Height);
                var screen = new Rect(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop, SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
                if (screen.Contains(saved))
                {
                    position = new Point(left, top);
                }
            }

            Left = position.X;
            Top = position.Y;
        }
        #endregion

        #region Event Handlers
        private void OpacityButton_Click(object sender, RoutedEventArgs e)
        {
            opacityMenu.PlacementTarget = (UIElement)sender;
            opacityMenu.IsOpen = true;
        }

        private void OpacityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyOpacity((double)((MenuItem)sender).Tag);

            settings.Opacity = Opacity;
            settingsService.Save();
        }
        #endregion
    }
}
