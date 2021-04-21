using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlobalModels;
using Repository;

namespace Logic
{
    public class ReveiwLogic : Interfaces.IReveiwLogic
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
        public async Task<List<Review>> GetReviews(string movieid)
        {
            var repoReviews = await _repo.GetMovieReviews(movieid);
            if(repoReviews == null)
            {
                Console.WriteLine("MovieLogic.GetReviews() was called for a movie that doesn't exist.");
                return null;
            }

            var reviews = new List<Review>();
            foreach (var repoReview in repoReviews)
            {
                reviews.Add(Mapper.RepoReviewToReview(repoReview));
            }
            return reviews;
        }

        /// <summary>
        /// Returns Review objects [n*(page-1), n*(page-1) + n] whose MovieId
        /// is equal to the movieid argument. Where n is the current page size
        /// for reviews and sortorder determines how the Review objects are
        /// sorted prior to pagination. Returns null if the movie doesn't
        /// exist.
        /// </summary>
        public async Task<List<Review>> GetReviewsPage(string movieid, int page, string sortorder)
        {
            if(page < 1)
            {
                Console.WriteLine("ReviewLogic.GetReviewsPage() was called with a negative or zero page number.");
                return null;
            }

            Repository.Models.Setting pageSizeSetting = _repo.GetSetting("reviewspagesize");
            int pageSize = pageSizeSetting.IntValue ?? default(int);
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
                    repoReviews = repoReviews.OrderBy(r => r.Rating).ToList<Repository.Models.Review>();
                break;
                case "ratingdsc":
                    repoReviews = repoReviews.OrderByDescending(r => r.Rating).ToList<Repository.Models.Review>();
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

            List<Review> reviews = new List<Review>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                reviews.Add(Mapper.RepoReviewToReview(repoReviews[i]));
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
        public async Task<bool> CreateReview(Review review)
        {
            var repoReview = Mapper.ReviewToRepoReview(review);
            return await _repo.AddReview(repoReview);
        }
    }
}