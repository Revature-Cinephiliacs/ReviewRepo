using System;
using System.Collections.Generic;
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
        public void TestReviewToReviewDto()
        {
            var review = new Review()
            {
                ReviewId = Guid.NewGuid(),
                UsernameId = Guid.NewGuid(),
                ImdbId = "Anis",
                Score = 34,
                Review1 = "Some reviews",
                CreationTime = DateTime.Now
            };
          
            var revDto =  ReviewMapper.RepoReviewToReview(review);

            Assert.Equal(review.UsernameId,revDto.Usernameid);

        }   [Fact]
        public void TestReviewDtoToReview()
        {
         
            var reviewDto = new ReviewDto()
            {
                Reviewid = Guid.NewGuid(),
                Usernameid = Guid.NewGuid(),
                Imdbid = "Anis",
                Score = 34,
                Review = "Some reviews",
                CreationTime = DateTime.Now
            };
            var revDto =  ReviewMapper.ReviewToRepoReview(reviewDto);

            Assert.Equal(reviewDto.Usernameid,revDto.UsernameId);

        }

    }
}
