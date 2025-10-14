namespace TruyenCV.DTO.Request;

// Request DTOs for creating and updating comic categories
public class CreateComicCategoryRequest
{
	public required string name { get; set; }
}

public class UpdateComicCategoryRequest
{
	public required string id { get; set; }
	public required string name { get; set; }
}
