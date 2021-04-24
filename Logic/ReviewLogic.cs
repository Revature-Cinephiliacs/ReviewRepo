using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Repository;
using Repository.Models;

namespace Logic
{
    public class ReviewLogic : Interfaces.IReviewLogic
    {
        private readonly ReviewRepoLogic _repo;
        
        public ReviewLogic(ReviewRepoLogic repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Returns a list of every Review object whose Movieid is equal to
        /// the movieid argument. Returns null if the movie doesn't
        /// exist.
        /// </summary>
        public async Task<List<Models.Review>> GetReviews(string movieid)
        {
            var repoReviews = await _repo.GetMovieReviews(movieid);
            if(repoReviews == null)
            {
                Console.WriteLine("MovieLogic.GetReviews() was called for a movie that doesn't exist.");
                return null;
            }

            var reviews = new List<Models.Review>();
            foreach (var repoReview in repoReviews)
            {
                reviews.Add(ReviewMapper.RepoReviewToReview(repoReview));
            }
            return reviews;
        }

        /// <summary>
        /// Returns a list of all Review objects from the database that match the movie ID specified
        /// in the argument. Returns null if the movie doesn't exist.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        public async Task<List<Repository.Models.Review>> GetMovieReviews(string movieid)
        {
            return await _repo.GetMovieReviews(movieid);
        }

        /// <summary>
        /// Returns Review objects [n*(page-1), n*(page-1) + n] whose MovieId
        /// is equal to the movieid argument. Where n is the current page size
        /// for reviews and sortorder determines how the Review objects are
        /// sorted prior to pagination. Returns null if the movie doesn't
        /// exist.
        /// </summary>
        public async Task<List<Models.Review>> GetReviewsPage(string movieid, int page, string sortorder)
        {
            if(page < 1)
            {
                Console.WriteLine("ReviewLogic.GetReviewsPage() was called with a negative or zero page number.");
                return null;
            }

            Repository.Models.Setting pageSizeSetting = _repo.GetSetting("reviewspagesize");
            int pageSize = (int)pageSizeSetting.IntValue;
            if(pageSize < 1)
            {
                Console.WriteLine("ReviewLogic.GetReviewsPage() was called but the reviewspagesize is invalid");
                return null;
            }

            List<Repository.Models.Review> repoReviews = await _repo.GetMovieReviews(movieid);

            if(repoReviews == null)
            {
                Console.WriteLine("ReviewLogic.GetReviewsPage() was called for a movie that doesn't exist.");
                return null;
            }

            // Sort the list of Reviews
            switch (sortorder)
            {
                case "ratingasc":
                    repoReviews = repoReviews.OrderBy(r => r.Score).ToList<Repository.Models.Review>();
                break;
                case "ratingdsc":
                    repoReviews = repoReviews.OrderByDescending(r => r.Score).ToList<Repository.Models.Review>();
                break;
                case "timeasc":
                    repoReviews = repoReviews.OrderBy(r => r.CreationTime).ToList<Repository.Models.Review>();
                break;
                case "timedsc":
                    repoReviews = repoReviews.OrderByDescending(r => r.CreationTime).ToList<Repository.Models.Review>();
                break;
                default:
                    return null;
            }

            int numberOfReviews = repoReviews.Count;
            int startIndex = pageSize * (page - 1);

            if(startIndex > numberOfReviews - 1)
            {
                Console.WriteLine("MovieLogic.GetReviewsPage() was called for a page number without reviews.");
                return null;
            }

            int endIndex = startIndex + pageSize - 1;
            if(endIndex > numberOfReviews - 1)
            {
                endIndex = numberOfReviews - 1;
            }

            List<Models.Review> reviews = new List<Models.Review>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                reviews.Add(ReviewMapper.RepoReviewToReview(repoReviews[i]));
            }
            return reviews;
        }

        public async Task<bool> SetReviewsPageSize(int pagesize)
        {
            if(pagesize < 1 || pagesize > 100)
            {
                return false;
            }

            Repository.Models.Setting setting = new Repository.Models.Setting();
            setting.Setting1 = "reviewspagesize";
            setting.IntValue = pagesize;
            return await _repo.SetSetting(setting);
        }

        /// <summary>
        /// Sets the page size for reviews.
        /// </summary>
        public async Task<bool> CreateReview(Models.Review review)
        {
            var repoReview = ReviewMapper.ReviewToRepoReview(review);
            return await _repo.AddReview(repoReview);
        }
    }
}