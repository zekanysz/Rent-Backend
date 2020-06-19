using MongoDB.Driver;
using MovieRent.Data;
using MovieRent.Models;
using MovieRent.Models.TmdbMovieResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Service
{
    public class MovieService
    {

        private IMongoCollection<TmdbMovieResult> _movies;

        public MovieService(IMovieDBSettings settings)
        {

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _movies = database.GetCollection<TmdbMovieResult>(settings.MoviesCollectionName);
        }

        public List<TmdbMovieResult> GetAll() =>
           _movies.Find(Movie => true).ToList();

        public TmdbMovieResult GetMovieByTmdbId(int tmdbId)
        {
            return _movies.Find<TmdbMovieResult>(Movie => Movie.id == tmdbId).FirstOrDefault();
        }

        public bool IsItExistsByTitle(string title)
        {
            return _movies.Find<TmdbMovieResult>(Movie => Movie.original_title == title).Any();
        }

        public bool IsItExistsByTmdbId(int tmdbId)
        {
            return _movies.Find<TmdbMovieResult>(Movie => Movie.id == tmdbId).Any();
        }

        public TmdbMovieResult Create(TmdbMovieResult movie)
        {
            _movies.InsertOne(movie);
            return movie;
        }

    }
}
