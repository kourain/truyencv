namespace TruyenCV.DTOs.Request;

// Request DTOs for creating and updating comic have category
public class CreateComicHaveCategoryRequest
{
	public required string comic_id { get; set; }
	public required string comic_category_id { get; set; }
}

public class UpdateComicHaveCategoryRequest
{
	public required string comic_id { get; set; }
	public required string comic_category_id { get; set; }
	public required string new_comic_id { get; set; }
	public required string new_comic_category_id { get; set; }
}
