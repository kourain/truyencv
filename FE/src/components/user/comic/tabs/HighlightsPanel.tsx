"use client";

interface HighlightsPanelProps {
  highlights?: string[];
  isLoading?: boolean;
}

const HighlightsPanel = ({ highlights, isLoading = false }: HighlightsPanelProps) => {
  if (isLoading) {
    return (
      <div className="grid gap-3">
        {Array.from({ length: 4 }).map((_, index) => (
          <div key={index} className="h-12 animate-pulse rounded-2xl bg-surface-muted/40" />
        ))}
      </div>
    );
  }

  if (!highlights?.length) {
    return <p className="text-sm text-surface-foreground/60">Chưa có thông tin nổi bật cho truyện này.</p>;
  }

  return (
    <ul className="grid gap-3">
      {highlights.map((highlight, index) => (
        <li
          key={index}
          className="rounded-2xl border border-primary/40 bg-primary/10 px-4 py-3 text-sm text-primary-foreground"
        >
          {highlight}
        </li>
      ))}
    </ul>
  );
};

export default HighlightsPanel;
