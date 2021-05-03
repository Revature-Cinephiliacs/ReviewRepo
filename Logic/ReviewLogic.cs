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
        /// Returns a list of every ReviewDto object whose Movieid is equal to
        /// the movieid argument. Returns null if the movie doesn't
        /// exist.
        /// </summary>
        public async Task<List<ReviewDto>> GetReviews(string movieid)
        {
            var repoReviews = await _repo.GetMovieReviews(movieid);
            if(repoReviews == null || repoReviews.Count == 0)
            {
                Console.WriteLine("MovieLogic.GetReviews() was called for a movie that doesn't exist.");
                return null;
            }

            var reviews = new List<ReviewDto>();
            foreach (var repoReview in repoReviews)
            {
                reviews.Add(await ReviewMapper.RepoReviewToReviewAsync(repoReview));
            }
            return reviews;
        }

        public async Task<List<ReviewDto>> GetReviewsByUser(string userId)
        {
            List<ReviewDto> revDto = new List<ReviewDto>();

            List<Review> reviews = await _repo.getListofReviewsByUser(userId);
            if (reviews == null || reviews.Count == 0 )
            {
                return null;
            }

            foreach (var rev in reviews)
            {
                revDto.Add(await ReviewMapper.RepoReviewToReviewAsync(rev));
            }

            return revDto;
        }

        /// <summary>
        /// Returns ReviewDto objects [n*(page-1), n*(page-1) + n] whose MovieId
        /// is equal to the movieid argument. Where n is the current page size
        /// for reviews and sortorder determines how the ReviewDto objects are
        /// sorted prior to pagination. Returns null if the movie doesn't
        /// exist.
        /// </summary>
        public async Task<List<ReviewDto>> GetReviewsPage(string movieid, int page, string sortorder)
        {
            if(page < 1)
            {
                Console.WriteLine("ReviewLogic.GetReviewsPage() was called with a negative or zero page number.");
                return null;
            }

            Setting pageSizeSetting = _repo.GetSetting("reviewspagesize");
            int pageSize = (int)pageSizeSetting.IntValue;
            if(pageSize < 1)
            {
                Console.WriteLine("ReviewLogic.GetReviewsPage() was called but the reviewspagesize is invalid");
                return null;
            }

            List<Review> repoReviews = await _repo.GetMovieReviews(movieid);

            if(repoReviews == null || repoReviews.Count ==0)
            {
                Console.WriteLine("ReviewLogic.GetReviewsPage() was called for a movie that doesn't exist.");
                return null;
            }

            switch (sortorder)
            {
                case "ratingasc":
                    repoReviews = repoReviews.OrderBy(r => r.Score).ToList();
                break;
                case "ratingdsc":
                    repoReviews = repoReviews.OrderByDescending(r => r.Score).ToList();
                break;
                case "timeasc":
                    repoReviews = repoReviews.OrderBy(r => r.CreationTime).ToList();
                break;
                case "timedsc":
                    repoReviews = repoReviews.OrderByDescending(r => r.CreationTime).ToList();
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

            List<ReviewDto> reviews = new List<ReviewDto>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                reviews.Add(await ReviewMapper.RepoReviewToReviewAsync(repoReviews[i]));
            }
            return reviews;
        }

        public async Task<bool> SetReviewsPageSize(int pagesize)
        {
            if(pagesize < 1 || pagesize > 100)
            {
                return false;
            }

            Setting setting = new Setting {Setting1 = "reviewspagesize", IntValue = pagesize};
            return await _repo.SetSetting(setting);
        }


        public Review UpdatedRev(Review reviewDto)
        {
            return _repo.updateReview(reviewDto);
        }

        public async Task<Review> getOneReview(Guid reviewId)
        {
            return await _repo.getSingleReview(reviewId);
        }

        public void deleteReview(Review reviewDto)
        {
            _repo.deleteReview(reviewDto);
        }

        public async Task<List<ReviewDto>> GetReviewsByRating(int rating)
        {
            List<ReviewDto> revDto = new List<ReviewDto>();

            List<Review> reviews = await _repo.getAllReviewByRating(rating);
            if (reviews == null || reviews.Count == 0 )
            {
                return null;
            }

            foreach (var rev in reviews)
            {
                revDto.Add(await ReviewMapper.RepoReviewToReviewAsync(rev));
            }

            return revDto;
        }

        public async Task<List<ReviewDto>> GetReviewsByRating(string imdb,int rating)
        {
            List<ReviewDto> revDto = new List<ReviewDto>();

            List<Review> reviews = await _repo.getAllReviewByRating(imdb,rating);
            if (reviews == null || reviews.Count == 0 )
            {
                return null;
            }

            System.Console.WriteLine(reviews.Count);
            foreach (var rev in reviews)
            {
                revDto.Add(await ReviewMapper.RepoReviewToReviewAsync(rev));
                System.Console.WriteLine(rev.Score);
            }

            return revDto;
        }
        public async Task<bool> CreateReview(ReviewDto reviewDto)
        {
            var repoReview = ReviewMapper.ReviewToRepoReview(reviewDto);
            return await _repo.AddReview(repoReview);
        }
        public async Task<List<ReviewDto>> GetReviewsByIDS(List<string> ids)
        {
            List<ReviewDto> revDto = new List<ReviewDto>();
            
            foreach (var rev in await _repo.getAllReviewsBYIDS(ids))
            {
                revDto.Add(await ReviewMapper.RepoReviewToReviewAsync(rev));
            }
            return revDto;
        }

        /// <summary>
        /// Gets a ReviewDto from ReviewController and passes it into the ReviewMapper
        /// to get back a ReviewNotification that matches the ReviewDto.
        /// </summary>
        /// <param name="reviewDto"></param>
        /// <returns></returns>
        public ReviewNotification GetReviewNotification(ReviewDto reviewDto)
        {
            var reviewNotification = ReviewMapper.ReviewToReviewNotification(reviewDto);
            return reviewNotification;
        }
    }
}