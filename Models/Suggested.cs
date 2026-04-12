namespace WhereToEat_BE.Models
{
    public class Suggested
    {
        public Guid? Id { get; set; }

        public Guid UserId { get; set; }

        public string RestaurantName { get; set; }

        public DateTime? SuggestedAt { get; set; }
    }
}
