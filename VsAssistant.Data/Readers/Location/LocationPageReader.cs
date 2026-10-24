using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VsAssistant.Data.Readers.Location
{
    public class LocationPageReader
    {
        #region Members
        private HtmlAgilityPack.HtmlDocument doc;
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

        public CastleInfo FearCastle { get; private set; } = new CastleInfo();
        public CastleInfo GhostCastle { get; private set; } = new CastleInfo();
        public CastleInfo SteelCastle { get; private set; } = new CastleInfo();
        public CastleInfo WhiteStoneCastle { get; private set; } = new CastleInfo();
        #endregion

        #region Methods
        public void ReadData()
        {
            var web = new HtmlWeb();
            doc = web.Load("https://vsmuta.com/info/locs");

            ReadCaveData();
            ReadPortalData();

            ReadFarForestData();
            ReadNearForestData();
            ReadNortSwampData();
            ReadSoutSwampData();
            ReadWestMineData();
            ReadNearMineData();
            ReadLakeData();

            ReadFearCastleData();
            ReadGhostCastleData();
            ReadSteelCastleData();
            ReadWhiteStoneCastleData();
        }
        #endregion

        #region Implementation
        private void ReadCaveData()
        {
            var xPath = "/html/body/div/main/section[2]/aside/article/div[2]/div[1]/div/div/div[2]/text()[2]";
            var date = ReadAttackDate(xPath)!.Value;
            date = date.AddMinutes(2);

            CaveNextAttack = date;
        }

        private void ReadPortalData()
        {
            var xPath = "/html/body/div/main/section[2]/aside/article/div[2]/div[2]/div/div/div[2]/text()[2]";
            var date = ReadAttackDate(xPath)!.Value;
            date = date.AddMinutes(2);

            PortalNextAttack = date;
        }

        private void ReadFarForestData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[1]/div/div/div[3]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[1]/div/div/div[3]/div[1]/text()[2]";

            FarForest = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadNearForestData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[2]/div/div/div[3]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[2]/div/div/div[3]/div[1]/text()[2]";

            NearForest = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadNortSwampData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[3]/div/div/div[3]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[3]/div/div/div[3]/div[1]/text()[2]";

            NortSwamp = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadSoutSwampData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[4]/div/div/div[3]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[4]/div/div/div[3]/div[1]/text()[2]";

            SoutSwamp = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadWestMineData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[5]/div/div/div[3]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[5]/div/div/div[3]/div[1]/text()[2]";

            WestMine = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadNearMineData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[6]/div/div/div[3]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[6]/div/div/div[3]/div[1]/text()[2]";

            NearMine = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadLakeData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[7]/div/div/div[3]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[7]/div/div/div[3]/div[1]/text()[2]";

            Lake = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadFearCastleData()
        {
            var ownerLogoPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[1]/div[1]/img";
            var attackerLogoPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[1]/div[3]/img";
            var statusPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[1]/div[2]/div[2]";

            FearCastle = ReadCastleInfo(ownerLogoPath, attackerLogoPath, statusPath);
        }

        private void ReadGhostCastleData()
        {
            var ownerLogoPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[3]/div[1]/img";
            var attackerLogoPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[3]/div[3]/img";
            var statusPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[3]/div[2]/div[2]";

            GhostCastle = ReadCastleInfo(ownerLogoPath, attackerLogoPath, statusPath);
        }

        private void ReadSteelCastleData()
        {
            var ownerLogoPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[5]/div[1]/img";
            var attackerLogoPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[5]/div[3]/img";
            var statusPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[5]/div[2]/div[2]";

            SteelCastle = ReadCastleInfo(ownerLogoPath, attackerLogoPath, statusPath);
        }

        private void ReadWhiteStoneCastleData()
        {
            var ownerLogoPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[7]/div[1]/img";
            var attackerLogoPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[7]/div[3]/img";
            var statusPath = "/html/body/div/main/section[1]/aside[2]/article[2]/div[7]/div[2]/div[2]";

            WhiteStoneCastle = ReadCastleInfo(ownerLogoPath, attackerLogoPath, statusPath);
        }

        private CastleInfo ReadCastleInfo(string ownerLogoPath, string attackerLogoPath, string statusPath)
        {
            var ownerId = ReadClanId(ownerLogoPath);
            var attackerId = ReadClanId(attackerLogoPath);
            var text = HtmlEntity.DeEntitize(doc.DocumentNode.SelectSingleNode(statusPath)?.InnerText ?? string.Empty).Trim();

            var startMatch = Regex.Match(text, @"Битва начнется через\s*(\d+)\s*мин");
            var status = startMatch.Success ? CastleStatus.AssaultRequested
                : text.Contains("Идет битва") ? CastleStatus.Battle : CastleStatus.Normal;

            // The page shows whole minutes only, so the battle can start up to a minute later
            DateTime? battleStart = startMatch.Success ? DateTime.Now.AddMinutes(int.Parse(startMatch.Groups[1].Value)) : null;

            // While the castle is not attacked, the status line holds the owner clan name
            var ownerName = status == CastleStatus.Normal && text.Length > 0 ? text : null;

            var owner = ownerId.HasValue ? new ClanInfo(ownerId.Value, ownerName) : null;
            var attacker = attackerId.HasValue ? new ClanInfo(attackerId.Value, null) : null;

            return new CastleInfo(status, owner, attacker, battleStart);
        }

        private int? ReadClanId(string logoPath)
        {
            // Clan id is the logo file name, e.g. https://vsmuta.com/images/clan/36.png
            var src = doc.DocumentNode.SelectSingleNode(logoPath)?.GetAttributeValue("src", string.Empty);
            var match = Regex.Match(src ?? string.Empty, @"(\d+)\.png$");

            return match.Success ? int.Parse(match.Groups[1].Value) : null;
        }

        private ResLocationInfo ReadResLocInfo(string statusPath, string attackDatePath)
        {
            DateTime? date = ReadAttackDate(attackDatePath);
            var text = doc.DocumentNode.SelectSingleNode(statusPath).InnerText;

            var status = text.Contains("Локация занята монстрами") || date.HasValue ? ResLocationStatus.Occupied
                : text.Contains("Идет битва") ? ResLocationStatus.Battle : ResLocationStatus.Free;

            return new ResLocationInfo(status, date);
        }

        private DateTime? ReadAttackDate(string xPath)
        {
            var text = doc.DocumentNode.SelectSingleNode(xPath)?.InnerText;

            return ServerTime.ParseToLocal(text?.Replace("в ", string.Empty));
        }
        #endregion
    }
}
