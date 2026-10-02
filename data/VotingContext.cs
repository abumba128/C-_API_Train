using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project_1.Models;

namespace Project_1.data
{
    public class VotingContext : DbContext
    {
        public DbSet<Candidate> Candidates {get; set;}
        public DbSet<Vote> Votes {get; set;}

        public VotingContext(DbContextOptions<VotingContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Candidate>().HasData(
                new Candidate { Id = 1, Name = "Кандидат 1", PhotoUrl = "https://via.placeholder.com/250x200?text=Кандидат+1", VoteCount = 0 },
                new Candidate { Id = 2, Name = "Кандидат 2", PhotoUrl = "https://via.placeholder.com/250x200?text=Кандидат+2", VoteCount = 0 },
                new Candidate { Id = 3, Name = "Кандидат 3", PhotoUrl = "https://via.placeholder.com/250x200?text=Кандидат+3", VoteCount = 0 },
                new Candidate { Id = 4, Name = "Кандидат 4", PhotoUrl = "https://via.placeholder.com/250x200?text=Кандидат+4", VoteCount = 0 },
                new Candidate { Id = 5, Name = "Кандидат 5", PhotoUrl = "https://via.placeholder.com/250x200?text=Кандидат+5", VoteCount = 0 }
                 
            );
        }
    }
}