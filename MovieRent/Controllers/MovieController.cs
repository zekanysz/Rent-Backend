using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieRent.Models;
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

        [Authorize(Roles = "Admin")]
        [HttpPost("Add/{title}")]
        public IActionResult Post(string title)
        {
            string url = String.Format("http://www.omdbapi.com/?apikey=f91c8c6f&t=" + title);

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = "GET";
            HttpWebResponse webResponse = null;
            webResponse = (HttpWebResponse)webRequest.GetResponse();
            string result = null;
            using (Stream stream = webResponse.GetResponseStream())
            {
                StreamReader streamReader = new StreamReader(stream);
                result = streamReader.ReadToEnd();
                streamReader.Close();
            }

            Movie movie = JsonConvert.DeserializeObject<Movie>(result);
            string IMDBId = movie.imdbID;

            bool isThereAny = _movieService.IsItExistsByIMDBId(IMDBId);
            if (isThereAny)
            {
                return Ok("This movie already exists in the DataBase");
            }

            //string imgUrl = String.Format("http://img.omdbapi.com/?apikey=f91c8c6f&i=" + IMDBId);

            //WebRequest imgWebRequest = WebRequest.Create(imgUrl);
            //imgWebRequest.Method = "GET";
            //HttpWebResponse imgWebResponse = null;
            //imgWebResponse = (HttpWebResponse)imgWebRequest.GetResponse();
            //var contentType = imgWebResponse.ContentType;
            //var imgType = contentType.Split("/").Last(); 

            //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "poster\\");

            //using (var client = new WebClient())
            //{
            //    client.DownloadFile(imgUrl, filePath + IMDBId + "." + imgType);
            //}

            return Ok(_movieService.Create(movie));
        }


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

        [HttpGet("GetMovieByImdbId/{imdbId}")]
        public IActionResult GetMovieById(string imdbId)
        {
            return Ok(_movieService.GetMovieByImdbId(imdbId));
        }
    }
}
