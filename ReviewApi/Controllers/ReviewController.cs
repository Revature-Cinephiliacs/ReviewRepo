using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using Logic.Interfaces;
using Repository.Models;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

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
        /// Example for using authentication
        /// </summary>
        /// <returns></returns>
        [HttpGet("users")]
        [Authorize]
        public async Task<ActionResult<string>> GetExample()
        {
            return Ok(new { response = "success" });
        }
        /// <summary>
        /// return a list of reviews depending on specific user stored in the DB
        /// if the user exist but no review it will throw a no content status code
        /// if the user doesn't exist it'll throw 404 not found
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        [HttpGet("ByUserId/{userId}")]
        public async Task<ActionResult<List<ReviewDto>>> GetReviewsByUserId(Guid userId)
        {
            List<ReviewDto> revDto = await _reviewLogic.GetReviewsByUser(userId);
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
        public async Task<ActionResult> CreateReview([FromBody] ReviewDto reviewDto)
        {
            if(!ModelState.IsValid)
            {
                Console.WriteLine("ReviewController.CreateReview() was called with invalid body data.");
                return StatusCode(400);
            }

            if(await _reviewLogic.CreateReview(reviewDto))
            {
                return StatusCode(201);
            }
            
            return StatusCode(400);
            
        }

        /// <summary>
        /// When CreateReview is called successfully, it will trigger this method to send a notification
        /// to Movies to get a list of userids who follow the movie associated with the imdbid contained
        /// in the notification.
        /// </summary>
        /// <param name="reviewNotification"></param>
        /// <returns></returns>
        public async Task<ActionResult<ReviewNotification>> SendNotification(ReviewNotification reviewNotification)
        {
            HttpClient client = new HttpClient();
            string path = "http://20.94.153.81/movie/review/notification";
            HttpResponseMessage response = await client.PostAsJsonAsync(path, reviewNotification);
            if(response.IsSuccessStatusCode)
            {   
                return StatusCode(200);
            }
            else
            {
                return StatusCode(400);
            }
         
        }

        /// <summary>
        /// updates the reviews posted by the user
        /// first it'll check if the review exist in the database if not it'll throw 404
        /// </summary>
        /// <param name="reviewId"></param>
        /// <param name="reviewDto"></param>
        /// <returns></returns>
        [HttpPatch("update/{reviewId}")]
        public async Task<ActionResult> updateMovie(Guid reviewId,Review reviewDto)
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
        /// <summary>
        /// delete a review by a specific reviewId
        /// it'll check if the review exist 
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        [HttpDelete("delete/{reviewId}")]
        public async Task<ActionResult> deleteRev(Guid reviewId)
        {
            var rev = await _reviewLogic.getOneReview(reviewId);
            if (rev != null)
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
