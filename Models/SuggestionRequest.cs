namespace WhereToEat_BE.Models
{
    public class SuggestionRequest
    {
        public string SearchText { get; set; }
        public string Location { get; set; }

        public string Cuisine { get; set; }
    }
}
