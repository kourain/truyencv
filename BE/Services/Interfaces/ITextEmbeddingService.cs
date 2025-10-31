using System.Threading.Tasks;

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
    /// <param name="parts">Danh sách chuỗi cần embedding.</param>
    /// <returns>Vector kết quả hoặc null nếu dữ liệu không hợp lệ.</returns>
    Task<float[][]?> CreateEmbeddingAsync(params string?[] parts);
}

/// <summary>
/// Tùy chọn cấu hình cho dịch vụ embedding.
/// </summary>
public class EmbeddingOptions
{
    private int _dimensions = EmbeddingDefaults.Dimensions;
    private int _maxResults = EmbeddingDefaults.MaxResults;
    private double _minScore = EmbeddingDefaults.MinScore;
    private int _timeoutSeconds = 15;
    private int _retryCount = 2;
    private int _retryDelayMilliseconds = 1000;

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

    /// <summary>
    /// URL của microservice sinh embedding. Ví dụ: http://localhost:44644/embed
    /// </summary>
    public string? ServiceUrl { get; set; }

    /// <summary>
    /// Cho phép vô hiệu hoá gọi microservice khi cần.
    /// </summary>
    public bool Enabled { get; set; } = true;

    public int TimeoutSeconds
    {
        get => _timeoutSeconds;
        set => _timeoutSeconds = Math.Clamp(value, 5, 120);
    }

    public int RetryCount
    {
        get => _retryCount;
        set => _retryCount = Math.Clamp(value, 0, 5);
    }

    public int RetryDelayMilliseconds
    {
        get => _retryDelayMilliseconds;
        set => _retryDelayMilliseconds = Math.Clamp(value, 100, 10000);
    }
}
