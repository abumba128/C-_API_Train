using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_1.Models
{
   public class Vote
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }

        public string IpAddress { get; set; } = string.Empty;

        public DateTime VotedAt { get; set; }
    }
}