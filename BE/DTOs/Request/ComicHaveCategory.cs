namespace TruyenCV.DTO.Request;

// Request DTOs for creating and updating comic have category
public class CreateComicHaveCategoryRequest
{
	public required ulong comic_id { get; set; }
	public required ulong comic_category_id { get; set; }
}

public class UpdateComicHaveCategoryRequest
{
	public required ulong comic_id { get; set; }
	public required ulong comic_category_id { get; set; }
	public required ulong new_comic_id { get; set; }
	public required ulong new_comic_category_id { get; set; }
}
