using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Models.TmdbPeople
{
    public class Profile
    {
        public string iso_639_1 { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int vote_count { get; set; }
        public double vote_average { get; set; }
        public string file_path { get; set; }
        public double aspect_ration { get; set; }
    }
}
