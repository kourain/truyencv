import { Activity, AlertCircle, BookOpen, ShieldCheck, Users } from "lucide-react";

import { formatNumber } from "@helpers/format";
import type { OverviewStats } from "@components/admin/types";
import type { PingResponse, HealthStatus } from "@services/health.service";

type ServiceStatusSectionProps = {
  overview: OverviewStats;
  pingData: PingResponse | null | undefined;
  isPingLoading: boolean;
  isPingError: boolean;
};

type DisplayStatus = HealthStatus | "unknown";

const statusLabelMap: Record<DisplayStatus, string> = {
  healthy: "Hoạt động ổn định",
  degraded: "Hiệu năng giảm",
  unhealthy: "Không khả dụng",
  unknown: "Chưa xác định"
};

const statusBadgeMap: Record<DisplayStatus, string> = {
  healthy: "text-emerald-400",
  degraded: "text-amber-300",
  unhealthy: "text-red-400",
  unknown: "text-surface-foreground/50"
};

const statusBadgeTextMap: Record<DisplayStatus, string> = {
  healthy: "Đang chạy",
  degraded: "Suy giảm",
  unhealthy: "Gián đoạn",
  unknown: "Chưa rõ"
};

const ServiceStatusSection = ({ overview, pingData, isPingLoading, isPingError }: ServiceStatusSectionProps) => {
  const apiStatus: DisplayStatus = isPingLoading ? "unknown" : isPingError ? "unhealthy" : pingData?.api.status ?? "unknown";
  const apiLatency = !isPingLoading && !isPingError ? pingData?.api.response_time_ms ?? null : null;
  const databaseStatus: DisplayStatus =
    isPingLoading ? "unknown" : isPingError ? "unhealthy" : pingData?.dependencies.database.status ?? "unknown";
  const databaseLatency = !isPingLoading && !isPingError ? pingData?.dependencies.database.latency_ms ?? null : null;
  const databaseError = !isPingLoading && !isPingError ? pingData?.dependencies.database.error ?? null : null;

  const renderApiDescription = () => {
    if (isPingLoading) {
      return "Đang kiểm tra kết nối...";
    }
    if (isPingError) {
      return "Không phản hồi từ API.";
    }
    const latencyText = apiLatency !== null ? `${formatNumber(apiLatency)} ms` : "đang thu thập";
    return `API ${statusLabelMap[apiStatus]} • ${latencyText}`;
  };

  const renderDatabaseDescription = () => {
    if (isPingLoading || isPingError) {
      return null;
    }
    const latencyText = databaseLatency !== null ? `${formatNumber(databaseLatency)} ms` : "không xác định";
    const base = `Cơ sở dữ liệu: ${statusLabelMap[databaseStatus]} • ${latencyText}`;
    return databaseError ? `${base} • ${databaseError}` : base;
  };

  const apiDescription = renderApiDescription();
  const databaseDescription = renderDatabaseDescription();

  return (
    <article className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
      <header className="flex items-center gap-2 text-primary-foreground">
        <ShieldCheck className="h-5 w-5" />
        <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Tình trạng dịch vụ</h3>
      </header>
      <div className="mt-5 space-y-3">
        <div className="flex items-center justify-between rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
          <div className="flex items-center gap-3">
            <span className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-primary">
              <Activity className="h-5 w-5" />
            </span>
            <div className="space-y-1">
              <p className="text-sm font-semibold text-primary-foreground">API quản trị</p>
              <p className="text-xs text-surface-foreground/60">{apiDescription}</p>
              {databaseDescription && (
                <p className="text-[11px] text-surface-foreground/50">{databaseDescription}</p>
              )}
            </div>
          </div>
          <span className={`text-xs font-semibold uppercase tracking-wide ${statusBadgeMap[apiStatus]}`}>
            {statusBadgeTextMap[apiStatus]}
          </span>
        </div>

        <div className="flex items-center justify-between rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
          <div className="flex items-center gap-3">
            <span className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-primary">
              <BookOpen className="h-5 w-5" />
            </span>
            <div>
              <p className="text-sm font-semibold text-primary-foreground">Kho truyện</p>
              <p className="text-xs text-surface-foreground/60">
                {overview.totalComics > 0
                  ? `${formatNumber(overview.totalComics)} truyện đang theo dõi`
                  : "Chưa có dữ liệu truyện"}
              </p>
            </div>
          </div>
          <span className="text-xs font-semibold uppercase tracking-wide text-primary/70">
            {overview.totalComics > 0 ? "Đầy đủ" : "Trống"}
          </span>
        </div>

        <div className="flex items-center justify-between rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
          <div className="flex items-center gap-3">
            <span className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-primary">
              <Users className="h-5 w-5" />
            </span>
            <div>
              <p className="text-sm font-semibold text-primary-foreground">Quản trị viên</p>
              <p className="text-xs text-surface-foreground/60">
                {overview.totalUsers > 0
                  ? `${formatNumber(overview.totalUsers)} tài khoản có quyền truy cập`
                  : "Chưa có ai được phân quyền"}
              </p>
            </div>
          </div>
          <span className="text-xs font-semibold uppercase tracking-wide text-primary/70">
            {overview.totalUsers > 0 ? "Hoạt động" : "Đang chờ"}
          </span>
        </div>

        {isPingError && (
          <div className="flex items-start gap-3 rounded-xl border border-red-500/50 bg-red-500/10 px-4 py-3 text-sm text-red-400">
            <AlertCircle className="mt-0.5 h-4 w-4 flex-shrink-0" />
            Không thể kết nối tới API. Vui lòng kiểm tra lại dịch vụ backend hoặc thử làm mới.
          </div>
        )}
      </div>
    </article>
  );
};

export default ServiceStatusSection;
