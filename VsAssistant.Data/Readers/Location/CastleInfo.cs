namespace VsAssistant.Data.Readers.Location
{
    public class CastleInfo
    {
        public CastleStatus Status { get; set; } = CastleStatus.Normal;
        public ClanInfo? Owner { get; set; }

        /// <summary>
        /// Set only while an assault is requested or the battle is going on.
        /// </summary>
        public ClanInfo? Attacker { get; set; }

        /// <summary>
        /// Set only when an assault is requested.
        /// </summary>
        public DateTime? BattleStart { get; set; }

        public CastleInfo()
        {
        }

        public CastleInfo(CastleStatus status, ClanInfo? owner, ClanInfo? attacker, DateTime? battleStart)
        {
            Status = status;
            Owner = owner;
            Attacker = attacker;
            BattleStart = battleStart;
        }
    }
}
