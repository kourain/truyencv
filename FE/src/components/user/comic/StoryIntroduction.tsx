"use client";

interface StoryIntroductionProps {
  introduction?: string;
  isLoading?: boolean;
}

const StoryIntroduction = ({ introduction, isLoading = false }: StoryIntroductionProps) => {
  if (isLoading) {
    return (
      <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
        <div className="h-6 w-36 animate-pulse rounded bg-surface-muted/50" />
        <div className="mt-4 grid gap-3">
          {Array.from({ length: 5 }).map((_, index) => (
            <div key={index} className="h-4 w-full animate-pulse rounded bg-surface-muted/40" />
          ))}
        </div>
      </section>
    );
  }

  if (!introduction) {
    return null;
  }

  return (
    <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
      <header className="mb-4 flex flex-col gap-1">
        <p className="text-xs uppercase tracking-[0.3em] text-primary-foreground/60">Giới thiệu truyện</p>
        <h2 className="text-xl font-semibold text-primary-foreground">Tóm tắt nội dung</h2>
      </header>
      <p className="whitespace-pre-line text-sm leading-relaxed text-surface-foreground/80 lg:text-base">
        {introduction}
      </p>
    </section>
  );
};

export default StoryIntroduction;
