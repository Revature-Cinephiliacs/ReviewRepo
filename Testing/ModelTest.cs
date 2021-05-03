﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Repository.Models;
using Xunit;

namespace Testing
{
    public class ModelTest
    {
        [Fact]
        public void TestReview()
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
            var expected = "Anis";
            var actual = review.ImdbId;
            Assert.Equal(expected,actual);
        }
        [Fact]
        public void TestSetting()
        {
            var setting = new Setting()
            {
                SettingId = Guid.NewGuid(),
                StringValue = "Anis",
                IntValue = 34,
                Setting1 = "Some setting",
               
            };
            var expected = "Anis";
            var actual = setting.StringValue;
            Assert.Equal(expected,actual);
        }
        [Fact]
        public void TestReviewDto()
        {
            var review = new ReviewDto()
            {
                Reviewid = Guid.NewGuid(),
                Usernameid = Guid.NewGuid().ToString(),
                Imdbid = "Anis",
                Score = 34,
                Review = "Some reviews",
                CreationTime = DateTime.Now
            };
            var expected = "Anis";
            var actual = review.Imdbid;
            Assert.Equal(expected,actual);
        }

        [Fact]
        public void TestReviewNotification()
        {
            var noti = new ReviewNotification()
            {
              Imdbid = "12345",
              Usernameid = "Anis",
             Reviewid   = Guid.NewGuid()
            };
            var expected = "12345";
            var actual = noti.Imdbid;

            Assert.Equal(expected,actual);
        }
    }
}
