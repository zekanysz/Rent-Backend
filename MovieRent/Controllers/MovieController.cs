using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieRent.Models;
using MovieRent.Models.ImdbJSON;
using MovieRent.Models.TmdbMovieCredits;
using MovieRent.Models.TmdbMovieResult;
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


        public MovieController(MovieService movieService, CreditService creditService)
        {
            _movieService = movieService;
            _creditService = creditService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add/{imdbId}")]
        public IActionResult MovieResult(string imdbId)
        {
            string getImdbIdUrl = String.Format("https://api.themoviedb.org/3/find/" + imdbId +"?api_key=8d8ebd7ef7cb1361e624639ac8fea328&external_source=imdb_id");

            WebRequest webRequestImdb = WebRequest.Create(getImdbIdUrl);
            webRequestImdb.Method = "GET";
            HttpWebResponse webResponseImdb = null;
            webResponseImdb = (HttpWebResponse)webRequestImdb.GetResponse();
            string resultImdb = null;
            using (Stream streamImdb = webResponseImdb.GetResponseStream())
            {
                StreamReader streamReaderImdb = new StreamReader(streamImdb);
                resultImdb = streamReaderImdb.ReadToEnd();
                streamReaderImdb.Close();
            }

            ImdbMovieResult imdbMovieResult = JsonConvert.DeserializeObject<ImdbMovieResult>(resultImdb);


            string getTmdbIdUrl = String.Format("https://api.themoviedb.org/3/movie/" + imdbMovieResult.movie_results.FirstOrDefault().id + "?api_key=8d8ebd7ef7cb1361e624639ac8fea328&language=en-US");

            WebRequest webRequestTmdb = WebRequest.Create(getTmdbIdUrl);
            webRequestTmdb.Method = "GET";
            HttpWebResponse webResponseTmdb = null;
            webResponseTmdb = (HttpWebResponse)webRequestTmdb.GetResponse();
            string resultTmdb = null;
            using (Stream streamTmdb = webResponseTmdb.GetResponseStream())
            {
                StreamReader streamReaderTmdb = new StreamReader(streamTmdb);
                resultTmdb = streamReaderTmdb.ReadToEnd();
                streamReaderTmdb.Close();
            }

            TmdbMovieResult tmdbMovieResult = JsonConvert.DeserializeObject<TmdbMovieResult>(resultTmdb);

            bool isThereAnyMovie = _movieService.IsItExistsByTmdbId(tmdbMovieResult.id);
            if (isThereAnyMovie)
            {
                return Ok("This movie already exists in the DataBase");
            }
            bool isCreditsCreated = _movieService.Create(tmdbMovieResult);


            string getCreditsIdUrl = String.Format("https://api.themoviedb.org/3/movie/" + tmdbMovieResult.id + "/credits?api_key=8d8ebd7ef7cb1361e624639ac8fea328");

            WebRequest webRequestCredits = WebRequest.Create(getCreditsIdUrl);
            webRequestCredits.Method = "GET";
            HttpWebResponse webResponseCredits = null;
            webResponseCredits = (HttpWebResponse)webRequestCredits.GetResponse();
            string resultCredits = null;
            using (Stream streamCredits = webResponseCredits.GetResponseStream())
            {
                StreamReader streamReaderCredits = new StreamReader(streamCredits);
                resultCredits = streamReaderCredits.ReadToEnd();
                streamReaderCredits.Close();
            }

            MovieCredits movieCredits = JsonConvert.DeserializeObject<MovieCredits>(resultCredits);

            bool isThereAnyCredits = _creditService.IsItExistsByTmdbId(movieCredits.id);
            if (isThereAnyCredits)
            {
                return Ok("These credits already exists in the DataBase");
            }

            bool isMovieCreated = _creditService.Create(movieCredits);


            if (!isMovieCreated && !isCreditsCreated)
            {
                return Ok("Error");
            }

            return Ok("Movie and Credites were inserted to the Database!");
        }

        [HttpGet("AllMovies")]
        public IActionResult GetMovies()
        {
            return Ok(_movieService.GetAll());
        }

        //[HttpPost("AddCredit/{tmdbId}")]
        //public IActionResult AddCredit(string tmdbId)
        //{
           
        //}

    }
}
