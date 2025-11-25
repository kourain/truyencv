using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace TruyenCV.Services;

public sealed class ChapterEnhancementService : IChapterEnhancementService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Convert2TvOptions _convertOptions;
    private readonly TtsServiceOptions _ttsOptions;
    private readonly ILogger<ChapterEnhancementService> _logger;

    public ChapterEnhancementService(
        IHttpClientFactory httpClientFactory,
        IOptions<Convert2TvOptions> convertOptionsAccessor,
        IOptions<TtsServiceOptions> ttsOptionsAccessor,
        ILogger<ChapterEnhancementService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _convertOptions = convertOptionsAccessor?.Value ?? new Convert2TvOptions();
        _ttsOptions = ttsOptionsAccessor?.Value ?? new TtsServiceOptions();
        _logger = logger;
    }

    public async Task<string> ConvertToVietnameseAsync(string content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new UserRequestException("Nội dung chương không hợp lệ");
        }

        if (string.IsNullOrWhiteSpace(_convertOptions.ServiceUrl))
        {
            _logger.LogWarning("Convert2TV service URL is not configured");
            throw new UserRequestException("Dịch vụ chuyển đổi tạm thời không khả dụng", statusCode: 503);
        }

        var payload = JsonConvert.SerializeObject(new Convert2TvPayload
        {
            messages = [new Convert2TvMessage
            {
                role = "user",
                content = $"<message>{content}</message>"
            }]
        });

        var client = _httpClientFactory.CreateClient(nameof(ChapterEnhancementService) + ":convert");
        client.Timeout = TimeSpan.FromSeconds(_convertOptions.TimeoutSeconds);

        using var request = new HttpRequestMessage(HttpMethod.Post, _convertOptions.ServiceUrl)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Convert2TV error {Status}: {Body}", (int)response.StatusCode, Truncate(raw));
            throw new UserRequestException("Không thể chuyển đổi nội dung chương. Vui lòng thử lại sau.", statusCode: (int)response.StatusCode);
        }

        var result = JsonConvert.DeserializeObject<Convert2TvResponse>(raw);
        var converted = result?.output[1]?.content.FirstOrDefault()?.textResponseComic.Trim();
        if (string.IsNullOrWhiteSpace(converted))
        {
            _logger.LogWarning("Convert2TV trả về nội dung trống");
            throw new UserRequestException("Dịch vụ Convert2TV không trả về nội dung hợp lệ");
        }

        return converted;
    }

    public async Task<ChapterTtsResult> GenerateTtsAsync(string content, string referenceAudio, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new UserRequestException("Nội dung chương không hợp lệ");
        }

        if (string.IsNullOrWhiteSpace(referenceAudio))
        {
            throw new UserRequestException("Bạn cần chọn giọng đọc");
        }

        if (string.IsNullOrWhiteSpace(_ttsOptions.ServiceUrl))
        {
            _logger.LogWarning("TTS service URL is not configured");
            throw new UserRequestException("Dịch vụ đọc truyện tạm thời không khả dụng", statusCode: 503);
        }

        var client = _httpClientFactory.CreateClient(nameof(ChapterEnhancementService) + ":tts");
        client.Timeout = TimeSpan.FromSeconds(_ttsOptions.TimeoutSeconds);

        using var form = new MultipartFormDataContent
        {
            { new StringContent(content, Encoding.UTF8), "text" },
            { new StringContent(referenceAudio, Encoding.UTF8), "reference_audio" },
            { new StringContent(_ttsOptions.Normalize ? "true" : "false", Encoding.UTF8), "normalize" },
            { new StringContent(content.ToSHA256String(), Encoding.UTF8), "output_name" }
        };

        using var response = await client.PostAsync(_ttsOptions.ServiceUrl, form, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("TTS error {Status}: {Body}", (int)response.StatusCode, Truncate(errorBody));
            throw new UserRequestException("Không thể tạo giọng đọc cho chương này. Vui lòng thử lại sau.", statusCode: (int)response.StatusCode);
        }

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "audio/wav";
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName
            ?? $"chapter_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.wav";
        fileName = fileName?.Trim('"') ?? $"chapter_{Guid.NewGuid():N}.wav";

        return new ChapterTtsResult
        {
            ContentType = contentType,
            FileName = fileName,
            Data = bytes
        };
    }

    public async Task<IReadOnlyList<string>> GetAvailableVoicesAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_ttsOptions.SoundListUrl))
        {
            return Array.Empty<string>();
        }

        var client = _httpClientFactory.CreateClient(nameof(ChapterEnhancementService) + ":tts-voices");
        client.Timeout = TimeSpan.FromSeconds(_ttsOptions.TimeoutSeconds);

        using var response = await client.GetAsync(_ttsOptions.SoundListUrl, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("TTS voice list error {Status}: {Body}", (int)response.StatusCode, Truncate(raw));
            return Array.Empty<string>();
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        try
        {
            var voices = JsonConvert.DeserializeObject<List<string>>(content) ?? new List<string>();
            return voices;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Không thể parse danh sách giọng đọc");
            return Array.Empty<string>();
        }
    }

    private static string Truncate(string value, int maxLength = 500)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength] + "…";
    }
}

internal sealed class Convert2TvPayload
{
    public required Convert2TvMessage[] messages { get; set; }
}

internal sealed class Convert2TvMessage
{
    public required string role { get; set; }

    public required string content { get; set; }
}
internal sealed class Content
{
    public string text { get; set; }
    public string type { get; set; }
    [JsonIgnore]
    public string textResponseComic
    {
        get
        {
            int pos0 = text.IndexOf("<resulttag>") + 11;
            int pos1 = text.IndexOf("</resulttag>");
            if (pos1 - pos0 > 0)
                return text.Substring(pos0, pos1 - pos0);
            else return "";
        }
    }
}
internal sealed class Output
{
    public string id { get; set; }
    public Content[] content { get; set; }
    public string role { get; set; }
    public string status { get; set; }
}
internal sealed class Usage
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}
internal sealed class Convert2TvResponse
{
    public string id { get; set; }
    public string model { get; set; }
    public Output[] output { get; set; }
    public Usage usage { get; set; }
}
