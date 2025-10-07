using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
	// Convert CreateComicCategoryRequest to ComicCategory entity
	public static ComicCategory ToEntity(this CreateComicCategoryRequest request)
	{
		return new ComicCategory
		{
			name = request.name,
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow
		};
	}

	// Convert ComicCategory entity to ComicCategoryResponse
	public static ComicCategoryResponse ToRespDTO(this ComicCategory category)
	{
		return new ComicCategoryResponse
		{
			id = category.id,
			name = category.name,
			created_at = category.created_at,
			updated_at = category.updated_at
		};
	}

	// Update ComicCategory entity from UpdateComicCategoryRequest
	public static void UpdateFromRequest(this ComicCategory category, UpdateComicCategoryRequest request)
	{
		category.name = request.name;
		category.updated_at = DateTime.UtcNow;
	}
}
