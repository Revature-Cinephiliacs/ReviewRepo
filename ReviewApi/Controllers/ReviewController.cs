using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using Logic.Interfaces;
using Repository.Models;

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
    }
}