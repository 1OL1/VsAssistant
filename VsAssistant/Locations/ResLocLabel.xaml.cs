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
using VsAssistant.Data;

namespace VsAssistant.Locations
{
    /// <summary>
    /// Interaction logic for ResLocLabel.xaml
    /// </summary>
    public partial class ResLocLabel : System.Windows.Controls.Label
    {
        public ResLocationStatus Status
        {
            get { return (ResLocationStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(ResLocationStatus), typeof(ResLocLabel), new PropertyMetadata(ResLocationStatus.Free, ValueChanged));

        public TimeSpan? TimeToEvent
        {
            get { return (TimeSpan?)GetValue(TimeToEventProperty); }
            set { SetValue(TimeToEventProperty, value); }
        }

        public static readonly DependencyProperty TimeToEventProperty =
            DependencyProperty.Register("TimeToEvent", typeof(TimeSpan?), typeof(ResLocLabel), new PropertyMetadata(null, ValueChanged));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ResLocLabel)d).UpdateView();
        }

        public ResLocLabel()
        {
            InitializeComponent();
            UpdateView();
        }

        private void UpdateView()
        {
            switch (Status)
            {
                case ResLocationStatus.Free:
                    Content = "Свбна";
                    VisualStateManager.GoToElementState(this, "Free", false);
                    break;

                case ResLocationStatus.Battle:
                    Content = "Битва";
                    VisualStateManager.GoToElementState(this, "Battle", false);
                    break;

                case ResLocationStatus.Occupied when TimeToEvent.HasValue:
                    var time = TimeToEvent.Value;
                    Content = time.ToString(@"mm\:ss");

                    if (time < TimeSpan.FromMinutes(2))
                    {
                        VisualStateManager.GoToElementState(this, "Alert", false);
                    }
                    else
                    {
                        VisualStateManager.GoToElementState(this, "Warning", false);
                    }
                    break;

                default:
                    Content = "Занята";
                    VisualStateManager.GoToElementState(this, "Occupied", false);
                    break;
            }
        }
    }
}
