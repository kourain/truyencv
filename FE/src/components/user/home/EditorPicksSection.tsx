"use client";

import { Sparkles, Star } from "lucide-react";

import type { HighlightedComic } from "@services/user/home.service";

import SectionHeader from "./SectionHeader";
import EmptyState from "./EmptyState";

interface EditorPicksSectionProps {
  items: HighlightedComic[];
  isLoading: boolean;
}

const EditorPicksSection = ({ items, isLoading }: EditorPicksSectionProps) => (
  <section className="grid gap-4">
    <SectionHeader
      icon={<Sparkles className="h-5 w-5 text-primary" />}
      title="Biên tập viên đề cử"
      description="Những lựa chọn đặc sắc nhất trong tuần"
    />

    <div className="grid gap-4 md:grid-cols-3">
      {isLoading
        ? Array.from({ length: 3 }).map((_, index) => <EditorPickSkeleton key={index} />)
        : items.map((comic) => <EditorPickCard key={comic.comic_id} comic={comic} />)}
      {!isLoading && items.length === 0 && <EmptyState message="Danh sách đề cử đang được cập nhật." />}
    </div>
  </section>
);

const EditorPickCard = ({ comic }: { comic: HighlightedComic }) => (
  <div className="group relative overflow-hidden rounded-3xl border border-surface-muted/60 bg-surface/80 p-4 shadow-lg transition hover:-translate-y-1 hover:border-primary hover:shadow-2xl">
    <div className="flex items-start gap-4">
      <div className="relative h-36 w-28 overflow-hidden rounded-2xl bg-surface-muted/60">
        {comic.cover_url ? (
          <img src={comic.cover_url} alt={comic.comic_title} className="h-full w-full object-cover" loading="lazy" />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-xs text-surface-foreground/60">No cover</div>
        )}
      </div>
      <div className="flex flex-1 flex-col gap-2">
        <h3 className="text-base font-semibold text-primary-foreground">{comic.comic_title}</h3>
        <p className="text-xs leading-relaxed text-surface-foreground/70">{comic.short_description}</p>
        <div className="mt-auto flex items-center gap-3 text-xs text-surface-foreground/60">
          {comic.latest_chapter && <span>Chương mới nhất: {comic.latest_chapter}</span>}
          {comic.average_rating && (
            <span className="flex items-center gap-1 text-primary">
              <Star className="h-3 w-3" />
              {comic.average_rating.toFixed(1)}
            </span>
          )}
        </div>
      </div>
    </div>
  </div>
);

const EditorPickSkeleton = () => (
  <div className="flex gap-4 rounded-3xl border border-surface-muted/60 bg-surface-muted/40 p-4">
    <div className="h-36 w-28 animate-pulse rounded-2xl bg-surface-muted/80" />
    <div className="flex flex-1 flex-col gap-3">
      <div className="h-4 w-3/4 animate-pulse rounded-full bg-surface-muted/80" />
      <div className="h-3 w-full animate-pulse rounded-full bg-surface-muted/60" />
      <div className="h-3 w-2/3 animate-pulse rounded-full bg-surface-muted/60" />
      <div className="mt-auto h-3 w-1/4 animate-pulse rounded-full bg-surface-muted/60" />
    </div>
  </div>
);

export default EditorPicksSection;
