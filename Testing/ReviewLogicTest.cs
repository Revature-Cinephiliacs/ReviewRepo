using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic;
using Microsoft.EntityFrameworkCore;
using Models;
using Repository;
using Repository.Models;
using Xunit;

namespace Testing
{
    public class ReviewLogicTest
    {
        readonly DbContextOptions<Cinephiliacs_ReviewContext> dbOptions =
            new DbContextOptionsBuilder<Cinephiliacs_ReviewContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        [Fact]
        public async Task TestGetReviews()
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
                List<Review> reviews2 = await context1.Reviews.Where(r => r.ImdbId == "12345").ToListAsync();
                foreach (var revDto in reviews2)
                {
                  result1.Add(ReviewMapper.RepoReviewToReview(revDto));  
                }
            }
            List<ReviewDto> result2; 
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                result2 = await msr.GetReviews("12345");
            }
            Assert.Equal(result1.Count,result2.Count);
        }
        [Fact]
        public async Task TestGetReviewsBadPath()
        {
            var review = new Review() {ReviewId = Guid.NewGuid(), ImdbId = "123", UsernameId = "Anis"};
            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureCreated();
                context1.Database.EnsureDeleted();
                context1.Add(review);
                context1.SaveChanges();
            }
            List<ReviewDto> result2; 
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                result2 = await msr.GetReviews("12345");
            }
            Assert.Null(result2);
        }
        [Fact]
        public async Task TestGetReviewsByUserId()
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
                List<Review> reviews2 = await context1.Reviews.Where(r => r.UsernameId == id).ToListAsync();
                foreach (var revDto in reviews2)
                {
                    result1.Add(ReviewMapper.RepoReviewToReview(revDto));  
                }
            }
            List<ReviewDto> result2; 
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                result2 = await msr.GetReviewsByUser(id);
            }
            Assert.Equal(result1.Count,result2.Count);
        }
        [Fact]
        public async Task TestGetReviewsByUserIdBadPath()
        {
            List<ReviewDto> result2; 
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                result2 = await msr.GetReviewsByUser(Guid.NewGuid().ToString());
            }
            Assert.Null(result2);
        }
        [Fact]
        public async Task TestSettingReviewPage()
        {
            int page = 5;
            var setting = new Setting();
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
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                setting.Setting1 = "reviewspagesize";
                setting.IntValue = page;
                result = await msr.SetReviewsPageSize(page);
            }
            Assert.True(result);
        }
        [Fact]
        public async Task TestSettingReviewPageBadPath()
        {
            int page = -5;
            var setting = new Setting();
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
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                setting.Setting1 = "reviewspagesize";
                setting.IntValue = page;
                result = await msr.SetReviewsPageSize(page);
            }
            Assert.False(result);
        }

        [Fact]
        public async Task TestGetReviewPageBadPath()
        {
            var id = Guid.NewGuid().ToString();
            int page = -5;
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
                List<Review> reviews2 = await context1.Reviews.Where(r => r.UsernameId == id).ToListAsync();
                foreach (var revDto in reviews2)
                {
                    result1.Add(ReviewMapper.RepoReviewToReview(revDto));  
                }
            }
            List<ReviewDto> result2; 
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));

                result2 = await msr.GetReviewsPage("12345", page, "ratingasc");
            }
            Assert.Null(result2);
        }
        [Fact]
        public async Task TestGetReviewPageSizeBadPath()
        {
            
            var id = Guid.NewGuid().ToString();
            int page = 6;
            List<Review> reviews = new List<Review>();
            var review1 = new Review() { ReviewId = Guid.NewGuid(),UsernameId = id,CreationTime = DateTime.Now,ImdbId = "12345",Score = 4};
            var review2 = new Review() { ReviewId = Guid.NewGuid(),UsernameId = id, CreationTime = DateTime.Now,ImdbId = "12345",Score = 5};

            var setting = new Setting() {SettingId = Guid.NewGuid(), IntValue = -6,Setting1 = "reviewspagesize"};
            
            reviews.Add(review1);
            reviews.Add(review2);
            List<ReviewDto> result1 = new List<ReviewDto>();
            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Reviews.Add(review1);
                context1.Reviews.Add(review2);
                context1.Settings.Add(setting);
                context1.SaveChanges();
                List<Review> reviews2 = await context1.Reviews.Where(r => r.UsernameId == id).ToListAsync();
                foreach (var revDto in reviews2)
                {
                    result1.Add(ReviewMapper.RepoReviewToReview(revDto));  
                }
            }
            List<ReviewDto> result2 ; 
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                var msr2 = new ReviewRepoLogic(context2);
                Setting set = msr2.GetSetting("reviewspagesize");
                int pagesize = (int) set.IntValue;
                if (pagesize < 1)
                {
                    result2 = null;
                }
                else
                {
                    result2 =  await msr.GetReviewsPage("12345", page, "ratingasc");
                }
                
            }
            Assert.Null(result2);
        }

        [Fact]
        public async Task ListOfReviewsByRating()
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
            List<ReviewDto> result1 = new List<ReviewDto>();

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(review3);
                context1.Add(review4);
                context1.Add(review2);
                context1.SaveChanges();
                foreach (var review in await context1.Reviews.Where(r => r.Score == 4).ToListAsync())
                {
                    result1.Add(ReviewMapper.RepoReviewToReview(review));
                }

            }

            List<ReviewDto> result2 = new List<ReviewDto>();
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2)); 
                result2 =  await msr.GetReviewsByRating(4);

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
            List<ReviewDto> result1 = new List<ReviewDto>();

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(review3);
                context1.Add(review4);
                context1.Add(review2);
                context1.SaveChanges();
                foreach (var review in await context1.Reviews.Where(r => r.Score == 5 && r.ImdbId == "12345").ToListAsync())
                {
                    result1.Add(ReviewMapper.RepoReviewToReview(review));
                }

            }

            List<ReviewDto> result2 = new List<ReviewDto>();
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2)); 
                result2 =  await msr.GetReviewsByRating("12345",5);

            }

            Assert.Equal(result2.Count, result1.Count);
        }
        [Fact]
        public async Task ListOfReviewsByRatingAndImdbBadPath()
        {
            
            List<ReviewDto> result1 = new List<ReviewDto>();

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.SaveChanges();
                foreach (var review in await context1.Reviews.Where(r => r.Score == 5 && r.ImdbId == "12345").ToListAsync())
                {
                    result1.Add(ReviewMapper.RepoReviewToReview(review));
                }

            }

            List<ReviewDto> result2 = new List<ReviewDto>();
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2)); 
                result2 =  await msr.GetReviewsByRating("12345",5);

            }

            Assert.Null(result2);
        }
        [Fact]
        public async Task ListOfReviewsByRatingBadPath()
        {
            
            List<ReviewDto> result1 = new List<ReviewDto>();

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.SaveChanges();
                foreach (var review in await context1.Reviews.Where(r => r.Score == 5 && r.ImdbId == "12345").ToListAsync())
                {
                    result1.Add(ReviewMapper.RepoReviewToReview(review));
                }

            }

            List<ReviewDto> result2 = new List<ReviewDto>();
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2)); 
                result2 =  await msr.GetReviewsByRating(5);

            }

            Assert.Null(result2);
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
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                result =  msr.getOneReview(review.ReviewId).Result;
                result.Review1 = "Other stuff";
                msr.UpdatedRev(result);
            }
            Assert.NotEqual(result.Review1,review.Review1);
        }
        [Fact]
        public void TestDeleteRev()
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
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                msr.deleteReview(review);
                result =  msr.getOneReview(review.ReviewId).Result;
            }
            Assert.NotEqual(review,result);
        }
        
        [Fact]
        public async Task AddReviewtest()
        {
            var review = new Review() { ReviewId = Guid.NewGuid(),UsernameId = Guid.NewGuid().ToString(),CreationTime = DateTime.Now,ImdbId = "12345",Score = 54};

            ReviewDto reviewDto;
            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(review);
                reviewDto = ReviewMapper.RepoReviewToReview(review);
                context1.SaveChanges();
            }

            bool result;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                result= msr.CreateReview(reviewDto).Result;

            }
            Assert.True(result);

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
            List<ReviewDto> result2;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                List<string> ids = new List<string>();
                foreach (var guid in idGuids)
                {
                    ids.Add(guid.ToString());
                }
                result2 = await msr.GetReviewsByIDS(ids);
            }

            Assert.Equal(result2.Count, result1.Count);
        }

        [Fact]
        public async void TestSendNotification()
        {
            ReviewLogic reviewLogic = new ReviewLogic(new ReviewRepoLogic(new Cinephiliacs_ReviewContext(dbOptions)));
            var expected = false;
            var actual = await reviewLogic.SendNotification(new ReviewNotification());
            Assert.Equal(expected, actual);
        }
    }
}
