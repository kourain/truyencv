"use client";

import { History } from "lucide-react";

import type { UserHistoryItem } from "@services/user/home.service";
import { formatRelativeTime } from "@helpers/format";

import EmptyState from "./EmptyState";
import SectionHeader from "./SectionHeader";

interface HistorySectionProps {
  items: UserHistoryItem[];
  isLoading: boolean;
}

const HistorySection = ({ items, isLoading }: HistorySectionProps) => (
  <section className="grid gap-4">
    <SectionHeader
      icon={<History className="h-5 w-5 text-primary" />}
      title="Lịch sử đọc gần đây"
      description="Tiếp tục những chương bạn đang theo dõi"
    />

    <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
      {isLoading
        ? Array.from({ length: 3 }).map((_, index) => <HistorySkeleton key={index} />)
        : items.map((history) => <HistoryCard key={history.comic_id} history={history} />)}
      {!isLoading && items.length === 0 && <EmptyState message="Bạn chưa đọc bộ truyện nào gần đây." />}
    </div>
  </section>
);

const HistoryCard = ({ history }: { history: UserHistoryItem }) => (
  <div className="group flex gap-4 rounded-3xl border border-surface-muted/60 bg-surface/80 p-4 shadow-lg transition hover:-translate-y-1 hover:border-primary hover:shadow-2xl">
    <div className="relative h-24 w-20 overflow-hidden rounded-2xl bg-surface-muted/60">
      {history.cover_url ? (
        <img src={history.cover_url} alt={history.comic_title} className="h-full w-full object-cover" loading="lazy" />
      ) : (
        <div className="flex h-full w-full items-center justify-center text-xs text-surface-foreground/60">No cover</div>
      )}
    </div>
    <div className="flex flex-1 flex-col justify-between">
      <div>
        <h3 className="text-sm font-semibold text-primary-foreground">{history.comic_title}</h3>
        <p className="text-xs text-surface-foreground/60">Đọc đến chương {history.last_read_chapter}/{history.total_chapters}</p>
      </div>
      <p className="text-xs text-surface-foreground/50">Cập nhật {formatRelativeTime(history.last_read_at)}</p>
    </div>
  </div>
);

const HistorySkeleton = () => (
  <div className="flex gap-4 rounded-3xl border border-surface-muted/60 bg-surface-muted/40 p-4">
    <div className="h-24 w-20 animate-pulse rounded-2xl bg-surface-muted/80" />
    <div className="flex flex-1 flex-col gap-2">
      <div className="h-4 w-3/4 animate-pulse rounded-full bg-surface-muted/80" />
      <div className="h-3 w-1/2 animate-pulse rounded-full bg-surface-muted/60" />
      <div className="mt-auto h-3 w-1/3 animate-pulse rounded-full bg-surface-muted/60" />
    </div>
  </div>
);

export default HistorySection;
