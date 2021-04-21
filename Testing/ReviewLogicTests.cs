using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic;
using Logic.Interfaces;
using ReviewApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Repository;
using Xunit;

namespace Testing
{
    public class ReviewLogicTests
    {
        readonly DbContextOptions<Repository.Models.Reviews_DbContext> dbOptions =
            new DbContextOptionsBuilder<Repository.Models.Reviews_DbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        [Fact]
        public async Task ReviewsTest()
        {
            GlobalModels.Review inputGMReview;
            GlobalModels.Review outputGMReview;

            RelatedDataSet dataSetA = new RelatedDataSet("JimmyJimerson", "ab10101010", "Theory");

            // Seed the test database
            using(var context = new Repository.Models.Reviews_DbContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                inputGMReview = Logic.ReviewMapper.RepoReviewToReview(dataSetA.Review);

                ReviewRepoLogic repoLogic = new ReviewRepoLogic(context);
                // Load Database entries for the object-under-test's foreign keys
                await repoLogic.AddUser(dataSetA.User);
                await repoLogic.AddMovie(dataSetA.Movie.MovieId);

                // Test CreateReview()
                IReviewLogic reviewLogic = new ReviewLogic(repoLogic);
                ReviewController reviewController = new ReviewController(reviewLogic);
                await reviewController.CreateReview(inputGMReview);
            }

            using(var context = new Repository.Models.Cinephiliacs_DbContext(dbOptions))
            {
                ReviewRepoLogic repoLogic = new ReviewRepoLogic(context);

                // Test GetReviews()
                IReviewLogic reviewLogic = new ReviewLogic(repoLogic);
                ReviewController reviewController = new ReviewController(reviewLogic);
                List<GlobalModels.Review> gmReviewList = (await reviewController.GetReviews(dataSetA.Movie.MovieId)).Value;
                outputGMReview = gmReviewList
                    .FirstOrDefault<GlobalModels.Review>(r => r.Movieid == dataSetA.Movie.MovieId);
            }

            Assert.Equal(inputGMReview, outputGMReview);
        }
        
        [Fact]
        public async Task ReviewsPageTest()
        {
            GlobalModels.Review inputGMReview;
            GlobalModels.Review outputGMReview;

            RelatedDataSet dataSetA = new RelatedDataSet("JimmyJimerson", "ab10101010", "Theory");

            // Seed the test database
            using(var context = new Repository.Models.Cinephiliacs_DbContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                inputGMReview = BusinessLogic.Mapper.RepoReviewToReview(dataSetA.Review);

                ReviewRepoLogic repoLogic = new ReviewRepoLogic(context);
                // Load Database entries for the object-under-test's foreign keys
                await repoLogic.AddUser(dataSetA.User);
                await repoLogic.AddMovie(dataSetA.Movie.MovieId);

                // Test CreateReview()
                IReviewLogic reviewLogic = new ReviewLogic(repoLogic);
                ReviewController reviewController = new ReviewController(reviewLogic);
                await reviewController.CreateReview(inputGMReview);
            }

            using(var context = new Repository.Models.Cinephiliacs_DbContext(dbOptions))
            {
                ReviewRepoLogic repoLogic = new ReviewRepoLogic(context);

                // Test GetReviews()
                IReviewLogic reviewLogic = new ReviewLogic(repoLogic);
                ReviewController reviewController = new ReviewController(reviewLogic);
                await reviewController.SetReviewsPageSize(10);
                List<GlobalModels.Review> gmReviewList = (await reviewController.GetReviewsPage(dataSetA.Movie.MovieId, 1, "ratingasc")).Value;
                outputGMReview = gmReviewList
                    .FirstOrDefault<GlobalModels.Review>(r => r.Movieid == dataSetA.Movie.MovieId);
            }

            Assert.Equal(inputGMReview, outputGMReview);
        }
}