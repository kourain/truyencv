using TruyenCV.Models;

namespace TruyenCV.DTOs;

/// <summary>
/// Wrapper class để lưu Comic entity cùng với match score từ search
/// </summary>
public class ComicSearchResult
{
	public Comic Comic { get; set; } = null!;
	public double Score { get; set; } // 0.0 - 1.0, higher is better
}
