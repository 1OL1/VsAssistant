using System.Diagnostics;
using System.Windows.Threading;

namespace VsAssistant.Data
{
    /// <summary>
    /// Periodically reads the locations page and keeps the latest data.
    /// One instance is shared between all forms that show location data.
    /// </summary>
    internal class LocationDataService
    {
        #region Constants
        private static readonly TimeSpan reloadInterval = TimeSpan.FromSeconds(20);
        #endregion

        #region Members
        private readonly DispatcherTimer timer;
        private bool isReloading;
        #endregion

        #region Events
        /// <summary>
        /// Raised on the UI thread after new data has been read.
        /// </summary>
        public event EventHandler? DataUpdated;
        #endregion

        #region Properties
        public DateTime CaveNextAttack { get; private set; }
        public DateTime PortalNextAttack { get; private set; }

        public ResLocationInfo FarForest { get; private set; } = new ResLocationInfo();
        public ResLocationInfo NearForest { get; private set; } = new ResLocationInfo();
        public ResLocationInfo NortSwamp { get; private set; } = new ResLocationInfo();
        public ResLocationInfo SoutSwamp { get; private set; } = new ResLocationInfo();
        public ResLocationInfo WestMine { get; private set; } = new ResLocationInfo();
        public ResLocationInfo NearMine { get; private set; } = new ResLocationInfo();
        public ResLocationInfo Lake { get; private set; } = new ResLocationInfo();

        public DateTime? LastUpdate { get; private set; }
        #endregion

        public LocationDataService()
        {
            timer = new DispatcherTimer();
            timer.Interval = reloadInterval;
            timer.Tick += Timer_Tick;
        }

        #region Methods
        public void Start()
        {
            timer.Start();
            _ = this.ReloadDataAsync();
        }

        public void Stop()
        {
            timer.Stop();
        }
        #endregion

        #region Event Handlers
        private async void Timer_Tick(object? sender, EventArgs e)
        {
            await this.ReloadDataAsync();
        }
        #endregion

        #region Implementation
        private async Task ReloadDataAsync()
        {
            if (isReloading)
            {
                return;
            }

            isReloading = true;
            try
            {
                var dataReader = new LocationPageReader();
                await Task.Run(dataReader.ReadData);

                // Continuation runs on the UI thread, so subscribers can update bound properties directly
                CaveNextAttack = dataReader.CaveNextAttack;
                PortalNextAttack = dataReader.PortalNextAttack;

                FarForest = dataReader.FarForest;
                NearForest = dataReader.NearForest;
                NortSwamp = dataReader.NortSwamp;
                SoutSwamp = dataReader.SoutSwamp;
                WestMine = dataReader.WestMine;
                NearMine = dataReader.NearMine;
                Lake = dataReader.Lake;

                LastUpdate = DateTime.Now;

                DataUpdated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Keep the previous data and try again on the next tick
                Debug.WriteLine($"Failed to read locations page: {ex}");
            }
            finally
            {
                isReloading = false;
            }
        }
        #endregion
    }
}
