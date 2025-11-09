"use client";

import Link from "next/link";
import { Star, ThumbsUp } from "lucide-react";

import { formatNumber, formatRelativeTime } from "@helpers/format";

import EmptyState from "./EmptyState";
import SectionHeader from "./SectionHeader";

interface LatestReviewsSectionProps {
  items: ReviewSummary[];
  isLoading: boolean;
}

const LatestReviewsSection = ({ items, isLoading }: LatestReviewsSectionProps) => (
  <section className="grid gap-4">
    <SectionHeader
      icon={<Star className="h-5 w-5 text-primary" />}
      title="Đánh giá mới nhất"
      description="Lắng nghe cộng đồng độc giả nhận xét"
    />
    <div className="grid gap-4 md:grid-cols-2">
      {isLoading
        ? Array.from({ length: 2 }).map((_, index) => <ReviewSkeleton key={index} />)
        : items.map((review) => <ReviewCard key={review.review_id} review={review} />)}
      {!isLoading && items.length === 0 && <EmptyState message="Chưa có đánh giá nào." />}
    </div>
  </section>
);

const ReviewCard = ({ review }: { review: ReviewSummary }) => (
  <article className="flex flex-col gap-3 rounded-3xl border border-surface-muted/60 bg-surface/80 p-4 shadow-lg transition hover:-translate-y-1 hover:border-primary hover:shadow-2xl">
    <header className="flex items-center justify-between">
      <div className="flex items-center gap-3">
        <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-sm font-semibold text-primary">
          {review.user_display_name.slice(0, 2).toUpperCase()}
        </div>
        <div className="flex flex-col">
          <span className="text-sm font-semibold text-primary-foreground">{review.user_display_name}</span>
          <Link href={`/comic/${review.comic_slug}`} className="text-xs text-surface-foreground/60 transition hover:text-primary">
            {review.comic_title}
          </Link>
        </div>
      </div>
      <div className="flex items-center gap-1 text-primary">
        <Star className="h-4 w-4" />
        <span className="text-sm font-semibold">{review.rating.toFixed(1)}</span>
      </div>
    </header>
    {review.content && <p className="text-sm text-surface-foreground/70">{review.content}</p>}
    <footer className="flex items-center justify-between text-xs text-surface-foreground/60">
      <span>{formatRelativeTime(review.created_at)}</span>
      <span className="flex items-center gap-1">
        <ThumbsUp className="h-3.5 w-3.5" /> {formatNumber(review.liked_count)}
      </span>
    </footer>
  </article>
);

const ReviewSkeleton = () => (
  <div className="rounded-3xl border border-surface-muted/60 bg-surface-muted/40 p-4">
    <div className="mb-3 flex items-center gap-3">
      <div className="h-10 w-10 animate-pulse rounded-full bg-surface-muted/80" />
      <div className="h-4 w-32 animate-pulse rounded-full bg-surface-muted/80" />
    </div>
    <div className="space-y-2">
      <div className="h-3 w-full animate-pulse rounded-full bg-surface-muted/60" />
      <div className="h-3 w-2/3 animate-pulse rounded-full bg-surface-muted/60" />
    </div>
    <div className="mt-3 h-3 w-1/4 animate-pulse rounded-full bg-surface-muted/60" />
  </div>
);

export default LatestReviewsSection;
