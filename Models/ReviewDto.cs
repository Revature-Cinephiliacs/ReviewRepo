using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public sealed class ReviewDto : IEquatable<ReviewDto>
    {
        [Required]
        [StringLength(20)]
        public string Imdbid { get; set; }

        [Required]
        public Guid Usernameid { get; set; }

        [Required]
        public Guid Reviewid { get; set; }

        [Required]
        [Range(0,5)]
        public double Score { get; set; }

        [Required]
        [StringLength(300)]
        public string Review { get; set; }

        public DateTime CreationTime { get; set; }
        public ReviewDto()
        {
            
        }

        public ReviewDto(string movieid, Guid userid, double rating, string review,Guid reviewid,DateTime creationTime)
        {
            Imdbid = movieid;
            Usernameid = userid;
            Score = rating;
            Review = review;
            Reviewid = reviewid;
            CreationTime = creationTime;
        }

        public bool Equals(ReviewDto other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return Imdbid == other.Imdbid;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ReviewDto);
        }

        public static bool operator ==(ReviewDto lhs, ReviewDto rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    return true;
                }

                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ReviewDto lhs, ReviewDto rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return Imdbid.GetHashCode();
        }
    }
}