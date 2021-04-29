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
            var review3 = new Review() { ReviewId = Guid.NewGuid(),UsernameId = Guid.NewGuid(),CreationTime = DateTime.Now,ImdbId = "12345",Score = 54};
            var review4 = new Review() { ReviewId = Guid.NewGuid(),UsernameId = Guid.NewGuid(), CreationTime = DateTime.Now,ImdbId = "12345",Score = 5};
            
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
            List<ReviewDto> result2 = null; 
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                result2 = await msr.GetReviews("12345");
            }
            Assert.Empty(result2);
        }
        [Fact]
        public async Task TestGetReviewsByUserId()
        {
            var id = Guid.NewGuid();
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
            List<ReviewDto> result2 = null; 
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                result2 = await msr.GetReviewsByUser(Guid.NewGuid());
            }
            Assert.Empty(result2);
        }
        [Fact]
        public async Task TestSettingReviewPage()
        {
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
                int page = 5;
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
                int page = -5;
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
            var id = Guid.NewGuid();
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
            var id = Guid.NewGuid();
            int page = 6;
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
                Setting set = new Setting() {SettingId = Guid.NewGuid(), IntValue = -5};
                page = (int)set.IntValue;
                result2 = await msr.GetReviewsPage("12345", page, "ratingasc");
            }
            Assert.Null(result2);
        }

        [Fact]
        public async Task ListOfReviewsByRating()
        {
            
            var review3 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 4
            };
            var review2 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 4
            };

            var review4 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid(), CreationTime = DateTime.Now, ImdbId = "12345",
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
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 4
            };
            var review2 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid(), CreationTime = DateTime.Now, ImdbId = "12345",
                Score = 4
            };

            var review4 = new Review()
            {
                ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid(), CreationTime = DateTime.Now, ImdbId = "12345",
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

            Assert.Empty(result2);
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

            Assert.Empty(result2);
        }
    }
}
