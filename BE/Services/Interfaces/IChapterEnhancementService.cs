using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TruyenCV.Services;

public interface IChapterEnhancementService
{
    Task<string> ConvertToVietnameseAsync(string content, CancellationToken cancellationToken = default);

    Task<ChapterTtsResult> GenerateTtsAsync(string content, string referenceAudio, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetAvailableVoicesAsync(CancellationToken cancellationToken = default);
}

public sealed class Convert2TvOptions
{
    private int _timeoutSeconds = 30;

    public string? ServiceUrl { get; set; }

    public int TimeoutSeconds
    {
        get => _timeoutSeconds;
        set => _timeoutSeconds = Math.Clamp(value, 5, 180);
    }
}

public sealed class TtsServiceOptions
{
    private int _timeoutSeconds = 60;

    public string? ServiceUrl { get; set; }

    public string? SoundListUrl { get; set; }

    public bool Normalize { get; set; } = true;

    public int TimeoutSeconds
    {
        get => _timeoutSeconds;
        set => _timeoutSeconds = Math.Clamp(value, 5, 300);
    }
}

public sealed class ChapterTtsResult
{
    public required string FileName { get; set; }

    public required string ContentType { get; set; }

    public required byte[] Data { get; set; }
}
