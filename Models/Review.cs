using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalModels
{
    public sealed class Review : IEquatable<Review>
    {
        [Required]
        [StringLength(20)]
        public string Movieid { get; set; }

        [Required]
        [StringLength(30)]
        public string Username { get; set; }

        [Required]
        [Range(0,5)]
        public decimal Rating { get; set; }

        [Required]
        [StringLength(300)]
        public string Text { get; set; }
        public Review()
        {
            
        }

        public Review(string movieid, string username, decimal rating, string text)
        {
            Movieid = movieid;
            Username = username;
            Rating = rating;
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

            return Movieid == other.Movieid;
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
            return Movieid.GetHashCode();
        }
    }
}