using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using VsAssistant.Data.Services;

namespace VsAssistant.Locations
{
    internal class LocViewModel : ObservableObject, IDisposable
    {
        #region Members
        private readonly LocationDataService dataService;
        private readonly DispatcherTimer timer;

        private DateTime caveNextAttack;
        private DateTime portalNextAttack;

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

        public LocViewModel(LocationDataService dataService)
        {
            this.dataService = dataService;
            this.dataService.DataUpdated += DataService_DataUpdated;

            this.UpdateData();
            this.CalcTimes();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        #region Methods
        public void Dispose()
        {
            timer.Stop();
            dataService.DataUpdated -= DataService_DataUpdated;
        }
        #endregion

        #region Event Handlers
        private void Timer_Tick(object? sender, EventArgs e)
        {
            this.CalcTimes();
        }

        private void DataService_DataUpdated(object? sender, EventArgs e)
        {
            this.UpdateData();
            this.CalcTimes();
        }
        #endregion

        #region Implementation
        private void UpdateData()
        {
            caveNextAttack = dataService.CaveNextAttack;
            portalNextAttack = dataService.PortalNextAttack;

            FarForest.Update(dataService.FarForest);
            NearForest.Update(dataService.NearForest);
            NortSwamp.Update(dataService.NortSwamp);
            SoutSwamp.Update(dataService.SoutSwamp);
            WestMine.Update(dataService.WestMine);
            NearMine.Update(dataService.NearMine);
            Lake.Update(dataService.Lake);
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
