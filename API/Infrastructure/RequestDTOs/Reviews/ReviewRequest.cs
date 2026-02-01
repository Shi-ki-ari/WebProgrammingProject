namespace API.Infrastructure.RequestDTOs.Reviews;

public class ReviewRequest
{
    public int MovieId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
}
