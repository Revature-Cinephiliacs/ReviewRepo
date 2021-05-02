using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using Logic.Interfaces;
using Newtonsoft.Json;
using Repository.Models;
using RestSharp;
using ReviewApi.AuthenticationHelper;

namespace ReviewApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewLogic _reviewLogic;
        public ReviewController(IReviewLogic reviewLogic)
        {
            _reviewLogic = reviewLogic;
        }

        /// <summary>
        /// return a list of reviews depending on specific user stored in the DB
        /// if the user exist but no review it will throw a no content status code
        /// if the user doesn't exist it'll throw 404 not found
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        [HttpGet("ByUserId/{userid}")]
        public async Task<ActionResult<List<ReviewDto>>> GetReviewsByUserId(string userid)
        {
            List<ReviewDto> revDto = await _reviewLogic.GetReviewsByUser(userid);
            if (revDto == null)
            {
                return StatusCode(404);
            }
            if(revDto.Count == 0)
            {
                return  StatusCode(204);
            }

            StatusCode(200);
            return revDto;
        }
        
        /// <summary>
        /// Returns a list of all ReviewDto objects for the specified movie ID.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        [HttpGet("{movieid}")]
        public async Task<ActionResult<List<ReviewDto>>> GetReviews(string movieid)
        {
            List<ReviewDto> reviews = await _reviewLogic.GetReviews(movieid);

            if(reviews == null)
            {
                return StatusCode(404);
            }
            if(reviews.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return reviews;
        }

        /// <summary>
        /// Returns ReviewDto objects [n*(page-1), n*(page-1) + n] that are associated with the
        /// provided movie ID. Where n is the current page size for comments and sortorder
        /// is a string that determines how the Reviews are sorted before pagination.
        /// </summary>
        /// <param name="movieid"></param>
        /// <param name="page"></param>
        /// <param name="sortorder"></param>
        /// <returns></returns>
        [HttpGet("{movieid}/{page}/{sortorder}")]
        public async Task<ActionResult<List<ReviewDto>>> GetReviewsPage(string movieid, int page, string sortorder)
        {
            List<ReviewDto> reviews = await _reviewLogic.GetReviewsPage(movieid, page, sortorder);

            if(reviews == null)
            {
                return StatusCode(404);
            }
            if(reviews.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return reviews;
        }

        /// <summary>
        /// Sets the page size for reviews
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpPost("page/{pagesize}")]
        public async Task<ActionResult> SetReviewsPageSize(int pagesize)
        {
            if(await _reviewLogic.SetReviewsPageSize(pagesize))
            {
                return StatusCode(201);
            }
            
            return StatusCode(400);
            
        }

        /// <summary>
        /// Adds a new Movie ReviewDto based on the information provided.
        /// Returns a 400 status code if creation fails.
        /// </summary>
        /// <param name="reviewDto"></param>
        /// <returns></returns>
        [HttpPost("reviewDto")]
        [Authorize]
        public async Task<ActionResult> CreateReview([FromBody] ReviewDto reviewDto)
        {
            var response = await Helper.Sendrequest("/userdata", Method.GET, Helper.GetTokenFromRequest(this.Request));
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            var userid = dictionary["sub"];

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ReviewController.CreateReview() was called with invalid body data.");
                return StatusCode(400);
            }

            if(reviewDto.Usernameid == userid && await _reviewLogic.CreateReview(reviewDto) )
            {
                return StatusCode(201);
            }
            
            return StatusCode(400);
        }
        /// <summary>
        /// updates the reviews posted by the user
        /// first it'll check if the review exist in the database if not it'll throw 404
        /// </summary>
        /// <param name="reviewId"></param>
        /// <param name="reviewDto"></param>
        /// <returns></returns>
        [HttpPut("update/admin/{reviewId}")]
        [Authorize("manage:awebsite")]
        public async Task<ActionResult> updateMovieAdmin(Guid reviewId,Review reviewDto)
        {
            var reviewExist = await _reviewLogic.getOneReview(reviewId);
            if (reviewExist != null)
            {
                reviewDto.ReviewId = reviewExist.ReviewId;
                _reviewLogic.UpdatedRev(reviewDto);
                return  StatusCode(200);
            }
            return  StatusCode(404);
        }
        [HttpPut("update/user/{reviewId}")]
        [Authorize]
        public async Task<ActionResult> updateMovieUser(Guid reviewId,Review reviewDto)
        {
            var response = await Helper.Sendrequest("/userdata", Method.GET, Helper.GetTokenFromRequest(this.Request));
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            var userid = dictionary["sub"];

            var reviewExist = await _reviewLogic.getOneReview(reviewId);
            if (reviewDto.UsernameId == userid && reviewExist != null)
            {
                reviewDto.ReviewId = reviewExist.ReviewId;
                _reviewLogic.UpdatedRev(reviewDto);
                return  StatusCode(200);
            }
            return  StatusCode(404);
        }
        /// <summary>
        /// delete a review by a specific reviewId
        /// it'll check if the review exist 
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        [HttpDelete("deleteRev/admin/{reviewId}")]
        [Authorize("manage:awebsite")]
        public async Task<ActionResult> deleteRevAdmin(Guid reviewId)
        {
            var rev = await _reviewLogic.getOneReview(reviewId);
            if (rev != null)
            {
                _reviewLogic.deleteReview(rev);
                return  StatusCode(200);
            }

            return StatusCode(404);
        }
        [HttpDelete("deleteRev/user/{reviewId}")]
        [Authorize]
        public async Task<ActionResult> deleteRev(Guid reviewId)
        {
            var response = await Helper.Sendrequest("/userdata", Method.GET, Helper.GetTokenFromRequest(this.Request));
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            var userid = dictionary["sub"];


            var rev = await _reviewLogic.getOneReview(reviewId);
            if (rev.UsernameId == userid && rev != null)
            {
                _reviewLogic.deleteReview(rev);
                return  StatusCode(200);
            }

            return StatusCode(404);
        }
        /// <summary>
        /// return a list of all the reviews in the database based on a score rating
        /// </summary>
        /// <param name="rating"></param>
        /// <returns></returns>
        [HttpGet("/byRating/{rating}")]
        public async Task<ActionResult<List<ReviewDto>>> GetReviewsByRating(int rating)
        {
            List<ReviewDto> reviews = await _reviewLogic.GetReviewsByRating(rating);

            if(reviews == null)
            {
                return StatusCode(404);
            }
            if(reviews.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return reviews;
        }

        /// <summary>
        /// return a list of all the reviews in the database based on a score rating of a unique movie
        /// </summary>
        /// <param name="rating"></param>
        /// <returns></returns>
        [HttpGet("/byRating/{imdb}/{rating}")]
        public async Task<ActionResult<List<ReviewDto>>> GetReviewsByRating(string imdb,int rating)
        {
            List<ReviewDto> reviews = await _reviewLogic.GetReviewsByRating(imdb,rating);

            if(reviews == null)
            {
                return StatusCode(404);
            }
            if(reviews.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return reviews;
        }
        /// <summary>
        /// return a list of reviews bases on a ReviewId List  (Admin tool)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("reportedReviews")]
        public async Task<ActionResult<List<ReviewDto>>> getListOfReviewsByIDS([FromBody] List<string> ids)
        {
            List<ReviewDto> revDto = await _reviewLogic.GetReviewsByIDS(ids);
            if(revDto == null)
            {
                return StatusCode(404);
            }
            if (revDto.Count == 0)
            {
                return StatusCode(204);
            }
            StatusCode(200);
            return revDto;
        }
    }
}
