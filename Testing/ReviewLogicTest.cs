using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Repository;
using Repository.Models;
using ReviewApi.Controllers;
using Xunit;

namespace Testing
{
    public class ReviewLogicTest
    {

        readonly DbContextOptions<Cinephiliacs_ReviewContext> dbOptions =
            new DbContextOptionsBuilder<Cinephiliacs_ReviewContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        private readonly ILogger<ReviewLogic> logicLogger = new ServiceCollection().AddLogging().BuildServiceProvider()
            .GetService<ILoggerFactory>().CreateLogger<ReviewLogic>();

        [Fact]
        public async Task TestGetReviews()
        {
            List<Review> reviews = new List<Review>();
            var review3 = new Review() { ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345", Score = 54 };
            var review4 = new Review() { ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345", Score = 5 };

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
                    result1.Add(await ReviewMapper.RepoReviewToReviewAsync(revDto));
                }
            }
            List<ReviewDto> result2;
            ActionResult<List<ReviewDto>> result3;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                var msr2 = new ReviewController(msr);
                result3 = await msr2.GetReviews("12345");
                result2 = await msr.GetReviews("12345");
            }
            Assert.Equal(result1.Count, result2.Count);
            Assert.Equal(result1.Count, result3.Value.Count);
        }
        [Fact]
        public async Task TestGetReviewsBadPath()
        {
            var review = new Review() { ReviewId = Guid.NewGuid(), ImdbId = "123", UsernameId = "Anis" };
            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureCreated();
                context1.Database.EnsureDeleted();
                context1.Add(review);
                context1.SaveChanges();
            }
            List<ReviewDto> result2;
            ActionResult<List<ReviewDto>> result3;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2),logicLogger);
                var msr2 = new ReviewController(msr);
                result3 = await msr2.GetReviews("12345");
                result2 = await msr.GetReviews("12345");
            }
            Assert.Null(result2);
            Assert.Null(result3.Value);
        }
        [Fact]
        public async Task TestGetReviewsByUserId()
        {
            var id = Guid.NewGuid().ToString();
            List<Review> reviews = new List<Review>();
            var review3 = new Review() { ReviewId = Guid.NewGuid(), UsernameId = id, CreationTime = DateTime.Now, ImdbId = "12345", Score = 54 };
            var review4 = new Review() { ReviewId = Guid.NewGuid(), UsernameId = id, CreationTime = DateTime.Now, ImdbId = "12345", Score = 5 };

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
                    result1.Add(await ReviewMapper.RepoReviewToReviewAsync(revDto));
                }
            }
            List<ReviewDto> result2;
            ActionResult<List<ReviewDto>> result3;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                var msr2 = new ReviewController(msr);
                result3 = await msr2.GetReviewsByUserId(id);
                result2 = await msr.GetReviewsByUser(id);
            }
            Assert.Equal(result1.Count, result2.Count);
            Assert.Equal(result1.Count, result3.Value.Count);
        }
        [Fact]
        public async Task TestGetReviewsByUserIdBadPath()
        {
            List<ReviewDto> result2;
            ActionResult<List<ReviewDto>> result3;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                var msr2 = new ReviewController(msr);
                result3 = await msr2.GetReviewsByUserId(Guid.NewGuid().ToString());
                result2 = await msr.GetReviewsByUser(Guid.NewGuid().ToString());
            }
            Assert.Empty(result2);
            Assert.Null(result3.Value);
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
            ActionResult result2;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {

                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                setting.Setting1 = "reviewspagesize";
                setting.IntValue = page;
                result = await msr.SetReviewsPageSize(page);
                var msr2 = new ReviewController(msr);
                result2 = await msr2.SetReviewsPageSize(page);

            }
            Assert.True(result);
            Assert.NotNull(result2);
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
            ActionResult result2;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {

                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2),logicLogger);
                var msr2 = new ReviewController(msr);
                setting.Setting1 = "reviewspagesize";
                setting.IntValue = page;
                result = await msr.SetReviewsPageSize(page);
                result2 = await msr2.SetReviewsPageSize(page);
            }
            Assert.False(result);
            Assert.Equal("Microsoft.AspNetCore.Mvc.StatusCodeResult", result2.ToString());
        }

        [Fact]
        public async Task TestGetReviewPageBadPath()
        {
            var id = Guid.NewGuid().ToString();
            int page = -5;
            List<Review> reviews = new List<Review>();
            var review3 = new Review() { ReviewId = Guid.NewGuid(), UsernameId = id, CreationTime = DateTime.Now, ImdbId = "12345", Score = 54 };
            var review4 = new Review() { ReviewId = Guid.NewGuid(), UsernameId = id, CreationTime = DateTime.Now, ImdbId = "12345", Score = 5 };

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
                    result1.Add(await ReviewMapper.RepoReviewToReviewAsync(revDto));
                }
            }
            List<ReviewDto> result2;
            ActionResult<List<ReviewDto>> result3;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2),logicLogger);
                var msr2 = new ReviewController(msr);
                result2 = await msr.GetReviewsPage("12345", page, "ratingasc");
                result3 = await msr2.GetReviewsPage("12345", page, "ratingasc");
            }
            Assert.Null(result2);
            Assert.Null(result3.Value);
        }
        [Fact]
        public async Task TestGetReviewPageSizeBadPath()
        {

            var id = Guid.NewGuid().ToString();

            List<Review> reviews = new List<Review>();
            var review1 = new Review() { ReviewId = Guid.NewGuid(), UsernameId = id, CreationTime = DateTime.Now, ImdbId = "12345", Score = 4 };
            var review2 = new Review() { ReviewId = Guid.NewGuid(), UsernameId = id, CreationTime = DateTime.Now, ImdbId = "12345", Score = 5 };

            var setting = new Setting() { SettingId = Guid.NewGuid(), IntValue = -6, Setting1 = "reviewspagesize" };

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
                    result1.Add(await ReviewMapper.RepoReviewToReviewAsync(revDto));
                }
            }
            List<ReviewDto> result2;
            ActionResult<List<ReviewDto>> result3 = null;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                int page = 6;
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                var msr2 = new ReviewRepoLogic(context2);
                var msr3 = new ReviewController(msr);
                Setting set = msr2.GetSetting("reviewspagesize");
                int pagesize = (int)set.IntValue;
                if (pagesize < 1)
                {
                    result2 = null;
                }
                else
                {
                    result2 = await msr.GetReviewsPage("12345", page, "ratingasc");
                    result3 = await msr3.GetReviewsPage("12345", page, "ratingasc");
                }

            }
            Assert.Null(result2);
            Assert.Null(result3);

        }

        [Fact]
        public async Task ListOfReviewsByRating()
        {

            var review3 = new Review()
            {
                ReviewId = Guid.NewGuid(),
                UsernameId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.Now,
                ImdbId = "12345",
                Score = 4
            };
            var review2 = new Review()
            {
                ReviewId = Guid.NewGuid(),
                UsernameId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.Now,
                ImdbId = "12345",
                Score = 4
            };

            var review4 = new Review()
            {
                ReviewId = Guid.NewGuid(),
                UsernameId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.Now,
                ImdbId = "12345",
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
                    result1.Add(await ReviewMapper.RepoReviewToReviewAsync(review));
                }

            }

            List<ReviewDto> result2;
            ActionResult<List<ReviewDto>> result3;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                var msr2 = new ReviewController(msr);
                result2 = await msr.GetReviewsByRating(4);
                result3 = await msr2.GetReviewsByRating(4);

            }

            Assert.Equal(result2.Count, result1.Count);
            Assert.Equal(result2.Count, result3.Value.Count);
        }

        [Fact]
        public async Task ListOfReviewsByRatingAndImdb()
        {

            var review3 = new Review()
            {
                ReviewId = Guid.NewGuid(),
                UsernameId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.Now,
                ImdbId = "12345",
                Score = 4
            };
            var review2 = new Review()
            {
                ReviewId = Guid.NewGuid(),
                UsernameId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.Now,
                ImdbId = "12345",
                Score = 4
            };

            var review4 = new Review()
            {
                ReviewId = Guid.NewGuid(),
                UsernameId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.Now,
                ImdbId = "12345",
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
                    result1.Add(await ReviewMapper.RepoReviewToReviewAsync(review));
                }

            }

            List<ReviewDto> result2;
            ActionResult<List<ReviewDto>> result3;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                var msr2 = new ReviewController(msr);
                result2 = await msr.GetReviewsByRating("12345", 5);
                result3 = await msr2.GetReviewsByRating("12345", 5);

            }

            Assert.Equal(result2.Count, result1.Count);
            Assert.Equal(result2.Count, result3.Value.Count);
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
                    result1.Add(await ReviewMapper.RepoReviewToReviewAsync(review));
                }

            }

            List<ReviewDto> result2;
            ActionResult<List<ReviewDto>> result3;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2),logicLogger);
                var msr2 = new ReviewController(msr);
                result2 = await msr.GetReviewsByRating("12345", 5);
                result3 = await msr2.GetReviewsByRating("12345", 5);

            }

            Assert.Null(result2);
            Assert.Null(result3.Value);
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
                    result1.Add(await ReviewMapper.RepoReviewToReviewAsync(review));
                }

            }

            List<ReviewDto> result2;
            ActionResult<List<ReviewDto>> result3;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2),logicLogger);
                var msr2 = new ReviewController(msr);
                result2 = await msr.GetReviewsByRating(5);
                result3 = await msr2.GetReviewsByRating(5);

            }

            Assert.Null(result2);
            Assert.Null(result3.Value);
        }
        [Fact]
        public void TestUpdateReview()
        {
            var review = new Review() { ReviewId = Guid.NewGuid(), Review1 = "Some stuff" };

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
                result = msr.getOneReview(review.ReviewId).Result;
                result.Review1 = "Other stuff";
                msr.UpdatedRev(result);
            }
            Assert.NotEqual(result.Review1, review.Review1);
        }
        [Fact]
        public async Task TestDeleteRev()
        {
            var review = new Review() { ReviewId = Guid.NewGuid(), Review1 = "Some stuff" };

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

                result = await msr.getOneReview(review.ReviewId);

            }
            Assert.NotEqual(review, result);

        }
        [Fact]
        public void TestDeleteRev2()
        {
            var review = new Review() { ReviewId = Guid.NewGuid(), Review1 = "Some stuff" };

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Reviews.Add(review);
                context1.SaveChanges();

            }

            TaskStatus result;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                var msr2 = new ReviewController(msr);

                result = msr2.deleteRev(review.ReviewId).Status;

            }
            Assert.Equal(7, (int)result);

        }

        [Fact]
        public async Task AddReviewtest()
        {
            var review = new Review() { ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345", Score = 54 };
            var review2 = new Review() { ReviewId = Guid.NewGuid(), UsernameId = Guid.NewGuid().ToString(), CreationTime = DateTime.Now, ImdbId = "12345", Score = 54 };

            ReviewDto reviewDto;
            ReviewDto reviewDto2;
            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Add(review);
                context1.Add(review2);
                reviewDto = await ReviewMapper.RepoReviewToReviewAsync(review);
                reviewDto2 = await ReviewMapper.RepoReviewToReviewAsync(review2);
                context1.SaveChanges();
            }

            bool result;
            TaskStatus result2;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                var msr2 = new ReviewController(msr);
                result = msr.CreateReview(reviewDto).Result;
                result2 = msr2.CreateReview(reviewDto2).Status;

            }
            Assert.True(result);
            Assert.Equal(7, (int)result2);

        }
        [Fact]
        public async Task TestGetReviewsByIds()
        {
            var review1 = new Review()
            {
                ReviewId = Guid.NewGuid(),
                UsernameId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.Now,
                ImdbId = "12345",
                Score = 4
            };
            var review2 = new Review()
            {
                ReviewId = Guid.NewGuid(),
                UsernameId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.Now,
                ImdbId = "12345",
                Score = 4
            };

            var review3 = new Review()
            {
                ReviewId = Guid.NewGuid(),
                UsernameId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.Now,
                ImdbId = "12345",
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
            ActionResult<List<ReviewDto>> result3;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2));
                var msr2 = new ReviewController(msr);
                List<string> ids = new List<string>();
                foreach (var guid in idGuids)
                {
                    ids.Add(guid.ToString());
                }
                result2 = await msr.GetReviewsByIDS(ids);
                result3 = await msr2.getListOfReviewsByIDS(ids);
            }

            Assert.Equal(result1.Count, result2.Count);
            Assert.Equal(result1.Count, result3.Value.Count);
        }

        [Fact]
        public async Task TestGetReviewsByIdsBadPath()
        {

            List<Guid> idGuids = new List<Guid>();

            using (var context1 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.SaveChanges();
            }
            ActionResult<List<ReviewDto>> result3;
            using (var context2 = new Cinephiliacs_ReviewContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new ReviewLogic(new ReviewRepoLogic(context2),logicLogger);
                var msr2 = new ReviewController(msr);
                List<string> ids = new List<string>();
                foreach (var guid in idGuids)
                {
                    ids.Add(guid.ToString());
                }
                result3 = await msr2.getListOfReviewsByIDS(ids);
            }

            Assert.Null(result3.Value);
        }

        [Fact]
        public async void TestSendNotification()
        {
            ReviewLogic reviewLogic = new ReviewLogic(new ReviewRepoLogic(new Cinephiliacs_ReviewContext(dbOptions)));
            var expected = false;
            var actual = await reviewLogic.SendNotification(new ReviewNotification());
            Assert.Equal(expected, actual);
        }
      
        [Fact]
        public void TestGetReviewNotification()
        {
            var revDto = new ReviewDto()
            {
                Imdbid = "12345",
                Usernameid = "Anis",
                Reviewid = Guid.NewGuid(),
                Score = 4,
                Review = "Really good",
                CreationTime = DateTime.Now
            };
            var msr = new ReviewLogic();
            var revNoti = msr.GetReviewNotification(revDto);

            Assert.Equal(revNoti.Reviewid,revDto.Reviewid);
        }
    }
}
