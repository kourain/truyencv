"use client";

import { ChevronRight } from "lucide-react";

import { formatRelativeTime } from "@helpers/format";
import type { CompletedComic } from "@services/user/home.service";

import EmptyState from "./EmptyState";
import SectionHeader from "./SectionHeader";

interface CompletedCarouselSectionProps {
  items: CompletedComic[];
  isLoading: boolean;
}

const CompletedCarouselSection = ({ items, isLoading }: CompletedCarouselSectionProps) => (
  <section className="grid gap-4">
    <SectionHeader
      icon={<ChevronRight className="h-5 w-5 text-primary" />}
      title="Truyện vừa hoàn thành"
      description="Đọc trọn bộ những tác phẩm đã kết thúc"
    />
    <div className="overflow-hidden">
      <div className="flex gap-4 overflow-x-auto pb-2">
        {isLoading
          ? Array.from({ length: 6 }).map((_, index) => <CompletedSkeleton key={index} />)
          : items.map((comic) => <CompletedCard key={comic.comic_id} comic={comic} />)}
        {!isLoading && items.length === 0 && <EmptyState message="Chưa có bộ truyện nào hoàn thành." />}
      </div>
    </div>
  </section>
);

const CompletedCard = ({ comic }: { comic: CompletedComic }) => (
  <div className="min-w-[220px] rounded-3xl border border-surface-muted/60 bg-surface/80 p-4 shadow-lg transition hover:-translate-y-1 hover:border-primary hover:shadow-2xl">
    <div className="relative h-36 overflow-hidden rounded-2xl bg-surface-muted/60">
      {comic.cover_url ? (
        <img src={comic.cover_url} alt={comic.comic_title} className="h-full w-full object-cover" loading="lazy" />
      ) : (
        <div className="flex h-full w-full items-center justify-center text-xs text-surface-foreground/60">No cover</div>
      )}
    </div>
    <div className="mt-4 space-y-1">
      <h3 className="text-sm font-semibold text-primary-foreground">{comic.comic_title}</h3>
      <p className="text-xs text-surface-foreground/60">{comic.total_chapters} chương</p>
      <p className="text-[11px] text-surface-foreground/50">Hoàn thành {formatRelativeTime(comic.completed_at)}</p>
    </div>
  </div>
);

const CompletedSkeleton = () => (
  <div className="min-w-[220px] rounded-3xl border border-surface-muted/60 bg-surface-muted/40 p-4">
    <div className="h-36 animate-pulse rounded-2xl bg-surface-muted/80" />
    <div className="mt-4 space-y-2">
      <div className="h-4 w-3/4 animate-pulse rounded-full bg-surface-muted/80" />
      <div className="h-3 w-1/2 animate-pulse rounded-full bg-surface-muted/60" />
    </div>
  </div>
);

export default CompletedCarouselSection;
