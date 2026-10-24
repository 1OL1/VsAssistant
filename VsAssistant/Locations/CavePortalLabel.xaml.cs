using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VsAssistant.Locations
{
    public partial class CavePortalLabel : System.Windows.Controls.Label
    {
        public TimeSpan TimeToEvent
        {
            get { return (TimeSpan)GetValue(TimeToEventProperty); }
            set { SetValue(TimeToEventProperty, value); }
        }

        public static readonly DependencyProperty TimeToEventProperty =
            DependencyProperty.Register("TimeToEvent", typeof(TimeSpan), typeof(CavePortalLabel),  new PropertyMetadata(TimeToEventChanged));

        private static void TimeToEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CavePortalLabel)d).UpdateView();
        }

        public CavePortalLabel()
        {
            InitializeComponent();

            Loaded += (_, _) => UpdateView();
        }

        private void UpdateView()
        {
            var time = TimeToEvent;

            Content = (time < TimeSpan.Zero ? "-" : string.Empty) + time.ToString(@"hh\:mm\:ss");

            if (time < TimeSpan.Zero)
            {
                GoToState("AlertNoBlink");
            }
            else if (time < TimeSpan.FromMinutes(2))
            {
                GoToState("Alert");
            }
            else if (time < TimeSpan.FromMinutes(15))
            {
                GoToState("Warning");
            }
            else
            {
                GoToState("Normal");
            }
        }

        private void GoToState(string state)
        {
            // Before loading, the window style still replaces Foreground and would discard the animated color
            if (IsLoaded)
            {
                VisualStateManager.GoToElementState(this, state, false);
            }
        }
    }
}
