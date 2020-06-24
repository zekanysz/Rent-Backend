using MongoDB.Driver;
using MovieRent.Data;
using MovieRent.Models.TmdbPeople;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Service
{
    public class PersonService
    {
        private IMongoCollection<PersonDetails> _peopleDetails;

        public PersonService(IMovieDBSettings settings)
        {

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _peopleDetails = database.GetCollection<PersonDetails>(settings.PersonDetailsCollectionName);
        }

        public List<PersonDetails> GetAll() =>
          _peopleDetails.Find(PersonDetails => true).ToList();

        public bool IsItExistsByPersonId(int personId)
        {
            return _peopleDetails.Find<PersonDetails>(PersonDetails => PersonDetails.id == personId).Any();
        }

        public PersonDetails GetPersonDetailsByPersonId(int personId)
        {
            return _peopleDetails.Find<PersonDetails>(PersonDetails => PersonDetails.id == personId).FirstOrDefault();
        }

        public bool Create(PersonDetails personDetails)
        {
            _peopleDetails.InsertOne(personDetails);
            return true;
        }
    }
}
