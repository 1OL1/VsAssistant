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

        #region Properties
        public static readonly DependencyProperty IsResizableProperty =
            DependencyProperty.Register(nameof(IsResizable), typeof(bool), typeof(BaseWindow), new PropertyMetadata(false));

        /// <summary>
        /// Shows a resize grip in the bottom-right corner; the size is kept in settings.
        /// </summary>
        public bool IsResizable
        {
            get { return (bool)GetValue(IsResizableProperty); }
            set { SetValue(IsResizableProperty, value); }
        }

        /// <summary>
        /// When false the window always opens at its default position.
        /// </summary>
        public bool RemembersPosition { get; set; } = true;
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

            // Resized by hand rather than with ResizeMode, which would let Windows snap the window to screen edges
            if (GetTemplateChild("PART_ResizeGrip") is Thumb resizeGrip)
            {
                resizeGrip.DragDelta += ResizeGrip_DragDelta;
                resizeGrip.DragCompleted += ResizeGrip_DragCompleted;
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            ApplyOpacity(settings.Opacity);
            RestoreSize();
            RestorePosition();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();

                if (RemembersPosition)
                {
                    settings.Left = Left;
                    settings.Top = Top;
                    settingsService.Save();
                }
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

        private void RestoreSize()
        {
            if (IsResizable && settings.Width is double width && settings.Height is double height)
            {
                var workArea = SystemParameters.WorkArea;
                Width = Math.Min(Math.Max(width, MinWidth), workArea.Width);
                Height = Math.Min(Math.Max(height, MinHeight), workArea.Height);
            }
        }

        private void RestorePosition()
        {
            var position = GetDefaultPosition(SystemParameters.WorkArea);

            if (RemembersPosition && settings.Left is double left && settings.Top is double top)
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

        private void ResizeGrip_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Width = Math.Max(MinWidth, Width + e.HorizontalChange);
            Height = Math.Max(MinHeight, Height + e.VerticalChange);
        }

        private void ResizeGrip_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            settings.Width = Width;
            settings.Height = Height;
            settingsService.Save();
        }
        #endregion
    }
}
