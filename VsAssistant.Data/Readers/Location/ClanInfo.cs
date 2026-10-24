namespace VsAssistant.Data.Readers.Location
{
    public class ClanInfo
    {
        public int Id { get; set; }

        /// <summary>
        /// Null when the page shows only the clan logo.
        /// </summary>
        public string? Name { get; set; }

        public string LogoUrl => $"https://vsmuta.com/images/clan/{Id}.png";

        public ClanInfo(int id, string? name)
        {
            Id = id;
            Name = name;
        }
    }
}
