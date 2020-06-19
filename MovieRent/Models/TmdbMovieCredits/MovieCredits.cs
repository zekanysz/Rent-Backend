using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Models.TmdbMovieCredits
{
    public class MovieCredits
    {
        public int id { get; set; }
        public List<Cast> cast { get; set; }
        public List<Crew> crew { get; set; }

    }
}
