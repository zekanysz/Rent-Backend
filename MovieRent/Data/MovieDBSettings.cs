using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Data
{
    public class MovieDBSettings : IMovieDBSettings
    {
        public string UsersCollectionName { get; set; }
        public string MoviesCollectionName { get; set; }
        public string CreditsCollectionName { get; set; }
        public string PersonImagesCollectionName { get; set; }
        public string RentsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IMovieDBSettings
    {
        string UsersCollectionName { get; set; }
        string MoviesCollectionName { get; set; }
        string CreditsCollectionName { get; set; }
        string PersonImagesCollectionName { get; set; }
        string RentsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }

    }
}
