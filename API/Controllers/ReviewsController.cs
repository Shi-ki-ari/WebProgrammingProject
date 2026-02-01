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

// ReviewsController inherits all CRUD operations from BaseCrudController
// Only needs to implement the mapping logic between DTOs and entities
public class ReviewsController : BaseCrudController<Review, ReviewService, ReviewRequest, ReviewResponse>
{
    // Maps ReviewRequest DTO to Review entity (for Create)
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
    [AllowAnonymous]
    public override IActionResult GetAll()
    {
        using (var context = new AppDbContext())
        {
            var reviews = context.Reviews
                .Include(r => r.Movie)
                .Include(r => r.User)
                .ToList();
            var response = reviews.Select(r => MapToResponse(r)).ToList();
            return Ok(response);
        }
    }
    
    // Override GetById to use eager loading (public - no auth required)
    [HttpGet("{id}")]
    [AllowAnonymous]
    public override IActionResult GetById(int id)
    {
        using (var context = new AppDbContext())
        {
            var review = context.Reviews
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == id);
            
            if (review == null)
                return NotFound();
            
            return Ok(MapToResponse(review));
        }
    }
    
    // Override Create - requires authentication
    [Authorize]
    public override IActionResult Create([FromBody] ReviewRequest request)
    {
        var review = MapToEntity(request);
        Service.Save(review);
        
        // Reload with eager loading to get navigation properties
        using (var context = new AppDbContext())
        {
            var savedReview = context.Reviews
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == review.Id);
            return Ok(MapToResponse(savedReview));
        }
    }
    
    // Override Update - requires authentication
    [Authorize]
    public override IActionResult Update(int id, [FromBody] ReviewRequest request)
    {
        var review = Service.GetById(id);
        if (review == null)
            return NotFound();
        
        UpdateEntity(review, request);
        Service.Save(review);
        
        // Reload with eager loading to get navigation properties
        using (var context = new AppDbContext())
        {
            var updatedReview = context.Reviews
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == review.Id);
            return Ok(MapToResponse(updatedReview));
        }
    }
}
