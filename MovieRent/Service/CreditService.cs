using MongoDB.Driver;
using MovieRent.Data;
using MovieRent.Models.TmdbMovieCredits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Service
{
    public class CreditService
    {
        private IMongoCollection<MovieCredits> _credits;

        public CreditService(IMovieDBSettings settings)
        {

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _credits = database.GetCollection<MovieCredits>(settings.CreditsCollectionName);
        }

        public List<MovieCredits> GetAll() =>
            _credits.Find(Credits => true).ToList();

        public bool IsItExistsByTmdbId(int tmdbId)
        {
            return _credits.Find<MovieCredits>(Credit => Credit.id == tmdbId).Any();
        }


        public bool Create(MovieCredits credit)
        {
            _credits.InsertOne(credit);
            return true;
        }


    }
}
