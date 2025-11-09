"use client";

import Link from "next/link";
import { Clock } from "lucide-react";

import { formatRelativeTime } from "@helpers/format";

import EmptyState from "./EmptyState";
import SectionHeader from "./SectionHeader";

interface LatestUpdatesSectionProps {
  items: ComicUpdate[];
  isLoading: boolean;
}

const LatestUpdatesSection = ({ items, isLoading }: LatestUpdatesSectionProps) => (
  <section className="grid gap-4">
    <SectionHeader
      icon={<Clock className="h-5 w-5 text-primary" />}
      title="Vừa lên chương mới"
      description="10 cập nhật mới nhất từ các bộ truyện bạn quan tâm"
    />
    <div className="overflow-hidden rounded-3xl border border-surface-muted/60 bg-surface/80 shadow-xl">
      <table className="w-full border-collapse text-left text-sm">
        <thead className="bg-surface-muted/60 text-xs uppercase tracking-[0.2em] text-surface-foreground/60">
          <tr>
            <th className="px-6 py-4 font-medium">Truyện</th>
            <th className="px-6 py-4 font-medium">Chương</th>
            <th className="px-6 py-4 font-medium">Cập nhật</th>
          </tr>
        </thead>
        <tbody>
          {isLoading
            ? Array.from({ length: 5 }).map((_, index) => <LatestUpdateSkeleton key={index} />)
            : items.map((item) => (
                <tr
                  key={`${item.comic_id}-${item.chapter_number}`}
                  className="border-t border-surface-muted/40 transition hover:bg-surface-muted/30"
                >
                  <td className="px-6 py-4 font-medium text-primary-foreground">
                    <Link href={`/comic/${item.comic_slug}`} className="transition hover:text-primary">
                      {item.comic_title}
                    </Link>
                  </td>
                  <td className="px-6 py-4 text-surface-foreground/80">{item.chapter_title}</td>
                  <td className="px-6 py-4 text-xs text-surface-foreground/60">{formatRelativeTime(item.updated_at)}</td>
                </tr>
              ))}
          {!isLoading && items.length === 0 && (
            <tr>
              <td colSpan={3} className="px-6 py-8">
                <EmptyState message="Chưa có chương mới được cập nhật." />
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  </section>
);

const LatestUpdateSkeleton = () => (
  <tr className="border-t border-surface-muted/40">
    <td className="px-6 py-4">
      <div className="h-3 w-48 animate-pulse rounded-full bg-surface-muted/60" />
    </td>
    <td className="px-6 py-4">
      <div className="h-3 w-40 animate-pulse rounded-full bg-surface-muted/60" />
    </td>
    <td className="px-6 py-4">
      <div className="h-3 w-28 animate-pulse rounded-full bg-surface-muted/60" />
    </td>
  </tr>
);

export default LatestUpdatesSection;
