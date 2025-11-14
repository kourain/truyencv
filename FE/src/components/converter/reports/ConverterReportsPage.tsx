"use client";

import { useMemo, useState } from "react";
import { Filter, RefreshCcw } from "lucide-react";

import { ReportStatus, ReportStatusLabel } from "@const/enum/report-status";
import ReportStatusBadge from "@components/admin/reports/ReportStatusBadge";
import { formatRelativeTime } from "@helpers/format";
import { useConverterReportsQuery } from "@services/converter";

const STATUS_OPTIONS: { label: string; value: ReportStatus | null }[] = [
  { label: "Tất cả", value: null },
  { label: ReportStatusLabel[ReportStatus.Pending], value: ReportStatus.Pending },
  { label: ReportStatusLabel[ReportStatus.InProgress], value: ReportStatus.InProgress },
  { label: ReportStatusLabel[ReportStatus.Resolved], value: ReportStatus.Resolved },
  { label: ReportStatusLabel[ReportStatus.Rejected], value: ReportStatus.Rejected },
];

const ConverterReportsPage = () => {
  const [statusFilter, setStatusFilter] = useState<ReportStatus | null>(null);
  const [limit, setLimit] = useState(20);
  const params = useMemo(() => ({ status: statusFilter, limit }), [statusFilter, limit]);

  const { data, isLoading, isFetching, refetch } = useConverterReportsQuery(params);

  const reports = data ?? [];

  return (
    <section className="space-y-6">
      <header className="rounded-2xl border border-surface-muted/60 bg-surface p-4 shadow-sm">
        <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div>
            <p className="text-xs uppercase tracking-wide text-primary">Trạng thái báo cáo</p>
            <h1 className="text-xl font-semibold text-primary-foreground">Báo cáo liên quan đến truyện của bạn</h1>
            <p className="text-sm text-surface-foreground/70">Theo dõi và xử lý khi người đọc gửi báo cáo về truyện hoặc chương.</p>
          </div>
          <div className="flex flex-wrap items-center gap-3">
            <div className="flex items-center gap-2 rounded-full border border-surface-muted/60 bg-surface px-3 py-1 text-xs text-surface-foreground/70">
              <Filter className="h-4 w-4 text-primary" />
              <select
                className="bg-transparent text-sm focus:outline-none"
                value={String(statusFilter ?? "all")}
                onChange={(event) => {
                  const value = event.target.value;
                  setStatusFilter(value === "all" ? null : (Number(value) as ReportStatus));
                }}
              >
                {STATUS_OPTIONS.map((option) => (
                  <option key={option.label} value={option.value ?? "all"}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>
            <div className="flex items-center gap-2 rounded-full border border-surface-muted/60 bg-surface px-3 py-1 text-xs text-surface-foreground/70">
              <span>Giới hạn</span>
              <select
                className="bg-transparent text-sm focus:outline-none"
                value={limit}
                onChange={(event) => setLimit(Number(event.target.value))}
              >
                {[10, 20, 50].map((value) => (
                  <option key={value} value={value}>
                    {value}
                  </option>
                ))}
              </select>
            </div>
            <button
              type="button"
              className="inline-flex items-center gap-2 rounded-full border border-primary/50 px-4 py-2 text-sm font-medium text-primary transition hover:bg-primary/10"
              onClick={() => refetch()}
              disabled={isFetching}
            >
              <RefreshCcw className={`h-4 w-4 ${isFetching ? "animate-spin" : ""}`} />
              Làm mới
            </button>
          </div>
        </div>
      </header>

      <div className="rounded-2xl border border-surface-muted/60 bg-surface shadow-sm">
        <div className="border-b border-surface-muted/60 px-4 py-3 text-sm text-surface-foreground/70">
          {isFetching ? "Đang tải dữ liệu báo cáo..." : `${reports.length} báo cáo được tìm thấy`}
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-surface-muted/60 text-sm">
            <thead className="bg-surface-muted/30 text-xs uppercase tracking-wide text-surface-foreground/60">
              <tr>
                <th className="px-4 py-3 text-left">Truyện / Chương</th>
                <th className="px-4 py-3 text-left">Người báo cáo</th>
                <th className="px-4 py-3 text-left">Nội dung</th>
                <th className="px-4 py-3 text-left">Trạng thái</th>
                <th className="px-4 py-3 text-left">Thời gian</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-surface-muted/50">
              {isLoading && (
                <tr>
                  <td colSpan={5} className="px-4 py-6 text-center text-surface-foreground/60">
                    Đang tải báo cáo...
                  </td>
                </tr>
              )}

              {!isLoading && reports.length === 0 && (
                <tr>
                  <td colSpan={5} className="px-4 py-8 text-center text-surface-foreground/60">
                    Không có báo cáo nào phù hợp với bộ lọc hiện tại.
                  </td>
                </tr>
              )}

              {reports.map((report) => (
                <tr key={report.id} className="hover:bg-primary/5">
                  <td className="px-4 py-3">
                    <div className="font-semibold text-primary-foreground">{report.comic_name ?? `Truyện ID ${report.comic_id}`}</div>
                    {report.chapter_id && <div className="text-xs text-surface-foreground/60">Chương: {report.chapter_id}</div>}
                  </td>
                  <td className="px-4 py-3 text-sm text-surface-foreground/70">
                    <div>{report.reporter_name ?? "Ẩn danh"}</div>
                    <div className="text-xs text-surface-foreground/50">{report.reporter_email ?? "Không có email"}</div>
                  </td>
                  <td className="px-4 py-3 text-sm text-surface-foreground/70">
                    <div className="font-semibold text-primary-foreground">{report.reason}</div>
                    {report.comment_content && (
                      <p className="mt-1 text-xs text-surface-foreground/60">Bình luận: {report.comment_content}</p>
                    )}
                  </td>
                  <td className="px-4 py-3">
                    <ReportStatusBadge status={report.status} />
                  </td>
                  <td className="px-4 py-3 text-xs text-surface-foreground/60">
                    Gửi {formatRelativeTime(report.created_at)}
                    <br />Cập nhật {formatRelativeTime(report.updated_at)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </section>
  );
};

export default ConverterReportsPage;
