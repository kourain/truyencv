"use client";

import { RefreshCcw } from "lucide-react";

import { HistoryStatus } from "@const/enum/history-status";
import { formatNumber, formatRelativeTime } from "@helpers/format";
import { useConverterCoinHistoryQuery } from "@services/converter";

const HISTORY_LABEL: Record<HistoryStatus, string> = {
  [HistoryStatus.Add]: "Được cộng",
  [HistoryStatus.Use]: "Đã sử dụng",
};

const HISTORY_STYLE: Record<HistoryStatus, string> = {
  [HistoryStatus.Add]: "text-emerald-600",
  [HistoryStatus.Use]: "text-rose-600",
};

const ConverterCoinHistoryPage = () => {
  const { data, isLoading, isFetching, refetch } = useConverterCoinHistoryQuery();
  const histories = data ?? [];
  const balanceDelta = histories.reduce((sum, record) => {
    const delta = record.status === HistoryStatus.Add ? record.coin : -record.coin;
    return sum + delta;
  }, 0);

  return (
    <section className="space-y-6">
      <header className="rounded-2xl border border-surface-muted/60 bg-surface p-4 shadow-sm">
        <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div>
            <p className="text-xs uppercase tracking-wide text-primary">Ví xu</p>
            <h1 className="text-xl font-semibold text-primary-foreground">Lịch sử sử dụng xu</h1>
            <p className="text-sm text-surface-foreground/70">Theo dõi toàn bộ biến động xu khi đổi doanh thu hoặc chi tiêu.</p>
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
      </header>

      <div className="grid gap-4 md:grid-cols-2">
        <div className="rounded-2xl border border-surface-muted/60 bg-surface p-4 shadow-sm">
          <p className="text-sm text-surface-foreground/60">Số lần giao dịch</p>
          <p className="text-3xl font-semibold text-primary-foreground">{histories.length}</p>
        </div>
        <div className="rounded-2xl border border-surface-muted/60 bg-surface p-4 shadow-sm">
          <p className="text-sm text-surface-foreground/60">Tổng biến động</p>
          <p className={`text-3xl font-semibold ${balanceDelta >= 0 ? "text-emerald-500" : "text-rose-500"}`}>
            {balanceDelta >= 0 ? "+" : "-"}
            {formatNumber(Math.abs(balanceDelta))} xu
          </p>
        </div>
      </div>

      <div className="rounded-2xl border border-surface-muted/60 bg-surface shadow-sm">
        <div className="border-b border-surface-muted/60 px-4 py-3 text-sm text-surface-foreground/70">
          {isFetching ? "Đang tải lịch sử xu..." : `${histories.length} dòng được hiển thị`}
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-surface-muted/60 text-sm">
            <thead className="bg-surface-muted/30 text-xs uppercase tracking-wide text-surface-foreground/60">
              <tr>
                <th className="px-4 py-3 text-left">Hành động</th>
                <th className="px-4 py-3 text-left">Số xu</th>
                <th className="px-4 py-3 text-left">Lý do</th>
                <th className="px-4 py-3 text-left">Thời gian</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-surface-muted/50">
              {isLoading && (
                <tr>
                  <td colSpan={4} className="px-4 py-6 text-center text-surface-foreground/60">
                    Đang tải dữ liệu...
                  </td>
                </tr>
              )}

              {!isLoading && histories.length === 0 && (
                <tr>
                  <td colSpan={4} className="px-4 py-8 text-center text-surface-foreground/60">
                    Bạn chưa có lịch sử xu nào.
                  </td>
                </tr>
              )}

              {histories.map((record) => (
                <tr key={record.id} className="hover:bg-primary/5">
                  <td className="px-4 py-3 text-sm font-semibold text-primary-foreground">
                    {HISTORY_LABEL[record.status]}
                  </td>
                  <td className={`px-4 py-3 text-sm font-semibold ${HISTORY_STYLE[record.status]}`}>
                    {record.status === HistoryStatus.Add ? "+" : "-"}
                    {formatNumber(record.coin)} xu
                  </td>
                  <td className="px-4 py-3 text-sm text-surface-foreground/70">
                    {record.reason ?? "Không có ghi chú"}
                  </td>
                  <td className="px-4 py-3 text-xs text-surface-foreground/60">{formatRelativeTime(record.created_at)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </section>
  );
};

export default ConverterCoinHistoryPage;
