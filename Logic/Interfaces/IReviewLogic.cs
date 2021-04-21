using System.Collections.Generic;
using System.Threading.Tasks;
using GlobalModels;

namespace Logic.Interfaces
{
    public interface IReviewLogic
    {
        /// <summary>
        /// Adds a new Review Object to storage.
        /// Returns true if sucessful; false otherwise.
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        public Task<bool> CreateReview(Review review);

        /// <summary>
        /// Returns a list of every Review object whose Movieid is equal to
        /// the movieid argument. Returns null if the movie doesn't
        /// exist.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        public Task<List<Review>> GetReviews(string movieid);

        /// <summary>
        /// Returns Review objects [n*(page-1), n*(page-1) + n] whose MovieId
        /// is equal to the movieid argument. Where n is the current page size
        /// for reviews and sortorder determines how the Review objects are
        /// sorted prior to pagination. Returns null if the movie doesn't
        /// exist.
        /// </summary>
        /// <param name="movieid"></param>
        /// <param name="page"></param>
        /// <param name="sortorder"></param>
        /// <returns></returns>
        Task<List<Review>> GetReviewsPage(string movieid, int page, string sortorder);

        /// <summary>
        /// Sets the page size for reviews.
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Task<bool> SetReviewsPageSize(int pagesize);
    }
}