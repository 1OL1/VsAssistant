using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using VsAssistant.Data.Readers;
using VsAssistant.Data.Readers.BloodLog;

namespace VsAssistant.BloodLog
{
    internal class BloodLogViewModel : ObservableObject, IDisposable
    {
        #region Constants
        private static readonly TimeSpan refreshInterval = TimeSpan.FromMinutes(1);
        #endregion

        #region Members
        private readonly ObservableCollection<BloodBattleViewModel> battles = new ObservableCollection<BloodBattleViewModel>();
        private readonly DispatcherTimer refreshTimer;

        private int loadVersion;
        private DateTime? loadedDate;
        private DateTime? lastUpdate;

        private DateTime selectedDate;
        private string searchText = string.Empty;
        private bool isLoading;
        private string? errorText;
        private string statusText = string.Empty;
        private string? emptyText;
        #endregion

        #region Properies
        public ICollectionView Battles { get; }

        /// <summary>
        /// Day of the log in server time; future days are not allowed by the API.
        /// </summary>
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                var date = value.Date > Today ? Today : value.Date;
                if (SetProperty(ref selectedDate, date))
                {
                    NextDayCommand.NotifyCanExecuteChanged();
                    _ = LoadAsync();
                }
            }
        }

        public DateTime MaxDate => Today;

        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    Battles.Refresh();
                    UpdateStatus();
                }
            }
        }

        public bool IsLoading
        {
            get { return isLoading; }
            private set { SetProperty(ref isLoading, value); }
        }

        public string? ErrorText
        {
            get { return errorText; }
            private set { SetProperty(ref errorText, value); }
        }

        public string StatusText
        {
            get { return statusText; }
            private set { SetProperty(ref statusText, value); }
        }

        /// <summary>
        /// Shown over the empty table; null while there are rows.
        /// </summary>
        public string? EmptyText
        {
            get { return emptyText; }
            private set { SetProperty(ref emptyText, value); }
        }

        public RelayCommand PreviousDayCommand { get; }
        public RelayCommand NextDayCommand { get; }
        public RelayCommand TodayCommand { get; }
        public RelayCommand RefreshCommand { get; }

        private static DateTime Today => ServerTime.Today.ToDateTime(TimeOnly.MinValue);
        #endregion

        public BloodLogViewModel()
        {
            Battles = CollectionViewSource.GetDefaultView(battles);
            Battles.Filter = item => string.IsNullOrWhiteSpace(SearchText) || ((BloodBattleViewModel)item).Matches(SearchText.Trim());

            PreviousDayCommand = new RelayCommand(() => SelectedDate = SelectedDate.AddDays(-1));
            NextDayCommand = new RelayCommand(() => SelectedDate = SelectedDate.AddDays(1), () => SelectedDate < Today);
            TodayCommand = new RelayCommand(() => SelectedDate = Today);
            RefreshCommand = new RelayCommand(() => _ = LoadAsync());

            SelectedDate = Today;

            refreshTimer = new DispatcherTimer { Interval = refreshInterval };
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        #region Methods
        public void Dispose()
        {
            refreshTimer.Stop();
        }
        #endregion

        #region Event Handlers
        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            // Only today's log still changes
            if (SelectedDate == Today && !IsLoading)
            {
                _ = LoadAsync();
            }

            // After midnight a new day becomes available
            OnPropertyChanged(nameof(MaxDate));
            NextDayCommand.NotifyCanExecuteChanged();
        }
        #endregion

        #region Implementation
        private async Task LoadAsync()
        {
            var version = ++loadVersion;
            var date = SelectedDate;

            IsLoading = true;
            UpdateStatus();
            try
            {
                var reader = new BloodLogReader();
                await reader.ReadDataAsync(DateOnly.FromDateTime(date));

                // A newer request was started meanwhile, e.g. the user clicked through several days
                if (version != loadVersion)
                {
                    return;
                }

                battles.Clear();
                foreach (var battle in reader.Battles)
                {
                    battles.Add(new BloodBattleViewModel(battle));
                }

                loadedDate = date;
                lastUpdate = DateTime.Now;
                ErrorText = null;
            }
            catch (Exception ex)
            {
                if (version != loadVersion)
                {
                    return;
                }

                // Rows of another day would be misleading, rows of the same day are just a bit old
                if (loadedDate != date)
                {
                    battles.Clear();
                    loadedDate = null;
                }

                ErrorText = $"Не удалось загрузить журнал: {ex.Message}";
            }
            finally
            {
                if (version == loadVersion)
                {
                    IsLoading = false;
                    UpdateStatus();
                }
            }
        }

        private void UpdateStatus()
        {
            var total = battles.Count;
            var shown = Battles.Cast<object>().Count();

            var count = shown == total ? $"Боёв: {total}" : $"Боёв: {shown} из {total}";
            var updated = lastUpdate.HasValue ? $" · обновлено в {lastUpdate:HH:mm:ss}" : string.Empty;
            StatusText = IsLoading ? "Загрузка…" : count + updated;

            EmptyText = IsLoading || ErrorText != null || shown > 0 ? null
                : total == 0 ? "За этот день боёв нет" : "Ничего не найдено";
        }
        #endregion
    }
}
