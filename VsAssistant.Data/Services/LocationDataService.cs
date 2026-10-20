using System.Diagnostics;
using VsAssistant.Data.Readers.Location;

namespace VsAssistant.Data.Services
{
    /// <summary>
    /// Periodically reads the locations page and keeps the latest data.
    /// One instance is shared between all forms that show location data.
    /// Polling runs only while someone is subscribed to <see cref="DataUpdated"/>.
    /// </summary>
    public class LocationDataService
    {
        #region Constants
        private static readonly TimeSpan reloadInterval = TimeSpan.FromSeconds(20);
        #endregion

        #region Members
        private CancellationTokenSource? pollingCancellation;
        private bool isReloading;
        private EventHandler? dataUpdated;
        #endregion

        #region Events
        /// <summary>
        /// Raised after new data has been read, on the synchronization context of the first subscriber
        /// (the UI thread when subscribed from a form).
        /// The first subscriber starts polling, the last one to unsubscribe stops it.
        /// </summary>
        public event EventHandler? DataUpdated
        {
            add
            {
                dataUpdated += value;
                if (dataUpdated != null && pollingCancellation == null)
                {
                    this.Start();
                }
            }
            remove
            {
                dataUpdated -= value;
                if (dataUpdated == null)
                {
                    this.Stop();
                }
            }
        }
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

        #region Implementation
        private void Start()
        {
            pollingCancellation = new CancellationTokenSource();
            _ = this.PollAsync(pollingCancellation.Token);
        }

        private void Stop()
        {
            pollingCancellation?.Cancel();
            pollingCancellation = null;
        }

        private async Task PollAsync(CancellationToken cancellationToken)
        {
            // Awaits resume on the caller's synchronization context, so all state changes happen on the UI thread
            using var timer = new PeriodicTimer(reloadInterval);
            try
            {
                do
                {
                    await this.ReloadDataAsync();
                }
                while (await timer.WaitForNextTickAsync(cancellationToken));
            }
            catch (OperationCanceledException)
            {
                // Last subscriber has left
            }
        }

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

                dataUpdated?.Invoke(this, EventArgs.Empty);
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
