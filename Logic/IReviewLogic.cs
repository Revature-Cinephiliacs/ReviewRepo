using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Repository.Models;

namespace Logic.Interfaces
{
    public interface IReviewLogic
    {
        /// <summary>
        /// Adds a new ReviewDto Object to storage.
        /// Returns true if sucessful; false otherwise.
        /// </summary>
        /// <param name="reviewDto"></param>
        /// <returns></returns>
         Task<bool> CreateReview(ReviewDto reviewDto);

        /// <summary>
        /// Returns a list of every ReviewDto object whose Movieid is equal to
        /// the movieid argument. Returns null if the movie doesn't
        /// exist.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
         Task<List<ReviewDto>> GetReviews(string movieid);

        /// <summary>
        /// Returns a list of every ReviewDto object whose UsernameId is equal to
        /// the reviewId argument. Returns null if the movie doesn't
        /// exist.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ReviewDto>> GetReviewsByUser(Guid userId);

        /// <summary>
        /// Returns ReviewDto objects [n*(page-1), n*(page-1) + n] whose MovieId
        /// is equal to the movieid argument. Where n is the current page size
        /// for reviews and sortorder determines how the ReviewDto objects are
        /// sorted prior to pagination. Returns null if the movie doesn't
        /// exist.
        /// </summary>
        /// <param name="movieid"></param>
        /// <param name="page"></param>
        /// <param name="sortorder"></param>
        /// <returns></returns>
        Task<List<ReviewDto>> GetReviewsPage(string movieid, int page, string sortorder);

        /// <summary>
        /// Sets the page size for reviews.
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Task<bool> SetReviewsPageSize(int pagesize);

        /// <summary>
        /// update ReviewDto object whose reviewId exist
        ///  Returns null if the review doesn't
        /// exist.
        /// </summary>
        /// <param name="reviewDto"></param>
        /// <returns></returns>
        Review UpdatedRev(Review reviewDto);

        Task<Review> getOneReview(Guid reviewId);

        /// <summary>
        /// delete a review from the database
        /// </summary>
        /// <param name="reviewDto"></param>
        void deleteReview(Review reviewDto);

        Task<List<ReviewDto>> GetReviewsByRating(int rating);
        Task<List<ReviewDto>> GetReviewsByRating(string imdb,int rating);

    }
}