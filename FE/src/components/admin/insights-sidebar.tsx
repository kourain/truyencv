import { Sparkles, Users } from "lucide-react";

import { formatNumber, formatRelativeTime } from "@helpers/format";
import type { OverviewStats } from "@components/admin/types";

type InsightsSidebarProps = {
  overview: OverviewStats;
  recentUsers: UserResponse[];
  showLoading: boolean;
};

const InsightsSidebar = ({ overview, recentUsers, showLoading }: InsightsSidebarProps) => (
  <div className="space-y-6">
    <article className="rounded-2xl border border-primary/40 bg-primary/10 p-6">
      <header className="flex items-center gap-3">
        <Sparkles className="h-5 w-5 text-primary" />
        <div>
          <h3 className="text-lg font-semibold text-primary-foreground">Gợi ý hành động</h3>
          <p className="text-xs text-primary/80">Tối ưu dựa trên dữ liệu mới nhất</p>
        </div>
      </header>
      <ul className="mt-4 space-y-3 text-sm text-surface-foreground/80">
        <li>
          • Ưu tiên cập nhật thêm chương cho {overview.continuing > 0 ? `${formatNumber(overview.continuing)} truyện đang phát hành.` : "các truyện mới được thêm."}
        </li>
        <li>• Xem lại chất lượng nội dung của nhóm truyện có điểm đánh giá dưới 3.5.</li>
        <li>
          • Duyệt phân quyền cho {overview.newUsers > 0 ? `${formatNumber(overview.newUsers)} người dùng mới` : "người dùng mới"} trong 7 ngày gần nhất.
        </li>
      </ul>
    </article>

    <article className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
      <header className="flex items-center gap-2 text-primary-foreground">
        <Users className="h-5 w-5" />
        <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Người dùng mới nhất</h3>
      </header>
      <div className="mt-4 space-y-3">
        {showLoading && (
          <div className="space-y-3">
            {Array.from({ length: 4 }).map((_, index) => (
              <div key={index} className="h-16 animate-pulse rounded-xl bg-surface-muted/40" />
            ))}
          </div>
        )}

        {!showLoading && recentUsers.length === 0 && (
          <div className="rounded-xl border border-dashed border-surface-muted/60 bg-surface px-4 py-6 text-center text-sm text-surface-foreground/60">
            Chưa có người dùng mới trong danh sách hiển thị.
          </div>
        )}

        {!showLoading &&
          recentUsers.map((user) => (
            <div
              key={user.id}
              className="flex flex-wrap items-center justify-between gap-3 rounded-xl border border-surface-muted/70 bg-surface px-4 py-3"
            >
              <div>
                <p className="text-sm font-semibold text-primary-foreground">{user.full_name || user.name}</p>
                <p className="text-xs text-surface-foreground/60">{user.email}</p>
              </div>
              <p className="text-xs text-surface-foreground/60">{formatRelativeTime(user.created_at)}</p>
            </div>
          ))}
      </div>
    </article>
  </div>
);

export default InsightsSidebar;
