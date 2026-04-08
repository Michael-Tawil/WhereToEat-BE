namespace WhereToEat_BE.Models
{
    public class AddFavouriteRequest
    {
        public string RestaurantName { get; set; }
        public string Address { get; set; }
        public double? Rating { get; set; }
        public string? Cuisine { get; set; }
        public string? PriceRange { get; set; }
    }
}
