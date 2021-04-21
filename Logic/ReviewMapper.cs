using System;
using GlobalModels;

namespace Logic
{
    public static class ReviewMapper
    {
        /// <summary>
        /// Maps an instance of Repository.Models.Review onto a new instance of
        /// GlobalModels.Review
        /// </summary>
        /// <param name="repoReview"></param>
        /// <returns></returns>
        public static Review RepoReviewToReview(Repository.Models.Review repoReview)
        {
            var review = new Review(repoReview.MovieId, repoReview.Username, repoReview.Rating,
                repoReview.Review1);
            return review;
        }

        /// <summary>
        /// Maps an instance of GlobalModels.Review onto a new instance of
        /// Repository.Models.Review. Sets Repository.Models.Review.CreationTime
        /// to the current time.
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        public static Repository.Models.Review ReviewToRepoReview(Review review)
        {
            var repoReview = new Repository.Models.Review();
            repoReview.Username = review.Username;
            repoReview.MovieId = review.Movieid;
            repoReview.Rating = review.Rating;
            repoReview.Review1 = review.Text;
            repoReview.CreationTime = DateTime.Now;

            return repoReview;
        }
    }
}