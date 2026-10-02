using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_1.Models
{
    public class Candidate
    {
        public string Name {get; set;} = string.Empty;
        public int Id {get; set;}
        public int VoteCount {get; set;} = 0;
        public string PhotoUrl {get; set;} = string.Empty;
    }
}