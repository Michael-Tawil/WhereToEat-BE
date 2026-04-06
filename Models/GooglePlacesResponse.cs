namespace WhereToEat_BE.Models
{
    public class GooglePlacesResponse
    {
        public List<GooglePlace> Places { get; set; }
    }

    public class GooglePlace
    {
        public DisplayName DisplayName { get; set; }
        public string FormattedAddress { get; set; }
        public double Rating { get; set; }
        public string PrimaryType { get; set; }
        public string PriceLevel { get; set; }
    }

    public class DisplayName
    {
        public string Text { get; set; }
    }
}
