namespace TruyenCV.DTO.Request;

// Request DTOs for creating and updating comic have category
public class CreateComicHaveCategoryRequest
{
	public required long comic_id { get; set; }
	public required long comic_category_id { get; set; }
}

public class UpdateComicHaveCategoryRequest
{
	public required long comic_id { get; set; }
	public required long comic_category_id { get; set; }
	public required long new_comic_id { get; set; }
	public required long new_comic_category_id { get; set; }
}
