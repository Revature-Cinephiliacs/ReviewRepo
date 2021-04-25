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
        private readonly Cinephiliacs_ReviewContext _dbContext;

        public ReviewRepoLogic(Cinephiliacs_ReviewContext dbContext)
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
                    && r.ImdbId == repoReview.ImdbId).FirstOrDefaultAsync();
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
            return await _dbContext.Reviews.Where(r => r.ImdbId == movieid).ToListAsync();
        }

        /// <summary>
        /// Gets the value(s) of an existing setting in the database with a matching key string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Setting GetSetting(string key)
        {
            return _dbContext.Settings.FirstOrDefault(s => s.Setting1 == key);
        }

        /// <summary>
        /// Creates a new setting entry or updates the value(s) of an existing setting
        /// in the database.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public async Task<bool> SetSetting(Setting setting)
        {
            if(setting == null || setting.Setting1.Length < 1)
            {
                Console.WriteLine("RepoLogic.SetSetting() was called with a null or invalid setting.");
                return false;
            }
            if(SettingExists(setting.Setting1))
            {
                Setting existentSetting = await _dbContext.Settings.Where(
                    s => s.Setting1 == setting.Setting1).FirstOrDefaultAsync();
                if(setting.IntValue != null)
                {
                    existentSetting.IntValue = setting.IntValue;
                }
                if(setting.StringValue != null)
                {
                    existentSetting.StringValue = setting.StringValue;
                }
            }
            else
            {
                await _dbContext.Settings.AddAsync(setting);
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Returns true iff the setting key, specified in the argument, exists in the database's Settings table.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool SettingExists(string key)
        {
            return (_dbContext.Settings.FirstOrDefault(s => s.Setting1 == key) != null);
        }
    }
}