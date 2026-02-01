using System.Linq;
using API.Infrastructure.RequestDTOs.Reviews;
using API.Infrastructure.ResponseDTOs.Reviews;
using Common.Entities;
using Common.Services;

namespace API.Controllers;

// ReviewsController inherits all CRUD operations from BaseCrudController
// Only needs to implement the mapping logic between DTOs and entities
public class ReviewsController : BaseCrudController<Review, ReviewService, ReviewRequest, ReviewResponse>
{
    // Maps ReviewRequest DTO to Review entity (for Create)
    protected override Review MapToEntity(ReviewRequest request)
    {
        return new Review
        {
            MovieId = request.MovieId,
            UserId = request.UserId,
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
        entity.UserId = request.UserId;
        entity.Rating = request.Rating;
        entity.Comment = request.Comment;
        // DatePosted remains unchanged on update
    }
}
