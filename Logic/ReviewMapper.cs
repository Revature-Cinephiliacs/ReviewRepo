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
        public static ReviewDto RepoReviewToReview(Review repoReview)
        {
            var review = new ReviewDto(repoReview.ImdbId, repoReview.UsernameId, repoReview.Score,
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
            repoReview.UsernameId = reviewDto.Usernameid;
            repoReview.ImdbId = reviewDto.Imdbid;
            repoReview.Score = reviewDto.Score;
            repoReview.Review1 = reviewDto.Review;
            repoReview.CreationTime = DateTime.Now;

            return repoReview;
        }
    }
}