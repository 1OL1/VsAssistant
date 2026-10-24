using System.Net.Http.Json;
using System.Text.Json.Serialization;
using VsAssistant.Data.Readers.Location;

namespace VsAssistant.Data.Readers.BloodLog
{
    /// <summary>
    /// Reads the blood log of one day; the API can filter only by date.
    /// </summary>
    public class BloodLogReader
    {
        #region Constants
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://vsmuta.com/api/") };
        #endregion

        #region Properties
        public List<BloodBattleInfo> Battles { get; private set; } = new List<BloodBattleInfo>();
        #endregion

        #region Methods
        public async Task ReadDataAsync(DateOnly serverDate, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetFromJsonAsync<BloodLogResponse>($"logs/blood/{serverDate:yyyy-MM-dd}", cancellationToken)
                ?? throw new InvalidOperationException("Blood log API returned an empty response");

            // The API returns errors with HTTP 200 as well
            if (response.Status != "success")
            {
                throw new InvalidOperationException($"Blood log API failed for {serverDate:yyyy-MM-dd}: {response.Message}");
            }

            Battles = response.Battles.Select(ReadBattleInfo).ToList();
        }
        #endregion

        #region Implementation
        private static BloodBattleInfo ReadBattleInfo(BattleDto battle)
        {
            return new BloodBattleInfo
            {
                Id = battle.Uuid,
                WinnerTeam = battle.TeamWin,
                Start = ServerTime.ParseToLocal(battle.TimeStart) ?? throw new FormatException($"Unexpected battle start '{battle.TimeStart}'"),
                End = ServerTime.ParseToLocal(battle.TimeEnd) ?? throw new FormatException($"Unexpected battle end '{battle.TimeEnd}'"),
                Members = battle.Members.Select(member => new BloodBattleMember
                {
                    Name = member.Title,
                    Team = member.Team,
                    Clan = member.ClanId is int clanId and > 0 ? new ClanInfo(clanId, member.Clan) : null
                }).ToList()
            };
        }
        #endregion

        #region API Contract
        private class BloodLogResponse
        {
            [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
            [JsonPropertyName("message")] public string? Message { get; set; }
            [JsonPropertyName("battles")] public List<BattleDto> Battles { get; set; } = new List<BattleDto>();
        }

        private class BattleDto
        {
            [JsonPropertyName("uuid")] public Guid Uuid { get; set; }
            [JsonPropertyName("team_win")] public int TeamWin { get; set; }
            [JsonPropertyName("time_start")] public string TimeStart { get; set; } = string.Empty;
            [JsonPropertyName("time_end")] public string TimeEnd { get; set; } = string.Empty;
            [JsonPropertyName("members")] public List<MemberDto> Members { get; set; } = new List<MemberDto>();
        }

        private class MemberDto
        {
            [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;
            [JsonPropertyName("team")] public int Team { get; set; }
            [JsonPropertyName("clan_id")] public int? ClanId { get; set; }
            [JsonPropertyName("clan")] public string? Clan { get; set; }
        }
        #endregion
    }
}
