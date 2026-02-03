using API.Infrastructure.RequestDTOs.Reviews;
using API.Infrastructure.ResponseDTOs.Reviews;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ReviewsController : BaseCrudController<Review, ReviewService, ReviewRequest, ReviewResponse>
{
    public ReviewsController(ReviewService service) : base(service)
    {
    }

    protected override Review MapToEntity(ReviewRequest request)
    {
        return new Review
        {
            MovieId = request.MovieId,
            Rating = request.Rating,
            Comment = request.Comment,
            DatePosted = DateTime.Now
        };
    }
    
    // Maps Review entity to ReviewResponse DTO (for all responses)
    // Includes related Movie title and User username
    protected override ReviewResponse MapToResponse(Review entity)
    {
        return new ReviewResponse
        {
            Id = entity.Id,
            MovieId = entity.MovieId,
            MovieTitle = entity.Movie.Title,
            UserId = entity.UserId,
            Username = entity.User.Username,
            Rating = entity.Rating,
            Comment = entity.Comment,
            ReviewDate = entity.DatePosted  
        };
    }
    
    protected override void UpdateEntity(Review entity, ReviewRequest request)
    {
        entity.Rating = request.Rating;
        entity.Comment = request.Comment;
    }
    
    [HttpGet]
    public override IActionResult GetAll()
    {
        var reviews = Service.GetAllWithMovieAndUser();
        var response = reviews.Select(r => MapToResponse(r)).ToList();
        return Ok(response);
    }
    

    [HttpGet("{id}")]
    public override IActionResult GetById(int id)
    {
        var review = Service.GetByIdWithMovieAndUser(id);
        
        if (review == null)
            return NotFound();
        
        return Ok(MapToResponse(review));
    }
    
    [HttpPost]
    [Authorize]
    public override IActionResult Create([FromBody] ReviewRequest request)
    {
        var userIdClaim = User.FindFirst("loggedUserId")?.Value;
        int userId = int.Parse(userIdClaim);
        
        var existingReview = Service.GetExistingReview(userId, request.MovieId);
        
        if (existingReview != null)
        {
            return BadRequest("You have already reviewed this movie. Use PUT to update your review.");
        }
        
        var review = MapToEntity(request);
        review.UserId = userId;
        Service.Save(review);
        
        var savedReview = Service.GetByIdWithMovieAndUser(review.Id);
        return Ok(MapToResponse(savedReview));
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public override IActionResult Update(int id, [FromBody] ReviewRequest request)
    {
        var userIdClaim = User.FindFirst("loggedUserId")?.Value;
        int userId = int.Parse(userIdClaim);

        var review = Service.GetByIdWithMovieAndUser(id);
        if (review == null)
            return NotFound();

        if (review.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();
        
        UpdateEntity(review, request);
        Service.Save(review);
        
        var updatedReview = Service.GetByIdWithMovieAndUser(review.Id);
        return Ok(MapToResponse(updatedReview));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public override IActionResult Delete(int id)
    {
        var userIdClaim = User.FindFirst("loggedUserId")?.Value;
        int userId = int.Parse(userIdClaim);

        var review = Service.GetByIdWithMovieAndUser(id);
        if (review == null)
            return NotFound();

        if (review.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        Service.Delete(review);
        return Ok();
    }
}
