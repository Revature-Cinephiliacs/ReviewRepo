using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public sealed class ReviewDto 
    {
        [Required]
        [StringLength(20)]
        public string Imdbid { get; set; }

        [Required]
        public string Usernameid { get; set; }

        [Required]
        public Guid Reviewid { get; set; }

        [Required]
        [Range(0,5)]
        public double Score { get; set; }

        [Required]
        [StringLength(5000)]
        public string Review { get; set; }

        public DateTime CreationTime { get; set; }
        public ReviewDto()
        {
            
        }

        public ReviewDto(string movieid, string userid, double rating, string review,Guid reviewid,DateTime creationTime)
        {
            Imdbid = movieid;
            Usernameid = userid;
            Score = rating;
            Review = review;
            Reviewid = reviewid;
            CreationTime = creationTime;
        }

    }
}