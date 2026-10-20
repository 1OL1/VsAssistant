using CommunityToolkit.Mvvm.ComponentModel;
using System;
using VsAssistant.Data.Readers.Location;

namespace VsAssistant.Locations
{
    internal class ResLocViewModel : ObservableObject
    {
        #region Members
        private DateTime? nextAttack;

        private ResLocationStatus status;
        private TimeSpan? timeToAttack;
        #endregion

        #region Properies
        public ResLocationStatus Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        public TimeSpan? TimeToAttack
        {
            get { return timeToAttack; }
            set { SetProperty(ref timeToAttack, value); }
        }
        #endregion

        #region Methods
        public void Update(ResLocationInfo info)
        {
            Status = info.Status;
            nextAttack = info.NextAttack;
        }

        public void CalcTime()
        {
            TimeToAttack = nextAttack - DateTime.Now;
        }
        #endregion
    }
}
