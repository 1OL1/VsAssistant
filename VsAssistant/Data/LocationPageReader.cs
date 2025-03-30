using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsAssistant.Data
{
    internal class LocationPageReader
    {
        #region Constants
        private static readonly TimeZoneInfo serverTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        #endregion

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
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[1]/div/div/div[2]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[1]/div/div/div[2]/div[1]/text()[2]";

            FarForest = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadNearForestData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[2]/div/div/div[2]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[2]/div/div/div[2]/div[1]/text()[2]";

            NearForest = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadNortSwampData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[3]/div/div/div[2]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[3]/div/div/div[2]/div[1]/text()[2]";

            NortSwamp = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadSoutSwampData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[4]/div/div/div[2]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[4]/div/div/div[2]/div[1]/text()[2]";

            SoutSwamp = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadWestMineData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[5]/div/div/div[2]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[5]/div/div/div[2]/div[1]/text()[2]";

            WestMine = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadNearMineData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[6]/div/div/div[2]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[6]/div/div/div[2]/div[1]/text()[2]";

            NearMine = ReadResLocInfo(statusPath, attackDatePath);
        }

        private void ReadLakeData()
        {
            var statusPath = "/html/body/div/main/section[2]/aside/article/div[3]/div[7]/div/div/div[2]/div";
            var attackDatePath = "/html/body/div/main/section[2]/aside/article/div[3]/div[7]/div/div/div[2]/div[1]/text()[2]";

            Lake = ReadResLocInfo(statusPath, attackDatePath);
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

            DateTime? date = null;
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Trim().Replace("в ", string.Empty);
                if (DateTime.TryParseExact(text, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    date = TimeZoneInfo.ConvertTime(parsedDate, serverTimeZone, TimeZoneInfo.Local);
                }
            }

            return date;
        }
        #endregion
    }
}
