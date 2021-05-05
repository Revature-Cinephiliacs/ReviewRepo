using System;
using System.ComponentModel.DataAnnotations;
namespace Models
{
    public class ReviewNotification
    {
        /// <summary>
        /// Facillitates passing of notifications.
        /// </summary>
        /// <value></value>
        public string Imdbid { get; set; }
        public string Usernameid { get; set; }
        public string Reviewid { get; set; }
    }
}