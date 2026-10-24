using CommunityToolkit.Mvvm.ComponentModel;
using System;
using VsAssistant.Data.Readers.Location;

namespace VsAssistant.Castles
{
    internal class CastleViewModel : ObservableObject
    {
        #region Members
        private DateTime? battleStart;
        private string? ownerName;

        private CastleStatus status;
        private string? ownerLogoUrl;
        private string? attackerLogoUrl;
        private string statusText = string.Empty;
        private bool isBattleSoon;
        #endregion

        #region Properies
        public string Name { get; }

        public CastleStatus Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        public string? OwnerLogoUrl
        {
            get { return ownerLogoUrl; }
            set { SetProperty(ref ownerLogoUrl, value); }
        }

        public string? AttackerLogoUrl
        {
            get { return attackerLogoUrl; }
            set { SetProperty(ref attackerLogoUrl, value); }
        }

        public string StatusText
        {
            get { return statusText; }
            set { SetProperty(ref statusText, value); }
        }

        public bool IsBattleSoon
        {
            get { return isBattleSoon; }
            set { SetProperty(ref isBattleSoon, value); }
        }
        #endregion

        public CastleViewModel(string name)
        {
            Name = name;
        }

        #region Methods
        public void Update(CastleInfo info)
        {
            Status = info.Status;
            OwnerLogoUrl = info.Owner?.LogoUrl;
            AttackerLogoUrl = info.Attacker?.LogoUrl;

            ownerName = info.Owner?.Name;
            battleStart = info.BattleStart;
        }

        public void CalcTime()
        {
            StatusText = Status switch
            {
                CastleStatus.AssaultRequested => $"⏳ {MinutesToBattle():00} мин.",
                CastleStatus.Battle => "⚔ Битва",
                _ => ownerName ?? string.Empty
            };

            IsBattleSoon = Status == CastleStatus.AssaultRequested && battleStart - DateTime.Now < TimeSpan.FromMinutes(1);
        }
        #endregion

        #region Implementation
        private int MinutesToBattle()
        {
            var minutes = Math.Ceiling(((battleStart ?? DateTime.Now) - DateTime.Now).TotalMinutes);
            return (int)Math.Max(minutes, 0);
        }
        #endregion
    }
}
