"use client";

import Link from "next/link";
import type { ReactNode } from "react";

import { formatNumber } from "@helpers/format";

import EmptyState from "./EmptyState";
import SectionHeader from "./SectionHeader";

interface RankingSectionProps {
  title: string;
  description?: string;
  icon: ReactNode;
  items: RankingComic[];
  isLoading: boolean;
  valueFormatter: (comic: RankingComic) => string;
  emptyMessage?: string;
}

const RankingSection = ({
  title,
  description,
  icon,
  items,
  isLoading,
  valueFormatter,
  emptyMessage,
}: RankingSectionProps) => (
  <div className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-xl">
    <SectionHeader icon={icon} title={title} description={description} />
    <div className="mt-4 space-y-3">
      {isLoading
        ? Array.from({ length: 4 }).map((_, index) => <RankingSkeleton key={index} />)
        : items.map((comic, index) => (
            <Link
              key={comic.comic_id}
              href={`/comic/${comic.comic_slug}`}
              className="flex items-center justify-between rounded-2xl border border-transparent bg-surface-muted/30 px-4 py-3 transition hover:border-primary"
            >
              <div className="flex items-center gap-4">
                <span className="flex h-8 w-8 items-center justify-center rounded-full bg-primary/10 text-sm font-semibold text-primary">
                  {index + 1}
                </span>
                <div className="flex flex-col">
                  <span className="text-sm font-semibold text-primary-foreground">{comic.comic_title}</span>
                  <span className="text-xs text-surface-foreground/60">Tổng lượt xem: {formatNumber(comic.total_views)}</span>
                </div>
              </div>
              <span className="text-xs font-medium text-primary-foreground/80">{valueFormatter(comic)}</span>
            </Link>
          ))}
      {!isLoading && items.length === 0 && <EmptyState message={emptyMessage ?? "Chưa có dữ liệu."} />}
    </div>
  </div>
);

const RankingSkeleton = () => (
  <div className="flex items-center justify-between rounded-2xl border border-transparent bg-surface-muted/40 px-4 py-3">
    <div className="flex items-center gap-4">
      <div className="h-8 w-8 animate-pulse rounded-full bg-surface-muted/80" />
      <div className="h-4 w-32 animate-pulse rounded-full bg-surface-muted/80" />
    </div>
    <div className="h-4 w-20 animate-pulse rounded-full bg-surface-muted/60" />
  </div>
);

export default RankingSection;
