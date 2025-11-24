using TruyenCV;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

public interface IComicReportRepository : IRepository<ComicReport>
{
    Task<IEnumerable<ComicReport>> GetByStatusAsync(ReportStatus? status, int offset, int limit);
    Task<IEnumerable<ComicReport>> GetByUserIdAsync(long userId, int offset, int limit);
    Task<IEnumerable<ComicReport>> GetByComicOwnerAsync(long ownerId, int offset, int limit, ReportStatus? status = null);
}
