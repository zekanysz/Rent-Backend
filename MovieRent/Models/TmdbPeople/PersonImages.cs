using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Models.TmdbPeople
{
    public class PersonImages
    {
        public int id { get; set; }
        public List<Profile> profiles { get; set; }
    }
}
