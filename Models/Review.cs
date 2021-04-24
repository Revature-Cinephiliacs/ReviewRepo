using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public sealed class Review : IEquatable<Review>
    {
        [Required]
        [StringLength(20)]
        public string Imdbid { get; set; }

        [Required]
        public Guid Usernameid { get; set; }

        [Required]
        [Range(0,5)]
        public double Score { get; set; }

        [Required]
        [StringLength(300)]
        public string Text { get; set; }
        public Review()
        {
            
        }

        public Review(string movieid, Guid userid, double rating, string text)
        {
            Imdbid = movieid;
            Usernameid = userid;
            Score = rating;
            Text = text;
        }

        public bool Equals(Review other)
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
            return this.Equals(obj as Review);
        }

        public static bool operator ==(Review lhs, Review rhs)
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

        public static bool operator !=(Review lhs, Review rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return Imdbid.GetHashCode();
        }
    }
}