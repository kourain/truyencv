"use client";

interface EmptyStateProps {
  message: string;
}

const EmptyState = ({ message }: EmptyStateProps) => (
  <div className="rounded-3xl border border-dashed border-surface-muted/60 bg-surface/60 p-6 text-center text-sm text-surface-foreground/60">
    {message}
  </div>
);

export default EmptyState;
