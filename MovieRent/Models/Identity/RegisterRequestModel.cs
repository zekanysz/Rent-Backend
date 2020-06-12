using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRent.Models.Identity
{
    public class RegisterRequestModel
    {
        //[Required]
        //public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string PasswordConfirm { get; set; }

    }
}
