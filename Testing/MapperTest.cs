using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic;
using Models;
using Repository.Models;
using Xunit;

namespace Testing
{
    public class MapperTest
    {
        [Fact]
        public async Task TestReviewToReviewDto()
        {
            var review = new Review()
            {
                ReviewId = Guid.NewGuid(),
                UsernameId = Guid.NewGuid().ToString(),
                ImdbId = "Anis",
                Score = 34,
                Review1 = "Some reviews",
                CreationTime = DateTime.Now
            };

            var revDto = await ReviewMapper.RepoReviewToReviewAsync(review);

            Assert.Equal(review.ImdbId, revDto.Imdbid);

        }
        [Fact]
        public void TestReviewDtoToReview()
        {

            var reviewDto = new ReviewDto()
            {
                Reviewid = Guid.NewGuid(),
                Usernameid = Guid.NewGuid().ToString(),
                Imdbid = "Anis",
                Score = 34,
                Review = "Some reviews",
                CreationTime = DateTime.Now
            };
            var revDto = ReviewMapper.ReviewToRepoReview(reviewDto);

            Assert.Equal(reviewDto.Usernameid, revDto.UsernameId);

        }

        [Fact]
        public void TestReviewToReviewNotification()
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
            var revNoti = ReviewMapper.ReviewToReviewNotification(revDto);

            Assert.Equal(revNoti.Imdbid,revDto.Imdbid);
        }
    }
}
