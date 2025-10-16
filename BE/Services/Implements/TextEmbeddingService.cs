using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace TruyenCV.Services;

/// <summary>
/// Triển khai cơ bản tạo vector embedding dựa trên tần suất token.
/// </summary>
public class TextEmbeddingService : ITextEmbeddingService
{
    private static readonly Regex TokenRegex = new("[\\p{L}\\p{N}]{2,}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private readonly EmbeddingOptions _options;

    public TextEmbeddingService(IOptions<EmbeddingOptions> optionsAccessor)
    {
        _options = optionsAccessor?.Value ?? new EmbeddingOptions();
    }

    public EmbeddingOptions Options => _options;

    public bool TryCreateEmbedding(out float[] embedding, params string?[] parts)
    {
        embedding = new float[_options.Dimensions];

        if (parts is null || parts.Length == 0)
            return false;

        var tokens = new List<(string Token, float Weight)>();
        for (var index = 0; index < parts.Length; index++)
        {
            var segment = parts[index];
            if (string.IsNullOrWhiteSpace(segment))
                continue;

            var weight = index == 0 ? 3f : 1f; // ưu tiên tên truyện
            foreach (var token in Tokenize(segment))
            {
                tokens.Add((token, weight));
            }
        }

        if (tokens.Count == 0)
            return false;

        Span<float> span = embedding;
        foreach (var (token, weight) in tokens)
        {
            var position = GetStableHash(token) % span.Length;
            span[position] += weight;
        }

        var magnitudeSquared = 0f;
        for (var i = 0; i < span.Length; i++)
        {
            magnitudeSquared += span[i] * span[i];
        }

        if (magnitudeSquared <= 0f)
            return false;

        var magnitude = MathF.Sqrt(magnitudeSquared);
        var norm = 1f / magnitude;
        for (var i = 0; i < span.Length; i++)
        {
            span[i] *= norm;
        }

        return true;
    }

    private static IEnumerable<string> Tokenize(string input)
    {
        foreach (Match match in TokenRegex.Matches(input.ToLowerInvariant()))
        {
            yield return match.Value;
        }
    }

    private static int GetStableHash(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hashBytes = SHA256.HashData(bytes);
        var hash = BitConverter.ToInt32(hashBytes, 0) & int.MaxValue;
        return hash;
    }
}
