using System;
using System.Net.Http;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json.Linq;
using Repository.Models;

namespace Logic
{
    public static class ReviewMapper
    {
        private static readonly string _userapi = "http://20.45.2.119/user/";
        /// <summary>
        /// Maps an instance of Repository.Models.ReviewDto onto a new instance of
        /// GlobalModels.ReviewDto
        /// </summary>
        /// <param name="repoReview"></param>
        /// <returns></returns>
        public static async Task<ReviewDto> RepoReviewToReviewAsync(Review repoReview)
        {
            string username = await Task.Run(() => GetUsernameFromAPI(repoReview.UsernameId));
            var review = new ReviewDto(repoReview.ImdbId, username, repoReview.Score,
                repoReview.Review1,repoReview.ReviewId,repoReview.CreationTime);
            return review;
        }

        /// <summary>
        /// Maps an instance of GlobalModels.ReviewDto onto a new instance of
        /// Repository.Models.ReviewDto. Sets Repository.Models.ReviewDto.CreationTime
        /// to the current time.
        /// </summary>
        /// <param name="reviewDto"></param>
        /// <returns></returns>
        public static Review ReviewToRepoReview(ReviewDto reviewDto)
        {
            var repoReview = new Review();
            repoReview.ReviewId = reviewDto.Reviewid;
            repoReview.UsernameId = reviewDto.Usernameid;
            repoReview.ImdbId = reviewDto.Imdbid;
            repoReview.Score = reviewDto.Score;
            repoReview.Review1 = reviewDto.Review;
            repoReview.CreationTime = DateTime.Now;

            return repoReview;
        }

        /// <summary>
        /// Maps an instance of GlobalModels.ReviewDto onto a new instance of
        /// GlobalModels.ReviewNotification.
        /// </summary>
        /// <param name="reviewDto"></param>
        /// <returns></returns>
        public static ReviewNotification ReviewToReviewNotification(ReviewDto reviewDto)
        {
            var reviewNotification = new ReviewNotification();
            reviewNotification.Usernameid = reviewDto.Usernameid;
            reviewNotification.Imdbid = reviewDto.Imdbid;
            reviewNotification.Reviewid = reviewDto.Reviewid.ToString();

            return reviewNotification;
        }

        /// Gets the username of the user from the userapi using userid
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static async Task<string> GetUsernameFromAPI(string userid)
        {
            HttpClient client = new HttpClient();
            string path = _userapi + "username/" + userid;
            HttpResponseMessage response = await client.GetAsync(path);
            if(response.IsSuccessStatusCode)
            {
                string jsonContent = await response.Content.ReadAsStringAsync();
                return jsonContent;
            }
            else
            {
                return null;
            }
        }
    }
}