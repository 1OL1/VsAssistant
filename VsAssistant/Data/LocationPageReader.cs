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
        private static readonly TimeZoneInfo serverTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

        #region Members
        private HtmlAgilityPack.HtmlDocument doc;
        #endregion

        #region Properties
        public DateTime CaveNextAttack { get; private set; }
        public DateTime PortalNextAttack { get; private set; }
        #endregion

        #region Methods
        public void ReadData()
        {
            var web = new HtmlWeb();
            doc = web.Load("https://vsmuta.com/info/locs");

            ReadCaveNextAttack();
            ReadPortalNextAttack();
        }
        #endregion

        #region Implementation
        private void ReadCaveNextAttack()
        {
            var xPath = "/html/body/div/main/section[2]/aside/article/div[2]/div[1]/div/div/div[2]/text()[2]";
            var text = doc.DocumentNode.SelectSingleNode(xPath).InnerText;

            text = text.Trim().Replace("в ", string.Empty);

            var date = DateTime.ParseExact(text, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
            date = date.AddMinutes(2);

            CaveNextAttack = TimeZoneInfo.ConvertTime(date, serverTimeZone, TimeZoneInfo.Local);
        }

        private void ReadPortalNextAttack()
        {
            var xPath = "/html/body/div/main/section[2]/aside/article/div[2]/div[2]/div/div/div[2]/text()[2]";
            var text = doc.DocumentNode.SelectSingleNode(xPath).InnerText;

            text = text.Trim().Replace("в ", string.Empty);

            var date = DateTime.ParseExact(text, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
            date = date.AddMinutes(2);

            PortalNextAttack = TimeZoneInfo.ConvertTime(date, serverTimeZone, TimeZoneInfo.Local);
        }
        #endregion
    }
}
