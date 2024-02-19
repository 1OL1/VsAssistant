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
            var control = (CavePortalLabel)d;
            var time = (TimeSpan)e.NewValue;

            control.Content = (time < TimeSpan.Zero ? "-" : string.Empty) + time.ToString(@"hh\:mm\:ss");

            if (time < TimeSpan.Zero)
            {
                VisualStateManager.GoToElementState(control, "AlertNoBlink", false);
            }
            else if (time < TimeSpan.FromMinutes(2))
            {
                VisualStateManager.GoToElementState(control, "Alert", false);
            }
            else if (time < TimeSpan.FromMinutes(15))
            {
                VisualStateManager.GoToElementState(control, "Warning", false);
            }
            else
            {
                VisualStateManager.GoToElementState(control, "Normal", false);
            }
        }

        public CavePortalLabel()
        {
            InitializeComponent();
        }
    }
}
