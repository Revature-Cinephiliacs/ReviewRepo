// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Logic;
// using Logic.Interfaces;
// using ReviewApi.Controllers;
// using Microsoft.EntityFrameworkCore;
// using Repository;
// using Xunit;

// namespace Testing
// {
//     public class UnitTest1
//     {
//         readonly DbContextOptions<Repository.Models.ReviewDBContext> dbOptions =
//             new DbContextOptionsBuilder<Repository.Models.ReviewDBContext>()
//         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

//         [Fact]
//         public async Task ReviewsTest()
//         {
//             GlobalModels.Review inputGMReview;
//             GlobalModels.Review outputGMReview;

//             RelatedDataSet dataSetA = new RelatedDataSet("JimmyJimerson", "ab10101010", "Theory");

//             // Seed the test database
//             using(var context = new Repository.Models.ReviewDBContext(dbOptions))
//             {
//                 context.Database.EnsureDeleted();
//                 context.Database.EnsureCreated();

//                 inputGMReview = Logic.ReviewMapper.RepoReviewToReview(dataSetA.Review);

//                 ReviewRepoLogic repoLogic = new ReviewRepoLogic(context);
//                 // Load Database entries for the object-under-test's foreign keys
//                 await reviewRepoLogic.AddUser(dataSetA.User);
//                 await reviewRepoLogic.AddMovie(dataSetA.Movie.MovieId);

//                 // Test CreateReview()
//                 IReviewLogic reviewLogic = new ReviewLogic(repoLogic);
//                 MovieController movieController = new MovieController(movieLogic);
//                 await movieController.CreateReview(inputGMReview);
//             }

//             using(var context = new Repository.Models.Cinephiliacs_DbContext(dbOptions))
//             {
//                 RepoLogic repoLogic = new RepoLogic(context);

//                 // Test GetReviews()
//                 IMovieLogic movieLogic = new MovieLogic(repoLogic);
//                 MovieController movieController = new MovieController(movieLogic);
//                 List<GlobalModels.Review> gmReviewList = (await movieController.GetReviews(dataSetA.Movie.MovieId)).Value;
//                 outputGMReview = gmReviewList
//                     .FirstOrDefault<GlobalModels.Review>(r => r.Movieid == dataSetA.Movie.MovieId);
//             }

//             Assert.Equal(inputGMReview, outputGMReview);
//         }
        
//         [Fact]
//         public async Task ReviewsPageTest()
//         {
//             GlobalModels.Review inputGMReview;
//             GlobalModels.Review outputGMReview;

//             RelatedDataSet dataSetA = new RelatedDataSet("JimmyJimerson", "ab10101010", "Theory");

//             // Seed the test database
//             using(var context = new Repository.Models.Cinephiliacs_DbContext(dbOptions))
//             {
//                 context.Database.EnsureDeleted();
//                 context.Database.EnsureCreated();

//                 inputGMReview = BusinessLogic.Mapper.RepoReviewToReview(dataSetA.Review);

//                 RepoLogic repoLogic = new RepoLogic(context);
//                 // Load Database entries for the object-under-test's foreign keys
//                 await repoLogic.AddUser(dataSetA.User);
//                 await repoLogic.AddMovie(dataSetA.Movie.MovieId);

//                 // Test CreateReview()
//                 IMovieLogic movieLogic = new MovieLogic(repoLogic);
//                 MovieController movieController = new MovieController(movieLogic);
//                 await movieController.CreateReview(inputGMReview);
//             }

//             using(var context = new Repository.Models.Cinephiliacs_DbContext(dbOptions))
//             {
//                 RepoLogic repoLogic = new RepoLogic(context);

//                 // Test GetReviews()
//                 IMovieLogic movieLogic = new MovieLogic(repoLogic);
//                 MovieController movieController = new MovieController(movieLogic);
//                 await movieController.SetReviewsPageSize(10);
//                 List<GlobalModels.Review> gmReviewList = (await movieController.GetReviewsPage(dataSetA.Movie.MovieId, 1, "ratingasc")).Value;
//                 outputGMReview = gmReviewList
//                     .FirstOrDefault<GlobalModels.Review>(r => r.Movieid == dataSetA.Movie.MovieId);
//             }

//             Assert.Equal(inputGMReview, outputGMReview);
//         }
//     }
// }
