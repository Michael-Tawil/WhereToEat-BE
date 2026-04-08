namespace WhereToEat_BE.Models
{
    public class Favourite
    {
        public Guid? Id {  get; set; }
        public Guid UserId { get; set; }
        public string RestaurantName { get; set; }
        public string Address { get; set; }
        public double? Rating { get; set; }
        public string? Cuisine { get; set; }
        public string? PriceRange { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
