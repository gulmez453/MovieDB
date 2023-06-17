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
        public bool Rate1 { get; set; }
        public bool Rate2 { get; set; }
        public bool Rate3 { get; set; }
        public bool Rate4 { get; set; }
        public bool Rate5 { get; set; }


    }
}
