using System;
using System.Collections.Generic;

namespace TruyenCV.DTO.Response;

public class UserProfileResponse
{
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
    public string avatar { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? email_verified_at { get; set; }
    public DateTime? banned_at { get; set; }
    public bool is_banned { get; set; }
    public string read_comic_count { get; set; } = string.Empty;
    public string read_chapter_count { get; set; } = string.Empty;
    public string bookmark_count { get; set; } = string.Empty;
    public string coin { get; set; } = string.Empty;
    public IReadOnlyCollection<string> roles { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> permissions { get; set; } = Array.Empty<string>();
}
