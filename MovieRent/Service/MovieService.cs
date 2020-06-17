using MongoDB.Driver;
using MovieRent.Data;
using MovieRent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Service
{
    public class MovieService
    {

        private IMongoCollection<Movie> _movies;

        public MovieService(IMovieDBSettings settings)
        {

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _movies = database.GetCollection<Movie>(settings.MoviesCollectionName);
        }

        public List<Movie> GetAll() =>
           _movies.Find(Movie => true).ToList();

        public Movie GetMovieByImdbId(string imdbId)
        {
            return _movies.Find<Movie>(Movie => Movie.imdbID == imdbId).FirstOrDefault();
        }

        public bool IsItExistsByTitle(string title)
        {
            return _movies.Find<Movie>(Movie => Movie.Title == title).Any();
        }

        public bool IsItExistsByIMDBId(string imdbId)
        {
            return _movies.Find<Movie>(Movie => Movie.imdbID == imdbId).Any();
        }

        public Movie Create(Movie movie)
        {
            _movies.InsertOne(movie);
            return movie;
        }

    }
}
