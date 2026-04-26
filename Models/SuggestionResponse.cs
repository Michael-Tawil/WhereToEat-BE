namespace WhereToEat_BE.Models
{
    public class SuggestionResponse
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; }
        public string Cuisine { get; set; }
        public string PriceRange { get; set; }
        public string Reason { get; set; }

        public string? GoogleMapsUri { get; set; }
    }
}
