using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieRent.Models;
using MovieRent.Models.ImdbJSON;
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

        public MovieController(MovieService movieService)
        {
            _movieService = movieService;
        }


        //[HttpGet("Search/{imdbId}")]
        //public IActionResult ImdbMovieResult(string imdbId)
        //{

        //    string getImdbIdUrl = String.Format("https://api.themoviedb.org/3/find/" + imdbId + "?api_key=8d8ebd7ef7cb1361e624639ac8fea328&external_source=imdb_id");

        //    WebRequest webRequestImdb = WebRequest.Create(getImdbIdUrl);
        //    webRequestImdb.Method = "GET";
        //    HttpWebResponse webResponseImdb = null;
        //    webResponseImdb = (HttpWebResponse)webRequestImdb.GetResponse();
        //    string resultImdb = null;
        //    using (Stream streamImdb = webResponseImdb.GetResponseStream())
        //    {
        //        StreamReader streamReaderImdb = new StreamReader(streamImdb);
        //        resultImdb = streamReaderImdb.ReadToEnd();
        //        streamReaderImdb.Close();
        //    }

        //    ImdbMovieResult imdbMovieResult = JsonConvert.DeserializeObject<ImdbMovieResult>(resultImdb);

        //    return Ok(imdbMovieResult);
        //}

        //[Authorize(Roles = "Admin")]
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

            bool isThereAny = _movieService.IsItExistsByTmdbId(tmdbMovieResult.id);
            if (isThereAny)
            {
                return Ok("This movie already exists in the DataBase");
            }

            return Ok(_movieService.Create(tmdbMovieResult));
        }





        //[Authorize(Roles = "Admin")]
        //[HttpPost("Add/{title}")]
        //public IActionResult Post(string title)
        //{
        //    string url = String.Format("http://www.omdbapi.com/?apikey=f91c8c6f&t=" + title);

        //    WebRequest webRequest = WebRequest.Create(url);
        //    webRequest.Method = "GET";
        //    HttpWebResponse webResponse = null;
        //    webResponse = (HttpWebResponse)webRequest.GetResponse();
        //    string result = null;
        //    using (Stream stream = webResponse.GetResponseStream())
        //    {
        //        StreamReader streamReader = new StreamReader(stream);
        //        result = streamReader.ReadToEnd();
        //        streamReader.Close();
        //    }

        //    Movie movie = JsonConvert.DeserializeObject<Movie>(result);
        //    string IMDBId = movie.imdbID;

        //    bool isThereAny = _movieService.IsItExistsByIMDBId(IMDBId);
        //    if (isThereAny)
        //    {
        //        return Ok("This movie already exists in the DataBase");
        //    }

        //    //string imgUrl = String.Format("http://img.omdbapi.com/?apikey=f91c8c6f&i=" + IMDBId);

        //    //WebRequest imgWebRequest = WebRequest.Create(imgUrl);
        //    //imgWebRequest.Method = "GET";
        //    //HttpWebResponse imgWebResponse = null;
        //    //imgWebResponse = (HttpWebResponse)imgWebRequest.GetResponse();
        //    //var contentType = imgWebResponse.ContentType;
        //    //var imgType = contentType.Split("/").Last(); 

        //    //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "poster\\");

        //    //using (var client = new WebClient())
        //    //{
        //    //    client.DownloadFile(imgUrl, filePath + IMDBId + "." + imgType);
        //    //}

        //    return Ok(_movieService.Create(movie));
        //}


        [HttpGet("AllMovies")]
        public IActionResult GetMovies()
        {
            return Ok(_movieService.GetAll());
        }

        //[HttpGet("GetPoster")]
        //public IActionResult GetPoster(string imdbId)
        //{
        //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "poster\\", imdbId + ".jpeg");
        //    var posterFileStrem = System.IO.File.OpenRead(filePath);
        //    return File(posterFileStrem, "image/jpeg");
        //}

        //[HttpGet("GetMovieByImdbId/{imdbId}")]
        //public IActionResult GetMovieById(string imdbId)
        //{
        //    return Ok(_movieService.GetMovieByImdbId(imdbId));
        //}
    }
}
