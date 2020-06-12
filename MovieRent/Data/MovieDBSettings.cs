﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Data
{
    public class MovieDBSettings : IMovieDBSettings
    {
        public string UsersCollectionName { get; set; }
        public string MoviesCollectionName { get; set; }
        public string RentsCollectionName { get; set; }
    }

    public interface IMovieDBSettings
    {
        string UsersCollectionName { get; set; }
        string MoviesCollectionName { get; set; }
        string RentsCollectionName { get; set; }

    }
}
