using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic;
using Microsoft.EntityFrameworkCore;
using Models;
using Repository;
using Repository.Models;
using Xunit;

namespace Testing
{
    public class RepoLogicTest
    {
        readonly DbContextOptions<Cinephiliacs_ReviewContext> dbOptions =
            new DbContextOptionsBuilder<Cinephiliacs_ReviewContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        [Fact]
        public async Task ListOfReviewsByImdb()
        {
            List<Review> reviews = new List<Review>();
            var review3 = new Review() { ReviewId = Guid.NewGuid(),UsernameId = "Anis",CreationTime = DateTime.Now,ImdbId = "12345",Score = 54};
            var review4 = new Review() { ReviewId = Guid.NewGuid(),UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now,ImdbId = "12345",Score = 5};
            
            reviews.Add(review3);
            reviews.Add(review4);

            List<Review> result1; 

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(review3);
                context1.Add(review4);
                context1.SaveChanges();
                result1 = await context1.Reviews.Where(r => r.ImdbId == "12345").ToListAsync();
            }
            List<Review> result2; 
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                result2 = await msr.GetMovieReviews("12345");
            }
            Assert.Equal(result2.Count,result1.Count);
        }
        [Fact]
        public async Task ListOfReviewsDtoByImdb()
        {
            List<Review> reviews = new List<Review>();
            var review3 = new Review() { ReviewId = Guid.NewGuid(),UsernameId = Guid.NewGuid().ToString(),CreationTime = DateTime.Now,ImdbId = "12345",Score = 54};
            var review4 = new Review() { ReviewId = Guid.NewGuid(),UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now,ImdbId = "12345",Score = 5};
            
            reviews.Add(review3);
            reviews.Add(review4);

            List<ReviewDto> result1 = new List<ReviewDto>(); 

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(review3);
                context1.Add(review4);
                context1.SaveChanges();
                List<Review> reviewsResult = await context1.Reviews.Where(r => r.ImdbId == "12345").ToListAsync();
                foreach (var review in reviewsResult)
                {
                    result1.Add(ReviewMapper.RepoReviewToReview(review));
                }
            }
            List<ReviewDto> result2 = new List<ReviewDto>(); 
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                List<Review> reviewsResult = await msr.GetMovieReviews("12345");
                foreach (var review in reviewsResult)
                {
                    result2.Add(ReviewMapper.RepoReviewToReview(review));
                }
            }
            Assert.Equal(result2,result1);
        }

        [Fact]
        public async Task AddReviewtest()
        {
            var review = new Review() { ReviewId = Guid.NewGuid(),UsernameId = Guid.NewGuid().ToString(),CreationTime = DateTime.Now,ImdbId = "12345",Score = 54};

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(review);
                context1.SaveChanges();

            }

            bool result;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                result= msr.AddReview(review).Result;

            }
            Assert.True(result);

        }
        [Fact]
        public async Task SettingTest()
        {
            var setting = new Setting(){Setting1 = "Anis",IntValue = 34,StringValue = "Medini"};

            Setting result1;
            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Settings.Add(setting);
                
                context1.SaveChanges();
                result1 = await context1.Settings.Where(r => r.Setting1 == setting.Setting1).FirstOrDefaultAsync();
               
            }
            Setting result2;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                result2 = msr.GetSetting(setting.Setting1);
               
            }
            Assert.Equal(result2.StringValue,result1.StringValue);
        }

        [Fact] 
        public async Task SetSettingTestNotHappyPath()
        {
            var review = new Review() { ReviewId = Guid.NewGuid(),UsernameId = Guid.NewGuid().ToString(),CreationTime = DateTime.Now,ImdbId = "12345",Score = 54};


            bool result;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                result = await msr.SetSetting(null);

            }
            Assert.False(result);
        }
        [Fact] 
        public async Task SetSettingTestHappyPath()
        {
            var setting = new Setting() { SettingId = Guid.NewGuid(),Setting1 = "Anis",IntValue = 54,StringValue = "Medini"};

            var result1 = new Setting();
            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Settings.Add(setting);
                
                context1.SaveChanges(); 
               result1 = await context1.Settings.Where(r => r.Setting1 == setting.Setting1).FirstOrDefaultAsync();
               
            }

            bool result;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                result = await msr.SetSetting(setting);

            }
            Assert.True(result);
        }

        [Fact] 
        public void SetSettingExistTest()
        {
            var setting = new Setting() { SettingId = Guid.NewGuid(),Setting1 = "Anis",IntValue = 54,StringValue = "Medini"};
            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Settings.Add(setting);
                context1.SaveChanges();

            }
            bool result;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                result = msr.SettingExists(setting.Setting1);

            }
            Assert.True(result);
        }

        [Fact]
        public async Task GetallMovieByUserId()
        {
            var id = Guid.NewGuid().ToString();
            List<Review> reviews = new List<Review>();
            var review3 = new Review() { ReviewId = Guid.NewGuid(),UsernameId = id,CreationTime = DateTime.Now,ImdbId = "12345",Score = 54};
            var review4 = new Review() { ReviewId = Guid.NewGuid(),UsernameId = id, CreationTime = DateTime.Now,ImdbId = "12345",Score = 5};
            reviews.Add(review3);
            reviews.Add(review4);

            List<ReviewDto> result1 = new List<ReviewDto>(); 

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(review3);
                context1.Add(review4);
                context1.SaveChanges();
                List<Review> reviewsResult = await context1.Reviews.Where(r => r.UsernameId == id).ToListAsync();
                foreach (var review in reviewsResult)
                {
                    result1.Add(ReviewMapper.RepoReviewToReview(review));
                }
            }
            List<ReviewDto> result2 = new List<ReviewDto>(); 
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                List<Review> reviewsResult = await msr.getListofReviewsByUser(id);
                foreach (var review in reviewsResult)
                {
                    result2.Add(ReviewMapper.RepoReviewToReview(review));
                }
            }
            Assert.Equal(result2.Count,result1.Count);
        }

        [Fact]
        public void TestDelete()
        {
            var review = new Review() {ReviewId = Guid.NewGuid(), Review1 = "Some stuff"};

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Reviews.Add(review);
                context1.SaveChanges();

            }

            Review result;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                msr.deleteReview(review);
                result =  msr.getSingleReview(review.ReviewId).Result;
            }
            Assert.NotEqual(review,result);
        }

        [Fact]
        public void TestUpdateReview()
        {
            var review = new Review() {ReviewId = Guid.NewGuid(), Review1 = "Some stuff"};

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Reviews.Add(review);
                context1.SaveChanges();

            }

            Review result;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                result =  msr.getSingleReview(review.ReviewId).Result;
                result.Review1 = "Other stuff";
                msr.updateReview(result);
            }
            Assert.NotEqual(result.Review1,review.Review1);
        }

        [Fact]
        public async Task ListOfReviewsByRating()
        {
            List<Review> reviews = new List<Review>();
            var review3 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 4
            };
            var review2 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 4
            };

            var review4 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 5
            };

            reviews.Add(review3);
            reviews.Add(review4);
            reviews.Add(review2);

            List<Review> result1;

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(review3);
                context1.Add(review4);
                context1.Add(review2);
                context1.SaveChanges();
                result1 = await context1.Reviews.Where(r => r.Score == 4).ToListAsync();
            }

            List<Review> result2;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                result2 = await msr.getAllReviewByRating(4);
            }

            Assert.Equal(result2.Count, result1.Count);
        }
        [Fact]
        public async Task ListOfReviewsByRatingAndImdb()
        {
            var review3 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 4
            };
            var review2 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 4
            };

            var review4 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 5
            };

           

            List<Review> result1;

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(review3);
                context1.Add(review4);
                context1.Add(review2);
                context1.SaveChanges();
                result1 = await context1.Reviews.Where(r => r.Score == 5 && r.ImdbId == "12345").ToListAsync();
            }

            List<Review> result2;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                result2 = await msr.getAllReviewByRating("12345",5);
            }

            Assert.Equal(result2.Count, result1.Count);
        }

        [Fact]
        public async Task TestGetReviewsByIds()
        {
            var review1 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 4
            };
            var review2 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 4
            };

            var review3 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 5
            };
            List<Guid> idGuids = new List<Guid>();
            idGuids.Add(review1.ReviewId);
            idGuids.Add(review2.ReviewId);
            idGuids.Add(review3.ReviewId);

            List<Review> result1;
            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(review1);
                context1.Add(review2);
                context1.Add(review3);
                context1.SaveChanges();
                result1 = await context1.Reviews.ToListAsync();
            }
            List<Review> result2;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewRepoLogic(context2);
                List<string> ids = new List<string>();
                foreach (var guid in idGuids)
                {
                    ids.Add(guid.ToString());
                }
                result2 = await msr.getAllReviewsBYIDS(ids);
            }

            Assert.Equal(result2.Count, result1.Count);
        }
    }
}
