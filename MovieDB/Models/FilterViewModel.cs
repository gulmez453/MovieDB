using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class FilterViewModel
    {
        public string Search { get; set; }
        public string Category { get; set; }
        public string ProduceYearMin { get; set; }
        public string ProduceYearMax { get; set; }
        public string MinuteMin { get; set; }
        public string MinuteMax { get; set; }
        public List<bool> Rates { get; set; }
        public List<string> AllCategories { get; set; }
    }
}
