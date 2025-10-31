using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace TruyenCV.Services;

/// <summary>
/// Dịch vụ sinh embedding cho truyện thông qua microservice PgVector-Embedding.
/// Nếu microservice không khả dụng sẽ sử dụng giải pháp hashing dự phòng.
/// </summary>
public class TextEmbeddingService : ITextEmbeddingService
{
    private static readonly Regex TokenRegex = new("[\\p{L}\\p{N}]{2,}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private readonly EmbeddingOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TextEmbeddingService> _logger;
    private bool _loggedMissingServiceUrl;

    public TextEmbeddingService(IOptions<EmbeddingOptions> optionsAccessor, IHttpClientFactory httpClientFactory, ILogger<TextEmbeddingService> logger)
    {
        _options = optionsAccessor?.Value ?? new EmbeddingOptions();
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public EmbeddingOptions Options => _options;

    public async Task<float[][]?> CreateEmbeddingAsync(params string?[] segments)
    {
        if (segments == null || segments.Length == 0)
        {
            return null;
        }

        var remote = await TryRequestEmbeddingAsync(segments!);
        if (remote != null)
        {
            return remote;
        }
        return null;
    }

    private async Task<float[][]?> TryRequestEmbeddingAsync(IReadOnlyList<string> segments)
    {
        if (!_options.Enabled)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(_options.ServiceUrl))
        {
            if (!_loggedMissingServiceUrl)
            {
                _logger.LogWarning("Chưa cấu hình Search:ComicVector:ServiceUrl, sử dụng thuật toán embedding dự phòng.");
                _loggedMissingServiceUrl = true;
            }
            return null;
        }

        var payload = JsonSerializer.Serialize(new { texts = segments });
        var requestUri = _options.ServiceUrl;
        var attempts = Math.Max(1, _options.RetryCount + 1);
        var client = _httpClientFactory.CreateClient(nameof(TextEmbeddingService));
        client.Timeout = TimeSpan.FromSeconds(Math.Clamp(_options.TimeoutSeconds, 5, 120));

        for (var attempt = 0; attempt < attempts; attempt++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            try
            {
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                var body = await response.Content.ReadFromJsonAsync<JObject>();
                if (response.IsSuccessStatusCode)
                {
                    var results = new float[segments.Count][];
                    for (var i = 0; i < segments.Count; i++)
                    {
                        var embedding = body?["vectors"][i]?["values"]?.ToObject<float[]>();
                        if (embedding != null)
                        {
                            results[i] = embedding;
                        }
                    }
                    return results;
                }

                _logger.LogWarning("Microservice embedding trả về mã {StatusCode}: {Body}", (int)response.StatusCode, Truncate(body.ToString() ?? string.Empty));
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Hết thời gian chờ khi gọi microservice embedding (attempt {Attempt}/{Attempts}).", attempt + 1, attempts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi microservice embedding.");
                break;
            }

            if (attempt + 1 < attempts)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(_options.RetryDelayMilliseconds));
            }
        }

        return null;
    }

    private float[] CreateFallbackEmbedding(string segments)
    {
        var dimension = Math.Clamp(_options.Dimensions, 8, 2048);
        var embedding = new float[dimension];
        var tokens = new List<(string Token, float Weight)>();

        foreach (var token in Tokenize(segments))
        {
            tokens.Add((token, 1));
        }

        if (tokens.Count == 0)
        {
            return embedding;
        }

        foreach (var (token, weight) in tokens)
        {
            var index = GetStableHash(token) % embedding.Length;
            embedding[index] += weight;
        }

        return NormalizeInPlace(embedding);
    }

    private static float[] NormalizeInPlace(float[] vector)
    {
        if (vector.Length == 0)
        {
            return vector;
        }

        var magnitudeSquared = 0f;
        for (var i = 0; i < vector.Length; i++)
        {
            magnitudeSquared += vector[i] * vector[i];
        }

        if (magnitudeSquared <= 0f)
        {
            return vector;
        }

        var magnitude = MathF.Sqrt(magnitudeSquared);
        var norm = 1f / magnitude;
        for (var i = 0; i < vector.Length; i++)
        {
            vector[i] *= norm;
        }

        return vector;
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
        return BitConverter.ToInt32(hashBytes, 0) & int.MaxValue;
    }

    private static string Truncate(string value, int maxLength = 200)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength] + "…";
    }
}
