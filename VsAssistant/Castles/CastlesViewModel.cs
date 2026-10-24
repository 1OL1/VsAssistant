using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Windows.Threading;
using VsAssistant.Data.Services;

namespace VsAssistant.Castles
{
    internal class CastlesViewModel : ObservableObject, IDisposable
    {
        #region Members
        private readonly LocationDataService dataService;
        private readonly DispatcherTimer timer;
        #endregion

        #region Properies
        public CastleViewModel FearCastle { get; } = new CastleViewModel("Страх");
        public CastleViewModel GhostCastle { get; } = new CastleViewModel("Привы");
        public CastleViewModel SteelCastle { get; } = new CastleViewModel("Сталь");
        public CastleViewModel WhiteStoneCastle { get; } = new CastleViewModel("БК");

        public IReadOnlyList<CastleViewModel> Castles { get; }
        #endregion

        public CastlesViewModel(LocationDataService dataService)
        {
            this.dataService = dataService;
            this.Castles = new[] { FearCastle, GhostCastle, SteelCastle, WhiteStoneCastle };

            this.dataService.DataUpdated += DataService_DataUpdated;

            this.UpdateData();
            this.CalcTimes();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
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
            FearCastle.Update(dataService.FearCastle);
            GhostCastle.Update(dataService.GhostCastle);
            SteelCastle.Update(dataService.SteelCastle);
            WhiteStoneCastle.Update(dataService.WhiteStoneCastle);
        }

        private void CalcTimes()
        {
            foreach (var castle in Castles)
            {
                castle.CalcTime();
            }
        }
        #endregion
    }
}
