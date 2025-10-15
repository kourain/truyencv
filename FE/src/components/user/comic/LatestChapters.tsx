"use client";

import Link from "next/link";
import { ChevronRight } from "lucide-react";

import { formatRelativeTime } from "@helpers/format";

interface LatestChaptersProps {
  chapters?: ComicDetailChapter[];
  slug: string;
  isLoading?: boolean;
}

const LatestChapters = ({ chapters, slug, isLoading = false }: LatestChaptersProps) => {
  if (isLoading) {
    return (
      <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
        <header className="mb-4 flex items-center justify-between">
          <div className="h-6 w-40 animate-pulse rounded bg-surface-muted/60" />
          <div className="h-6 w-28 animate-pulse rounded bg-surface-muted/60" />
        </header>
        <div className="grid gap-3">
          {Array.from({ length: 4 }).map((_, index) => (
            <div key={index} className="h-14 animate-pulse rounded-2xl bg-surface-muted/40" />
          ))}
        </div>
      </section>
    );
  }

  if (!chapters?.length) {
    return null;
  }

  return (
    <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
      <header className="mb-6 flex flex-wrap items-center justify-between gap-4">
        <div>
          <p className="text-xs uppercase tracking-[0.3em] text-primary-foreground/60">Chương mới</p>
          <h2 className="text-xl font-semibold text-primary-foreground">Các chương cập nhật gần nhất</h2>
        </div>
        <Link
          href={`/user/comic/${slug}/chapters`}
          className="inline-flex items-center gap-2 rounded-full border border-primary/50 px-4 py-2 text-sm font-semibold text-primary transition hover:bg-primary/10"
        >
          Xem tất cả
          <ChevronRight className="h-4 w-4" />
        </Link>
      </header>
      <div className="grid gap-3">
        {chapters.slice(0, 4).map((chapter) => (
          <Link
            key={chapter.id}
            href={`/user/comic/${slug}/chapter/${chapter.number}`}
            className="flex items-center justify-between gap-4 rounded-2xl border border-surface-muted/60 bg-surface px-4 py-4 transition hover:border-primary/40 hover:bg-primary/5"
          >
            <div className="flex flex-col">
              <span className="text-sm font-semibold text-primary-foreground">Chương {chapter.number}</span>
              <span className="text-sm text-surface-foreground/70">{chapter.title}</span>
            </div>
            <span className="text-xs font-medium uppercase tracking-wide text-surface-foreground/60">
              {formatRelativeTime(chapter.released_at)}
            </span>
          </Link>
        ))}
      </div>
    </section>
  );
};

export default LatestChapters;
