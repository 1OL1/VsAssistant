using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using VsAssistant.Data;

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

        public ResLocViewModel FarForest { get; } = new ResLocViewModel();
        public ResLocViewModel NearForest { get; } = new ResLocViewModel();
        public ResLocViewModel NortSwamp { get; } = new ResLocViewModel();
        public ResLocViewModel SoutSwamp { get; } = new ResLocViewModel();
        public ResLocViewModel WestMine { get; } = new ResLocViewModel();
        public ResLocViewModel NearMine { get; } = new ResLocViewModel();
        public ResLocViewModel Lake { get; } = new ResLocViewModel();

        private IEnumerable<ResLocViewModel> ResLocations => new[] { FarForest, NearForest, NortSwamp, SoutSwamp, WestMine, NearMine, Lake };
        #endregion

        public LocViewModel()
        {
            this.ReloadData();
            this.CalcTimes();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMicroseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        #region Event Handlers
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if ((DateTime.Now - lastTimerReload) > TimeSpan.FromSeconds(20))
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
            var dataReader = new LocationPageReader();
            dataReader.ReadData();

            caveNextAttack = dataReader.CaveNextAttack;
            portalNextAttack = dataReader.PortalNextAttack;

            FarForest.Update(dataReader.FarForest);
            NearForest.Update(dataReader.NearForest);
            NortSwamp.Update(dataReader.NortSwamp);
            SoutSwamp.Update(dataReader.SoutSwamp);
            WestMine.Update(dataReader.WestMine);
            NearMine.Update(dataReader.NearMine);
            Lake.Update(dataReader.Lake);
        }

        private void CalcTimes()
        {
            this.TimeToCave = caveNextAttack - DateTime.Now;
            this.TimeToPortal = portalNextAttack - DateTime.Now;

            foreach (var resLocation in ResLocations)
            {
                resLocation.CalcTime();
            }
        }
        #endregion
    }
}
