using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using Logic.Interfaces;

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
        /// Returns a list of all Review objects for the specified movie ID.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        [HttpGet("{movieid}")]
        public async Task<ActionResult<List<Review>>> GetReviews(string movieid)
        {
            List<Review> reviews = await _reviewLogic.GetReviews(movieid);

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
        /// Returns Review objects [n*(page-1), n*(page-1) + n] that are associated with the
        /// provided movie ID. Where n is the current page size for comments and sortorder
        /// is a string that determines how the Reviews are sorted before pagination.
        /// </summary>
        /// <param name="movieid"></param>
        /// <param name="page"></param>
        /// <param name="sortorder"></param>
        /// <returns></returns>
        [HttpGet("{movieid}/{page}/{sortorder}")]
        public async Task<ActionResult<List<Review>>> GetReviewsPage(string movieid, int page, string sortorder)
        {
            List<Review> reviews = await _reviewLogic.GetReviewsPage(movieid, page, sortorder);

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
            else
            {
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Adds a new Movie Review based on the information provided.
        /// Returns a 400 status code if creation fails.
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPost("review")]
        public async Task<ActionResult> CreateReview([FromBody] Review review)
        {
            if(!ModelState.IsValid)
            {
                Console.WriteLine("ReviewController.CreateReview() was called with invalid body data.");
                return StatusCode(400);
            }

            if(await _reviewLogic.CreateReview(review))
            {
                return StatusCode(201);
            }
            else
            {
                return StatusCode(400);
            }
        }
    }
}