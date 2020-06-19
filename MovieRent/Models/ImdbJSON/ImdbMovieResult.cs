using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Models.ImdbJSON
{
    public class ImdbMovieResult
    {
        public List<MovieResult> movie_results { get; set; }
    }
}
