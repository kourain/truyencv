"use client";

import { Star } from "lucide-react";

import { formatRelativeTime } from "@helpers/format";

interface ReviewsPanelProps {
  reviews?: ComicDetailReview[];
  isLoading?: boolean;
}

const ReviewsPanel = ({ reviews, isLoading = false }: ReviewsPanelProps) => {
  if (isLoading) {
    return (
      <div className="grid gap-4">
        {Array.from({ length: 3 }).map((_, index) => (
          <div key={index} className="h-32 animate-pulse rounded-3xl bg-surface-muted/40" />
        ))}
      </div>
    );
  }

  if (!reviews?.length) {
    return <p className="text-sm text-surface-foreground/60">Chưa có đánh giá nào cho truyện này.</p>;
  }

  return (
    <div className="grid gap-4">
      {reviews.map((review) => (
        <article
          key={review.id}
          className="flex flex-col gap-3 rounded-3xl border border-surface-muted/60 bg-surface px-5 py-4"
        >
          <header className="flex flex-wrap items-center justify-between gap-3">
            <div className="flex items-center gap-2 text-sm font-semibold text-primary-foreground">
              <span className="inline-flex h-8 w-8 items-center justify-center rounded-full bg-primary/20 text-primary">
                {review.user_display_name.slice(0, 1).toUpperCase()}
              </span>
              <div className="flex flex-col leading-tight">
                <span>{review.user_display_name}</span>
                <span className="text-xs uppercase tracking-wide text-surface-foreground/60">
                  {formatRelativeTime(review.created_at)}
                </span>
              </div>
            </div>
            <div className="flex items-center gap-1 rounded-full bg-primary/10 px-3 py-1 text-sm font-semibold text-primary">
              <Star className="h-4 w-4" />
              {review.rating.toFixed(1)}
            </div>
          </header>
          <p className="text-sm leading-relaxed text-surface-foreground/80">{review.comment}</p>
        </article>
      ))}
    </div>
  );
};

export default ReviewsPanel;
