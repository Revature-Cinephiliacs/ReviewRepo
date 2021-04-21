using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository
{
    public class ReviewRepoLogic
    {        
        private readonly ReviewDBContext _dbContext;

        public ReviewRepoLogic(ReviewDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds the Review specified in the argument to the database.
        /// If the User has already reviewed this movie, the review is replaced.
        /// Returns true iff successful.
        /// Returns false if the username or movie ID referenced in the Review object
        /// do not already exist in their respective database tables.
        /// </summary>
        /// <param name="repoReview"></param>
        /// <returns></returns>
        public async Task<bool> AddReview(Review repoReview)
        {
            // If the User has already reviewed this movie, update it
            Review review = await _dbContext.Reviews.Where(r => 
                    r.UsernameId == repoReview.UsernameId
                    && r.MovieId == repoReview.MovieId).FirstOrDefaultAsync<Review>();
            if(review == null)
            {
                await _dbContext.Reviews.AddAsync(repoReview);
            }
            else
            {
                review.Score = repoReview.Score;
                review.Review1 = repoReview.Review1;
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Returns a list of all Review objects from the database that match the movie ID specified
        /// in the argument. Returns null if the movie doesn't exist.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        public async Task<List<Review>> GetMovieReviews(string movieid)
        {
            return await _dbContext.Reviews.Where(r => r.MovieId == movieid).ToListAsync();
        }
    }
}