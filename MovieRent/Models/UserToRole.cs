using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MovieRent.Models
{
    [DataContract]
    public class UserToRole
    {
        [DataMember]
        public string User { get; set; }
        [DataMember]

        public string Role { get; set; }
    }
}
