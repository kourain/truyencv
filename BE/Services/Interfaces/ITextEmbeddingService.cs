using System.Diagnostics.CodeAnalysis;

namespace TruyenCV.Services;

/// <summary>
/// Cung cấp chức năng sinh embedding vector từ nội dung văn bản.
/// </summary>
public interface ITextEmbeddingService
{
    /// <summary>
    /// Giá trị cấu hình hiện tại cho embedding.
    /// </summary>
    EmbeddingOptions Options { get; }

    /// <summary>
    /// Tạo embedding từ một hoặc nhiều đoạn văn bản.
    /// </summary>
    /// <param name="embedding">Vector kết quả (luôn có kích thước Options.Dimensions).</param>
    /// <param name="parts">Danh sách chuỗi cần embedding.</param>
    /// <returns>True nếu embedding chứa thông tin hữu ích, false nếu văn bản rỗng.</returns>
    bool TryCreateEmbedding([NotNullWhen(true)] out float[] embedding, params string?[] parts);
}

/// <summary>
/// Tùy chọn cấu hình cho dịch vụ embedding.
/// </summary>
public class EmbeddingOptions
{
    private int _dimensions = EmbeddingDefaults.Dimensions;
    private int _maxResults = EmbeddingDefaults.MaxResults;
    private double _minScore = EmbeddingDefaults.MinScore;

    public int Dimensions
    {
        get => _dimensions;
        set => _dimensions = Math.Clamp(value, 8, 2048);
    }

    public int MaxResults
    {
        get => _maxResults;
        set => _maxResults = Math.Clamp(value, 1, 200);
    }

    public double MinScore
    {
        get => _minScore;
        set => _minScore = Math.Clamp(value, 0.0, 0.99);
    }
}
