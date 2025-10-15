namespace TruyenCV.DTOs.Response;

public class ComicCategoryResponse
{
	public string id { get; set; }
	public string name { get; set; }
	public CategoryType category_type { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }
}
