"use client";

import Image from "next/image";
import { BookmarkPlus, BookOpen, ListTree, Star } from "lucide-react";
import type { ReactNode } from "react";

import { formatNumber } from "@helpers/format";

interface HeroSectionProps {
  comic?: ComicDetailResponse["comic"];
  isLoading?: boolean;
}

const HeroSection = ({ comic, isLoading = false }: HeroSectionProps) => {
  if (isLoading) {
    return (
      <section className="grid gap-8 rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg lg:grid-cols-[320px_1fr] lg:p-8">
        <div className="h-80 w-full animate-pulse rounded-2xl bg-surface-muted/60" />
        <div className="flex flex-col gap-4">
          <div className="h-8 w-2/3 animate-pulse rounded bg-surface-muted/60" />
          <div className="h-6 w-1/2 animate-pulse rounded bg-surface-muted/60" />
          <div className="flex flex-wrap gap-3 pt-2">
            {Array.from({ length: 4 }).map((_, index) => (
              <div key={index} className="h-9 w-28 animate-pulse rounded-full bg-surface-muted/50" />
            ))}
          </div>
          <div className="grid gap-3 sm:grid-cols-3">
            {Array.from({ length: 3 }).map((_, index) => (
              <div key={index} className="h-20 animate-pulse rounded-2xl border border-surface-muted/50 bg-surface-muted/30" />
            ))}
          </div>
          <div className="mt-auto grid gap-3 sm:grid-cols-3">
            {Array.from({ length: 3 }).map((_, index) => (
              <div key={index} className="h-12 animate-pulse rounded-full bg-primary/20" />
            ))}
          </div>
        </div>
      </section>
    );
  }

  // if (!comic) {
  //   return null;
  // }
  console.log(comic);
  const ratingDescriptor = `${comic.rate.toFixed(1)}/5 (${formatNumber(comic.rate_count)} đánh giá)`;

  return (
    <section className="grid gap-8 rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg lg:grid-cols-[320px_1fr] lg:p-8">
      <div className="relative h-80 w-full overflow-hidden rounded-2xl border border-surface-muted/60">
        <Image
          src={comic.cover_url ?? "https://picsum.photos/seed/fallback-cover/320/480"}
          alt={comic.title}
          fill
          sizes="320px"
          className="object-cover"
          priority
          unoptimized
        />
      </div>

      <div className="flex flex-col gap-6">
        <div className="flex flex-col gap-3">
          <p className="text-sm uppercase tracking-[0.35em] text-primary-foreground/60">Truyện hiện tại</p>
          <h1 className="text-3xl font-bold text-primary-foreground lg:text-4xl">{comic.title}</h1>
          <div className="flex flex-wrap items-center gap-3 text-sm text-surface-foreground/70">
            <span>Tác giả: <span className="font-semibold text-primary-foreground">{comic.author_name}</span></span>
            <span className="hidden sm:inline">•</span>
            <span>{ratingDescriptor}</span>
            {comic.user_last_read_chapter && (
              <>
                <span className="hidden sm:inline">•</span>
                <span>Đang đọc đến chương {comic.user_last_read_chapter}</span>
              </>
            )}
          </div>
        </div>

        <div className="flex flex-wrap gap-2 text-xs text-primary-foreground">
          {comic.categories.map((category) => (
            <span
              key={category.id}
              className="inline-flex items-center gap-2 rounded-full border border-primary/40 bg-primary/10 px-4 py-2 font-semibold uppercase tracking-wide"
            >
              #{category.name}
            </span>
          ))}
        </div>

        <p className="line-clamp-4 text-sm leading-relaxed text-surface-foreground/80 lg:text-base">
          {comic.synopsis}
        </p>

        <div className="grid gap-3 sm:grid-cols-3">
          <StatCard label="Chương / tuần" value={`${comic.weekly_chapter_count}`} icon={<BookOpen className="h-4 w-4" />} />
          <StatCard label="Đề cử tuần" value={formatNumber(comic.weekly_recommendations)} icon={<Star className="h-4 w-4" />} />
          <StatCard label="Đánh dấu" value={formatNumber(comic.bookmark_count)} icon={<BookmarkPlus className="h-4 w-4" />} />
        </div>

        <div className="mt-auto grid gap-3 sm:grid-cols-3">
          <button
            type="button"
            className="flex items-center justify-center gap-2 rounded-full bg-primary px-5 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90"
          >
            <BookOpen className="h-4 w-4" />
            Đọc tiếp
          </button>
          <button
            type="button"
            className="flex items-center justify-center gap-2 rounded-full border border-primary/50 bg-primary/10 px-5 py-3 text-sm font-semibold text-primary transition hover:bg-primary/20"
          >
            <BookmarkPlus className="h-4 w-4" />
            Đánh dấu
          </button>
          <button
            type="button"
            className="flex items-center justify-center gap-2 rounded-full border border-surface-muted/60 bg-surface px-5 py-3 text-sm font-semibold text-primary-foreground transition hover:border-primary/50"
          >
            <ListTree className="h-4 w-4" />
            Mục lục
          </button>
        </div>
      </div>
    </section>
  );
};

interface StatCardProps {
  label: string;
  value: string;
  icon: ReactNode;
}

const StatCard = ({ label, value, icon }: StatCardProps) => {
  return (
    <div className="flex h-full items-center gap-3 rounded-2xl border border-surface-muted/60 bg-surface px-4 py-3">
      <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/20 text-primary">{icon}</div>
      <div className="flex flex-col">
        <span className="text-xs uppercase tracking-[0.3em] text-surface-foreground/60">{label}</span>
        <span className="text-lg font-semibold text-primary-foreground">{value}</span>
      </div>
    </div>
  );
};

export default HeroSection;
