using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Models.TmdbMovieCredits
{
    public class Crew
    {
        public string credit_id { get; set; }
        public string department { get; set; }
        public int gender { get; set; }
        public int id { get; set; }
        public string job { get; set; }
        public string name { get; set; }
        public string profile_path { get; set; }
    }
}
