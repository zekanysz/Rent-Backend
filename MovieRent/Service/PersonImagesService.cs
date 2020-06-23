using MongoDB.Driver;
using MovieRent.Data;
using MovieRent.Models.TmdbPeople;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Service
{
    public class PersonImagesService
    {
        private IMongoCollection<PersonImages> _peopleImages;

        public PersonImagesService(IMovieDBSettings settings)
        {

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _peopleImages = database.GetCollection<PersonImages>(settings.PersonImagesCollectionName);
        }

        public List<PersonImages> GetAll() =>
           _peopleImages.Find(PersonImages => true).ToList();

        public bool IsItExistsByPersonId(int personId)
        {
            return _peopleImages.Find<PersonImages>(PersonImages => PersonImages.id == personId).Any();
        }

        public PersonImages GetPersonImagesByPersonId(int personId)
        {
            return _peopleImages.Find<PersonImages>(PersonImages => PersonImages.id == personId).FirstOrDefault();
        }


        public bool Create(PersonImages personImages)
        {
            _peopleImages.InsertOne(personImages);
            return true;
        }
    }
}
