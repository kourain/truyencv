"use client";

import { formatRelativeTime } from "@helpers/format";

interface DiscussionsPanelProps {
  discussions?: ComicDetailDiscussion[];
  isLoading?: boolean;
}

const DiscussionsPanel = ({ discussions, isLoading = false }: DiscussionsPanelProps) => {
  if (isLoading) {
    return (
      <div className="grid gap-3">
        {Array.from({ length: 4 }).map((_, index) => (
          <div key={index} className="h-20 animate-pulse rounded-3xl bg-surface-muted/40" />
        ))}
      </div>
    );
  }

  if (!discussions?.length) {
    return <p className="text-sm text-surface-foreground/60">Chưa có thảo luận nào cho truyện này.</p>;
  }

  return (
    <div className="grid gap-3">
      {discussions.map((discussion) => (
        <article
          key={discussion.id}
          className="rounded-3xl border border-surface-muted/60 bg-surface px-5 py-4"
        >
          <header className="mb-2 flex items-center justify-between gap-3">
            <div className="flex items-center gap-2 text-sm font-semibold text-primary-foreground">
              <span className="inline-flex h-8 w-8 items-center justify-center rounded-full bg-surface-muted/60 text-primary">
                {discussion.user_display_name.slice(0, 1).toUpperCase()}
              </span>
              <span>{discussion.user_display_name}</span>
            </div>
            <span className="text-xs uppercase tracking-wide text-surface-foreground/60">
              {formatRelativeTime(discussion.created_at)}
            </span>
          </header>
          <p className="text-sm leading-relaxed text-surface-foreground/80">{discussion.message}</p>
        </article>
      ))}
    </div>
  );
};

export default DiscussionsPanel;
