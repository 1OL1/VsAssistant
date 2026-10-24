using VsAssistant.Data.Readers.Location;

namespace VsAssistant.Data.Readers.BloodLog
{
    public class BloodBattleInfo
    {
        public Guid Id { get; set; }

        public int WinnerTeam { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public List<BloodBattleMember> Members { get; set; } = new List<BloodBattleMember>();
    }

    public class BloodBattleMember
    {
        public string Name { get; set; } = string.Empty;
        public int Team { get; set; }

        public ClanInfo? Clan { get; set; }
    }
}
