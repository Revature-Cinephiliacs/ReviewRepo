using System;
using Models;
using Repository.Models;

namespace Logic
{
    public static class ReviewMapper
    {
        /// <summary>
        /// Maps an instance of Repository.Models.ReviewDto onto a new instance of
        /// GlobalModels.ReviewDto
        /// </summary>
        /// <param name="repoReview"></param>
        /// <returns></returns>
        public static Models.ReviewDto RepoReviewToReview(Repository.Models.Review repoReview)
        {
            var review = new Models.ReviewDto(repoReview.ImdbId, repoReview.UsernameId, repoReview.Score,
                repoReview.Review1);
            return review;
        }

        /// <summary>
        /// Maps an instance of GlobalModels.ReviewDto onto a new instance of
        /// Repository.Models.ReviewDto. Sets Repository.Models.ReviewDto.CreationTime
        /// to the current time.
        /// </summary>
        /// <param name="reviewDto"></param>
        /// <returns></returns>
        public static Repository.Models.Review ReviewToRepoReview(Models.ReviewDto reviewDto)
        {
            var repoReview = new Repository.Models.Review();
            repoReview.UsernameId = reviewDto.Usernameid;
            repoReview.ImdbId = reviewDto.Imdbid;
            repoReview.Score = reviewDto.Score;
            repoReview.Review1 = reviewDto.Text;
            repoReview.CreationTime = DateTime.Now;

            return repoReview;
        }
    }
}