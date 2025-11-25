"use client";

import Image from "next/image";
import Link from "next/link";
import { BookOpen, Trash2, Loader2 } from "lucide-react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useToast } from "@components/providers/ToastProvider";
import {
  getUserBookmarksWithDetails,
  removeBookmark,
} from "@services/user/bookmark.service";
import { useRouter } from "next/navigation";

const BookmarkedComicsSection = () => {
  const { pushToast } = useToast();
  const queryClient = useQueryClient();
  const router = useRouter();

  const { data: bookmarks, isLoading, isError } = useQuery({
    queryKey: ["user-bookmarks-with-details"],
    queryFn: getUserBookmarksWithDetails,
  });

  const removeBookmarkMutation = useMutation({
    mutationFn: (comicId: string) => removeBookmark(comicId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["user-bookmarks-with-details"] });
      pushToast({
        title: "Đã xóa đánh dấu",
        description: "Truyện đã được xóa khỏi danh sách theo dõi",
        variant: "success",
      });
    },
    onError: (error: any) => {
      pushToast({
        title: "Xóa thất bại",
        description: error.response?.data?.message || "Vui lòng thử lại sau",
        variant: "error",
      });
    },
  });

  if (isLoading) {
    return (
      <div className="flex w-full justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (isError) {
    return (
      <div className="rounded-3xl border border-red-500/40 bg-red-500/10 p-6 text-center text-sm text-red-200">
        Không thể tải danh sách truyện đã đánh dấu. Vui lòng thử lại sau.
      </div>
    );
  }

  if (!bookmarks || bookmarks.length === 0) {
    return (
      <div className="rounded-3xl border border-surface-muted/60 bg-surface/70 p-12 text-center">
        <p className="text-xl font-semibold text-primary-foreground">Chưa có truyện theo dõi</p>
        <p className="mt-2 text-sm text-surface-foreground/60">
          Bạn chưa đánh dấu truyện nào. Hãy khám phá và theo dõi những truyện yêu thích!
        </p>
      </div>
    );
  }

  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {bookmarks.map((bookmark) => {
        const readProgress = bookmark.user_last_read_chapter
          ? Math.round((bookmark.user_last_read_chapter / bookmark.latest_chapter_number) * 100)
          : 0;
        const continueChapter = bookmark.user_last_read_chapter || 1;

        return (
          <div
            key={bookmark.id}
            className="group relative flex flex-col gap-3 overflow-hidden rounded-3xl border border-surface-muted/60 bg-surface/80 p-4 shadow-lg transition hover:border-primary/50"
          >
            <Link
              href={`/user/comic/${bookmark.comic_slug}`}
              className="flex gap-3"
            >
              <div className="relative h-32 w-24 flex-shrink-0 overflow-hidden rounded-xl border border-surface-muted/60">
                <Image
                  src={bookmark.comic_cover_url}
                  alt={bookmark.comic_title}
                  fill
                  sizes="96px"
                  className="object-cover transition group-hover:scale-105"
                  unoptimized
                />
              </div>

              <div className="flex min-w-0 flex-1 flex-col gap-2">
                <h3 className="line-clamp-2 text-base font-semibold text-primary-foreground group-hover:text-primary">
                  {bookmark.comic_title}
                </h3>

                <div className="text-xs text-surface-foreground/70">
                  <p>Chương mới nhất: {bookmark.latest_chapter_number}</p>
                  {bookmark.user_last_read_chapter && (
                    <p>Đã đọc đến: Chương {bookmark.user_last_read_chapter}</p>
                  )}
                </div>

                {/* Progress bar */}
                {readProgress > 0 && (
                  <div className="mt-auto">
                    <div className="h-1.5 w-full overflow-hidden rounded-full bg-surface-muted/60">
                      <div
                        className="h-full bg-primary transition-all"
                        style={{ width: `${readProgress}%` }}
                      />
                    </div>
                    <p className="mt-1 text-xs text-surface-foreground/50">
                      {readProgress}% hoàn thành
                    </p>
                  </div>
                )}
              </div>
            </Link>

            <div className="flex gap-2">
              <button
                type="button"
                onClick={() => router.push(`/user/comic/${bookmark.comic_slug}/chapter/${continueChapter}`)}
                className="flex flex-1 items-center justify-center gap-2 rounded-full bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90"
              >
                <BookOpen className="h-4 w-4" />
                Đọc tiếp
              </button>
              <button
                type="button"
                onClick={() => removeBookmarkMutation.mutate(bookmark.comic_id)}
                disabled={removeBookmarkMutation.isPending}
                className="flex items-center justify-center rounded-full border border-rose-500/60 bg-rose-500/10 px-4 py-2 text-rose-300 transition hover:bg-rose-500/20 disabled:opacity-50"
              >
                <Trash2 className="h-4 w-4" />
              </button>
            </div>
          </div>
        );
      })}
    </div>
  );
};

export default BookmarkedComicsSection;
