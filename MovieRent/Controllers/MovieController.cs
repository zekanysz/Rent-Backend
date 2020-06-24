using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieRent.Models;
using MovieRent.Models.ImdbJSON;
using MovieRent.Models.TmdbMovieCredits;
using MovieRent.Models.TmdbMovieResult;
using MovieRent.Models.TmdbPeople;
using MovieRent.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MovieRent.Controllers
{
    public class MovieController : ApiController
    {
        private MovieService _movieService;
        private CreditService _creditService;
        private PersonService _personService;
        private PersonImagesService _personImagesService;


        public MovieController(MovieService movieService, CreditService creditService, PersonService personService, PersonImagesService personImagesService)
        {
            _movieService = movieService;
            _creditService = creditService;
            _personImagesService = personImagesService;
            _personService = personService;
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("Add/{imdbId}")]
        public IActionResult MovieResult(string imdbId)
        {
            WebRequest request;
            HttpWebResponse response;
            string result = null;

            string api = "api_key=8d8ebd7ef7cb1361e624639ac8fea328";
            string finalResponse = "";
            string getImdbIdUrl = String.Format("https://api.themoviedb.org/3/find/" + imdbId + "?" + api + "&external_source=imdb_id");

            request = WebRequest.Create(getImdbIdUrl);
            request.Method = "GET";
            response = (HttpWebResponse)request.GetResponse();
            using (Stream streamImdb = response.GetResponseStream())
            {
                StreamReader streamReaderImdb = new StreamReader(streamImdb);
                result = streamReaderImdb.ReadToEnd();
                streamReaderImdb.Close();
            }

            ImdbMovieResult imdbMovieResult = JsonConvert.DeserializeObject<ImdbMovieResult>(result);
            int tmdbId = imdbMovieResult.movie_results.FirstOrDefault().id;

            bool isThereAnyMovie = _movieService.IsItExistsByTmdbId(tmdbId);

            if (isThereAnyMovie)
            {
                finalResponse += "This movie already exists in the Database!";
            }
            else
            {
                string getTmdbIdUrl = String.Format("https://api.themoviedb.org/3/movie/" + tmdbId + "?" + api + "&language=en-US");

                request = WebRequest.Create(getTmdbIdUrl);
                response = (HttpWebResponse)request.GetResponse();
                using (Stream streamTmdb = response.GetResponseStream())
                {
                    StreamReader streamReaderTmdb = new StreamReader(streamTmdb);
                    result = streamReaderTmdb.ReadToEnd();
                    streamReaderTmdb.Close();
                }

                TmdbMovieResult tmdbMovieResult = JsonConvert.DeserializeObject<TmdbMovieResult>(result);

                bool isMovieCreated = _movieService.Create(tmdbMovieResult);

                if (isMovieCreated)
                    finalResponse += "Movie were inserted into the Database!";
                else
                    finalResponse += "Some error!";
            }

            bool isThereAnyCredits = _creditService.IsItExistsByTmdbId(tmdbId);

            if (isThereAnyCredits)
            {
                finalResponse += "\nThese credits already exists in the Database!";
            }
            else
            {
                string getCreditsIdUrl = String.Format("https://api.themoviedb.org/3/movie/" + tmdbId + "/credits?" + api);

                request = WebRequest.Create(getCreditsIdUrl);
                response = (HttpWebResponse)request.GetResponse();
                using (Stream streamCredits = response.GetResponseStream())
                {
                    StreamReader streamReaderCredits = new StreamReader(streamCredits);
                    result = streamReaderCredits.ReadToEnd();
                    streamReaderCredits.Close();
                }

                MovieCredits movieCredits = JsonConvert.DeserializeObject<MovieCredits>(result);

                bool isCreditsCreated = _creditService.Create(movieCredits);

                if (isCreditsCreated)
                    finalResponse += "Credites were inserted into the Database!";
                else
                    finalResponse += "Some error!";

                //int counter = 0;

                //foreach (var cast in movieCredits.cast)
                //{
                //    bool isThereAnyPersonImegas = _personImagesService.IsItExistsByPersonId(cast.id);
                //    if (isThereAnyPersonImegas)
                //        continue;
                //    else
                //        counter++;

                //    string getPersonImageIdUrl = String.Format("https://api.themoviedb.org/3/person/" + cast.id + "/images?" + api);

                //    request = WebRequest.Create(getPersonImageIdUrl);
                //    response = (HttpWebResponse)request.GetResponse();
                //    using (Stream streamPersonImages = response.GetResponseStream())
                //    {
                //        StreamReader streamReaderPersonImages = new StreamReader(streamPersonImages);
                //        result = streamReaderPersonImages.ReadToEnd();
                //        streamReaderPersonImages.Close();
                //    }

                //    PersonImages personImages = JsonConvert.DeserializeObject<PersonImages>(result);
                //    var orderedPersonImages = personImages.profiles.OrderByDescending(x => x.vote_average).ToList();
                //    personImages.profiles = orderedPersonImages;
                //    personImages.id = cast.id;

                //    bool isPersonImagesCreated = _personImagesService.Create(personImages);
                //}

                int personDetailsCounter = 0;

                foreach (var cast in movieCredits.cast)
                {
                    bool isThereAnyPersonDetails = _personService.IsItExistsByPersonId(cast.id);
                    if (isThereAnyPersonDetails)
                        continue;
                    else
                        personDetailsCounter++;

                    string getPersonDetailsIdUrl = String.Format("https://api.themoviedb.org/3/person/" + cast.id + "?" + api + "&language=en-US");

                    request = WebRequest.Create(getPersonDetailsIdUrl);
                    response = (HttpWebResponse)request.GetResponse();
                    using (Stream streamPersonDetails = response.GetResponseStream())
                    {
                        StreamReader streamReaderPersonDetails = new StreamReader(streamPersonDetails);
                        result = streamReaderPersonDetails.ReadToEnd();
                        streamReaderPersonDetails.Close();
                    }

                    PersonDetails personDetails = JsonConvert.DeserializeObject<PersonDetails>(result);

                    bool isPersonImagesCreated = _personService.Create(personDetails);
                }

                finalResponse += "\n" + personDetailsCounter + " actor profiles were inserted into the Database!";
            }


            return Ok(JsonConvert.SerializeObject(finalResponse));
        }

        [HttpGet("AllMovies")]
        public IActionResult GetMovies()
        {
            return Ok(_movieService.GetAll());
        }


        [HttpGet("AllCredits")]
        public IActionResult GetCredits()
        {
            return Ok(_creditService.GetAll());
        }

        [HttpGet("AllPersonImagesByMovieId/{movieId}")]
        public IActionResult GetPersonImages(int movieId)
        {

            MovieCredits movieCredits = _creditService.GetCreditsByMovieId(movieId);

            List<PersonImages> personImagesLsit = new List<PersonImages>();

            foreach (var cast in movieCredits.cast)
            {
                PersonImages personImages = _personImagesService.GetPersonImagesByPersonId(cast.id);
                personImagesLsit.Add(personImages);
            }

            return Ok(personImagesLsit);
        }

        [HttpGet("AllPersonDetailsByMovieId/{movieId}")]
        public IActionResult GetPersonDetails(int movieId)
        {
            MovieCredits movieCredits = _creditService.GetCreditsByMovieId(movieId);

            List<PersonDetails> personDetailsLsit = new List<PersonDetails>();

            foreach (var cast in movieCredits.cast)
            {
                PersonDetails personDetails = _personService.GetPersonDetailsByPersonId(cast.id);
                personDetailsLsit.Add(personDetails);
            }

            return Ok(personDetailsLsit);
        }


        [HttpGet("getPersonDetailsById/{personId}")]
        public IActionResult GetPersonDetailsById(int personId)
        {
            return Ok(_personService.GetPersonDetailsByPersonId(personId));
        }
    }
}
