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
using VsAssistant.Data.Readers.Location;

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

            Loaded += (_, _) => UpdateView();
        }

        private void UpdateView()
        {
            switch (Status)
            {
                case ResLocationStatus.Free:
                    Content = "Свбна";
                    GoToState("Free");
                    break;

                case ResLocationStatus.Battle:
                    Content = "Битва";
                    GoToState("Battle");
                    break;

                case ResLocationStatus.Occupied when TimeToEvent.HasValue:
                    var time = TimeToEvent.Value;
                    Content = time.ToString(@"mm\:ss");

                    if (time < TimeSpan.FromMinutes(2))
                    {
                        GoToState("Alert");
                    }
                    else
                    {
                        GoToState("Warning");
                    }
                    break;

                default:
                    Content = "Занята";
                    GoToState("Occupied");
                    break;
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
