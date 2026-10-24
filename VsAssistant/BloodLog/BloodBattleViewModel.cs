using System;
using System.Collections.Generic;
using System.Linq;
using VsAssistant.Data.Readers.BloodLog;

namespace VsAssistant.BloodLog
{
    internal class BloodBattleViewModel
    {
        #region Properies
        public DateTime Start { get; }
        public TimeSpan Duration { get; }

        public string TimeText { get; }
        public string DurationText { get; }

        public IReadOnlyList<BloodMemberViewModel> Team1 { get; }
        public IReadOnlyList<BloodMemberViewModel> Team2 { get; }

        public bool IsTeam1Winner { get; }
        public bool IsTeam2Winner { get; }

        public Uri BattleLogUrl { get; }
        #endregion

        public BloodBattleViewModel(BloodBattleInfo info)
        {
            Start = info.Start;
            Duration = info.End - info.Start;

            TimeText = $"{info.Start:HH:mm}";
            // Times come in whole minutes, so a short fight shows as zero
            DurationText = Duration.TotalMinutes < 1 ? "<1 мин" : $"{(int)Duration.TotalMinutes} мин";

            Team1 = info.Members.Where(m => m.Team == 1).Select(m => new BloodMemberViewModel(m)).ToList();
            Team2 = info.Members.Where(m => m.Team == 2).Select(m => new BloodMemberViewModel(m)).ToList();

            IsTeam1Winner = info.WinnerTeam == 1;
            IsTeam2Winner = info.WinnerTeam == 2;

            BattleLogUrl = new Uri($"https://vsmuta.com/battlelog/{info.Id}");
        }

        #region Methods
        public bool Matches(string text)
        {
            return Team1.Concat(Team2).Any(member => member.Matches(text));
        }
        #endregion
    }

    internal class BloodMemberViewModel
    {
        #region Properies
        public string Name { get; }
        public string? ClanName { get; }
        public string? LogoUrl { get; }
        public Uri ProfileUrl { get; }
        #endregion

        public BloodMemberViewModel(BloodBattleMember member)
        {
            Name = member.Name;
            ClanName = member.Clan?.Name;
            LogoUrl = member.Clan?.LogoUrl;
            ProfileUrl = new Uri($"https://vsmuta.com/user/{Uri.EscapeDataString(member.Name)}");
        }

        #region Methods
        public bool Matches(string text)
        {
            var search = Normalize(text);
            return Normalize(Name).Contains(search) || Normalize(ClanName ?? string.Empty).Contains(search);
        }
        #endregion

        #region Implementation
        // Punctuation and spaces are ignored, so "nip" finds the clan "N.I.P"
        private static string Normalize(string text)
        {
            return new string(text.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
        }
        #endregion
    }
}
