import { Activity, AlertCircle, BookOpen, ShieldCheck, Users } from "lucide-react";

import { formatNumber } from "@helpers/format";
import type { OverviewStats } from "@components/admin/types";

type ServiceStatusSectionProps = {
  overview: OverviewStats;
  pingData: { message?: string } | null | undefined;
  isPingLoading: boolean;
  isPingError: boolean;
};

const ServiceStatusSection = ({ overview, pingData, isPingLoading, isPingError }: ServiceStatusSectionProps) => (
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
          <div>
            <p className="text-sm font-semibold text-primary-foreground">API quản trị</p>
            <p className="text-xs text-surface-foreground/60">
              {isPingLoading ? "Đang kiểm tra kết nối..." : isPingError ? "Không phản hồi" : pingData?.message ?? "Hoạt động ổn định"}
            </p>
          </div>
        </div>
        <span
          className={`text-xs font-semibold uppercase tracking-wide ${
            isPingError ? "text-red-400" : "text-emerald-400"
          }`}
        >
          {isPingError ? "Gián đoạn" : "Đang chạy"}
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

export default ServiceStatusSection;
