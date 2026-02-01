using System;

namespace API.Infrastructure.ResponseDTOs.Reviews;

public class ReviewResponse
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public string MovieTitle { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime ReviewDate { get; set; }
}
