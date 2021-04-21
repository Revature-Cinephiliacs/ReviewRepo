using System;

namespace Repository
{
    public class ReviewRepoLogic
    {
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
            var userExists = UserExists(repoReview.Username);
            if(!userExists)
            {
                Console.WriteLine("RepoLogic.AddReview() was called for a user that doesn't exist.");
                return false;
            }
            var movieExists = MovieExists(repoReview.MovieId);
            if(!movieExists)
            {
                Console.WriteLine("RepoLogic.AddReview() was called for a movie that doesn't exist.");
                return false;
            }
            // If the User has already reviewed this movie, update it
            Review review = await _dbContext.Reviews.Where(r => 
                    r.Username == repoReview.Username 
                    && r.MovieId == repoReview.MovieId).FirstOrDefaultAsync<Review>();
            if(review == null)
            {
                await _dbContext.Reviews.AddAsync(repoReview);
            }
            else
            {
                review.Rating = repoReview.Rating;
                review.Review1 = repoReview.Review1;
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}