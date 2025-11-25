"use client";

import Image from "next/image";
import Link from "next/link";
import { BookmarkPlus, BookOpen, ListTree, Star } from "lucide-react";
import type { ReactNode } from "react";
import { useState, useEffect } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";

import { formatNumber } from "@helpers/format";
import { createBookmark, removeBookmark, checkBookmarkStatus } from "@services/user/bookmark.service";
import { useToast } from "@components/providers/ToastProvider";

interface HeroSectionProps {
  comic?: ComicDetailResponse["comic"];
  slug?: string;
  isLoading?: boolean;
}

const HeroSection = ({ comic, slug, isLoading = false }: HeroSectionProps) => {
  const router = useRouter();
  const { pushToast } = useToast();
  const queryClient = useQueryClient();
  const [isBookmarked, setIsBookmarked] = useState(false);
  const [isCheckingBookmark, setIsCheckingBookmark] = useState(false);

  // Check bookmark status when comic loads
  useEffect(() => {
    if (!comic?.id) return;
    setIsCheckingBookmark(true);
    checkBookmarkStatus(comic.id)
      .then((response) => setIsBookmarked(response.is_bookmarked))
      .catch(() => { })
      .finally(() => setIsCheckingBookmark(false));
  }, [comic?.id]);

  const bookmarkMutation = useMutation({
    mutationFn: async () => {
      if (!comic?.id) throw new Error("Comic ID not found");
      if (isBookmarked) {
        return removeBookmark(comic.id);
      } else {
        return createBookmark(comic.id.toString());
      }
    },
    onSuccess: () => {
      setIsBookmarked(!isBookmarked);
      queryClient.invalidateQueries({ queryKey: ["user-comic-detail", slug] });
      pushToast({
        title: isBookmarked ? "Đã xóa đánh dấu" : "Đã đánh dấu",
        description: isBookmarked ? "Truyện đã được xóa khỏi danh sách đánh dấu" : "Truyện đã được thêm vào danh sách đánh dấu",
        variant: "success"
      });
    },
    onError: (error: any) => {
      pushToast({
        title: "Thao tác thất bại",
        description: error.response?.data?.message || "Vui lòng thử lại sau",
        variant: "error"
      });
    }
  });

  const handleReadContinue = () => {
    if (!slug) return;

    // Navigate to last read chapter or first chapter
    const chapterNumber = comic?.user_last_read_chapter || 1;
    router.push(`/user/comic/${slug}/chapter/${chapterNumber}`);
  };

  const handleBookmark = () => {
    bookmarkMutation.mutate();
  };

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

  const chaptersHref = slug ? `/user/comic/${slug}/chapters` : null;
  const ratingDescriptor = `${comic?.rate.toFixed(1)}/5 (${formatNumber(comic?.rate_count || 0)} đánh giá)`;

  return (
    <section className="grid gap-8 rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg lg:grid-cols-[320px_1fr] lg:p-8">
      <div className="relative h-80 w-full overflow-hidden rounded-2xl border border-surface-muted/60">
        <Image
          src={comic?.cover_url ?? "https://picsum.photos/seed/fallback-cover/320/480"}
          alt={comic?.title ?? "Comic Cover"}
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
          <h1 className="text-3xl font-bold text-primary-foreground lg:text-4xl">{comic?.title}</h1>
          <div className="flex flex-wrap items-center gap-3 text-sm text-surface-foreground/70">
            <span>Tác giả: <span className="font-semibold text-primary-foreground">{comic?.author_name}</span></span>
            <span className="hidden sm:inline">•</span>
            <span>{ratingDescriptor}</span>
            {comic?.user_last_read_chapter && (
              <>
                <span className="hidden sm:inline">•</span>
                <span>Đang đọc đến chương {comic?.user_last_read_chapter}</span>
              </>
            )}
          </div>
        </div>

        <div className="flex flex-wrap gap-2 text-xs text-primary-foreground">
          {comic?.categories.map((category) => (
            <span
              key={category.id}
              className="inline-flex items-center gap-2 rounded-full border border-primary/40 bg-primary/10 px-4 py-2 font-semibold uppercase tracking-wide"
            >
              #{category.name}
            </span>
          ))}
        </div>

        <p className="line-clamp-4 text-sm leading-relaxed text-surface-foreground/80 lg:text-base">
          {comic?.synopsis}
        </p>

        <div className="grid gap-3 sm:grid-cols-3">
          <StatCard label="Chương / tuần" value={`${comic?.weekly_chapter_count}`} icon={<BookOpen className="h-4 w-4" />} />
          <StatCard label="Đề cử tháng" value={formatNumber(comic?.monthly_recommendations ?? 0)} icon={<Star className="h-4 w-4" />} />
          <StatCard label="Đánh dấu" value={formatNumber(comic?.bookmark_count ?? 0)} icon={<BookmarkPlus className="h-4 w-4" />} />
        </div>

        <div className="mt-auto grid gap-3 sm:grid-cols-3">
          <button
            type="button"
            onClick={handleReadContinue}
            className="flex items-center justify-center gap-2 rounded-full bg-primary px-5 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90"
          >
            <BookOpen className="h-4 w-4" />
            {comic?.user_last_read_chapter ? `Đọc tiếp (Ch.${comic.user_last_read_chapter})` : "Đọc từ đầu"}
          </button>
          <button
            type="button"
            onClick={handleBookmark}
            disabled={bookmarkMutation.isPending || isCheckingBookmark}
            className={`flex items-center justify-center gap-2 rounded-full px-5 py-3 text-sm font-semibold transition ${isBookmarked
              ? "bg-primary text-primary-foreground hover:bg-primary/90"
              : "border border-primary/50 bg-primary/10 text-primary hover:bg-primary/20"
              } disabled:opacity-50 disabled:cursor-not-allowed`}
          >
            <BookmarkPlus className="h-4 w-4" />
            {bookmarkMutation.isPending ? "Đang xử lý..." : isBookmarked ? "Đã đánh dấu" : "Đánh dấu"}
          </button>
          {chaptersHref ? (
            <Link
              href={chaptersHref}
              className="flex items-center justify-center gap-2 rounded-full border border-surface-muted/60 bg-surface px-5 py-3 text-sm font-semibold text-primary-foreground transition hover:border-primary/50"
            >
              <ListTree className="h-4 w-4" />
              Mục lục
            </Link>
          ) : (
            <button
              type="button"
              className="flex items-center justify-center gap-2 rounded-full border border-surface-muted/60 bg-surface px-5 py-3 text-sm font-semibold text-primary-foreground opacity-60"
              disabled
            >
              <ListTree className="h-4 w-4" />
              Mục lục
            </button>
          )}
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
