using System.ComponentModel;

namespace TruyenCV;

public enum ReportStatus
{
    [Description("Chờ xử lý")] Pending = 1,
    [Description("Đang xử lý")] InProgress = 2,
    [Description("Đã giải quyết")] Resolved = 3,
    [Description("Từ chối")] Rejected = 4
}
