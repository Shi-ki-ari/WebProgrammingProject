using System;
using System.Linq;
using API.Infrastructure.RequestDTOs.Reviews;
using API.Infrastructure.ResponseDTOs.Reviews;
using Common.Entities;
using Common.Persistence;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ReviewsController : BaseCrudController<Review, ReviewService, ReviewRequest, ReviewResponse>
{
    private readonly AppDbContext _context;

    public ReviewsController(ReviewService service, AppDbContext context) : base(service)
    {
        _context = context;
    }

    protected override Review MapToEntity(ReviewRequest request)
    {
        // Extract user ID from JWT token claims
        var userIdClaim = User.FindFirst("loggedUserId")?.Value;
        var userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;
        
        return new Review
        {
            MovieId = request.MovieId,
            UserId = userId,  // Get from authenticated user's token
            Rating = request.Rating,
            Comment = request.Comment,
            DatePosted = DateTime.Now  // Server sets the review date
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
            MovieTitle = entity.Movie?.Title ?? "Unknown",  // Get title from navigation property
            UserId = entity.UserId,
            Username = entity.User?.Username ?? "Unknown",  // Get username from navigation property
            Rating = entity.Rating,
            Comment = entity.Comment,
            ReviewDate = entity.DatePosted  // Map DatePosted to ReviewDate for response
        };
    }
    
    // Updates existing Review entity with values from ReviewRequest (for Update)
    protected override void UpdateEntity(Review entity, ReviewRequest request)
    {
        entity.MovieId = request.MovieId;
        // UserId cannot be changed - it stays with the original creator
        entity.Rating = request.Rating;
        entity.Comment = request.Comment;
        // DatePosted remains unchanged on update
    }
    
    // Override GetAll to use eager loading (public - no auth required)
    [HttpGet]
    public override IActionResult GetAll()
    {
        var reviews = _context.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .ToList();
        var response = reviews.Select(r => MapToResponse(r)).ToList();
        return Ok(response);
    }
    
    // Override GetById to use eager loading (public - no auth required)
    [HttpGet("{id}")]
    public override IActionResult GetById(int id)
    {
        var review = _context.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .FirstOrDefault(r => r.Id == id);
        
        if (review == null)
            return NotFound();
        
        return Ok(MapToResponse(review));
    }
    
    [HttpPost]
    [Authorize]
    public override IActionResult Create([FromBody] ReviewRequest request)
    {
        if (request.Rating < 1 || request.Rating > 5)
        {
            return BadRequest("Rating must be between 1 and 5 stars.");
        }

        var userIdClaim = User.FindFirst("loggedUserId")?.Value;
        var userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;
        
        var existingReview = _context.Reviews
            .FirstOrDefault(r => r.UserId == userId && r.MovieId == request.MovieId);
        
        if (existingReview != null)
        {
            return BadRequest(new { message = "You have already reviewed this movie. Use PUT to update your review." });
        }
        
        var review = MapToEntity(request);
        Service.Save(review);
        
        var savedReview = _context.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .FirstOrDefault(r => r.Id == review.Id);
        return Ok(MapToResponse(savedReview));
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public override IActionResult Update(int id, [FromBody] ReviewRequest request)
    {
        if (request.Rating < 1 || request.Rating > 5)
        {
            return BadRequest(new { message = "Rating must be between 1 and 5 stars." });
        }

        var userIdClaim = User.FindFirst("loggedUserId")?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var review = Service.GetById(id);
        if (review == null)
            return NotFound();

        // Check ownership - only allow users to edit their own reviews (admins can edit any)
        if (review.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();
        
        UpdateEntity(review, request);
        Service.Save(review);
        
        var updatedReview = _context.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .FirstOrDefault(r => r.Id == review.Id);
        return Ok(MapToResponse(updatedReview));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public override IActionResult Delete(int id)
    {
        // Get userId from JWT token
        var userIdClaim = User.FindFirst("loggedUserId")?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var review = Service.GetById(id);
        if (review == null)
            return NotFound();

        // Check ownership - only allow users to delete their own reviews (admins can delete any)
        if (review.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        Service.Delete(review);
        return Ok();
    }
}
