using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VsAssistant.Locations
{
    internal class LocViewModel : ObservableObject
    {
        #region Members
        private DateTime caveNextAttack;
        private DateTime portalNextAttack;
        private DateTime lastTimerReload = DateTime.Now;

        private TimeSpan timeToCave;
        private TimeSpan timeToPortal;
        #endregion

        #region Properies
        public TimeSpan TimeToCave
        {
            get { return timeToCave; }
            set { SetProperty(ref timeToCave, value); }
        }

        public TimeSpan TimeToPortal 
        {
            get { return timeToPortal; }
            set { SetProperty(ref timeToPortal, value); }
        }
        #endregion

        public LocViewModel()
        {
            this.ReloadData();
            this.CalcTimes();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMicroseconds(250);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        #region Event Handlers
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if ((DateTime.Now - lastTimerReload) > TimeSpan.FromSeconds(10) && (this.TimeToCave >= TimeSpan.Zero || this.TimeToPortal <= TimeSpan.Zero)
                || (DateTime.Now - lastTimerReload) > TimeSpan.FromMinutes(10))
            {
                this.ReloadData();
                lastTimerReload = DateTime.Now;
            }

            this.CalcTimes();
        }
        #endregion

        #region Implementation
        private void ReloadData()
        {
            var dataReader = new LocDataReader();
            dataReader.ReadData();

            caveNextAttack = dataReader.CaveNextAttack;
            portalNextAttack = dataReader.PortalNextAttack;
        }

        private void CalcTimes()
        {
            this.TimeToCave = caveNextAttack - DateTime.Now > TimeSpan.Zero ? caveNextAttack - DateTime.Now : TimeSpan.Zero;
            this.TimeToPortal = portalNextAttack - DateTime.Now > TimeSpan.Zero ? portalNextAttack - DateTime.Now : TimeSpan.Zero;
        }
        #endregion
    }
}
