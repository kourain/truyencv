"use client";

import Link from "next/link";
import { useMemo, useState } from "react";
import { Info } from "lucide-react";
import { useMutation, UseMutationOptions, useQueryClient } from "@tanstack/react-query";
import { useParams } from "next/navigation";

import AdsBanner from "@components/user/comic/AdsBanner";
import AuthorOtherWorks from "@components/user/comic/AuthorOtherWorks";
import { useUserComicChapterQuery } from "@services/user/comic-chapter.service";
import { useUserComicDetailQuery } from "@services/user/comic-detail.service";
import { useToast } from "@components/providers/ToastProvider";
import DiscussionsPanel from "../tabs/DiscussionsPanel";
import { ApiError } from "@helpers/httpClient";
import { recommendComic } from "@services/user/comic-recommend.service";
import { useUnlockComicChapterMutation } from "@services/user/comic-unlock.service";

const tooltipContent = "Mỗi lượt đề cử tiêu tốn 10 coin và bạn chỉ có thể đề cử một lần mỗi tháng.";

const useRecommendComicMutation = (
  options?: UseMutationOptions<ComicRecommendResponse, ApiError, string | number>,
) => {
  return useMutation<ComicRecommendResponse, ApiError, string | number>({
    mutationFn: (comicId: string | number) => recommendComic(comicId),
    ...options,
  });
};

const ComicChapterPage = () => {
  const params = useParams<{ slug: string; id: string }>();
  const { pushToast } = useToast();
  const [isConfirmOpen, setIsConfirmOpen] = useState(false);
  const [isUnlockConfirmOpen, setIsUnlockConfirmOpen] = useState(false);
  const queryClient = useQueryClient();

  const slug = useMemo(() => params?.slug ?? "", [params]);
  const chapterNumber = useMemo(() => {
    if (!params?.id) return null;
    const parsed = Number(params.id);
    return Number.isFinite(parsed) && parsed > 0 ? parsed : null;
  }, [params]);

  const { data: detailData, isLoading: isDetailLoading } = useUserComicDetailQuery(slug);
  const {
    data: chapterData,
    isLoading: isChapterLoading,
    error: chapterError,
    refetch: refetchChapter,
  } = useUserComicChapterQuery(slug, chapterNumber ?? 0);

  const comicId = detailData?.comic?.id ?? chapterData?.comic_id;
  const recommendMutation = useRecommendComicMutation();
  const unlockMutation = useUnlockComicChapterMutation();

  const chapterFallbackMeta = useMemo(() => {
    if (!detailData || !chapterNumber) {
      return undefined;
    }
    return detailData.latest_chapters?.find((item) => item.number === chapterNumber);
  }, [detailData, chapterNumber]);

  const chapterErrorMessage = chapterError?.response?.data?.message ?? "";
  const normalizedErrorMessage = chapterErrorMessage.toLowerCase();
  const isChapterLocked = Boolean(
    chapterError?.response?.status === 400 && normalizedErrorMessage.includes("mở khóa")
  );
  const lockMessage = chapterErrorMessage || "Chương này cần được mở khóa trước khi đọc.";
  const displayChapterTitle = chapterData?.chapter_title ?? chapterFallbackMeta?.title ?? "Đang cập nhật";
  const chapterIdForUnlock = chapterData?.chapter_id ?? chapterFallbackMeta?.id ?? null;

  const unlockPayload = useMemo(() => {
    if (!comicId || !chapterIdForUnlock) {
      return null;
    }
    return {
      comic_id: comicId,
      comic_chapter_id: chapterIdForUnlock,
    };
  }, [comicId, chapterIdForUnlock]);

  const hasRecommended = chapterData?.has_recommended ?? false;
  const isMutationPending = recommendMutation.isPending;
  const isRecommendDisabled = hasRecommended || isChapterLoading || isMutationPending || !comicId;

  const recommendLabel = isChapterLoading
    ? "Đang tải..."
    : isMutationPending
      ? "Đang đề cử..."
      : hasRecommended
        ? "Đã đề cử"
        : "Đề cử (+1)";

  const closeConfirmDialog = () => setIsConfirmOpen(false);
  const openConfirmDialog = () => {
    if (isRecommendDisabled) {
      return;
    }

    setIsConfirmOpen(true);
  };

  const openUnlockDialog = () => {
    if (!unlockPayload) {
      pushToast({
        title: "Không thể mở khóa",
        description: "Thiếu thông tin chương để mở khóa. Vui lòng thử lại sau.",
        variant: "error",
      });
      return;
    }

    setIsUnlockConfirmOpen(true);
  };

  const closeUnlockDialog = () => setIsUnlockConfirmOpen(false);

  const confirmRecommend = () => {
    if (!comicId || recommendMutation.isPending) {
      return;
    }

    recommendMutation.mutate(comicId, {
      onSuccess: (data) => {
        queryClient.setQueryData<ComicChapterReadResponse | undefined>(
          ["user-comic-chapter", slug, chapterNumber ?? 0],
          (previous) => {
            if (!previous) {
              return previous;
            }

            return {
              ...previous,
              has_recommended: true,
              monthly_recommendations: data.rcm_count,
            };
          },
        );
        pushToast({
          title: "Đề cử thành công",
          description: "Bạn đã đề cử truyện thành công. Cảm ơn vì đã ủng hộ tác giả!",
          variant: "success",
        });
        closeConfirmDialog();
      },
      onError: (error) => {
        const message = error.response?.data?.message ?? "Không thể đề cử vào lúc này. Vui lòng thử lại sau.";
        pushToast({
          title: "Đề cử thất bại",
          description: message,
          variant: "error",
        });
        closeConfirmDialog();
      },
    });
  };

  const confirmUnlock = () => {
    if (!unlockPayload || unlockMutation.isPending) {
      return;
    }

    unlockMutation.mutate(unlockPayload, {
      onSuccess: () => {
        pushToast({
          title: "Đã mở khóa chương",
          description: "Bạn có thể tiếp tục đọc chương này.",
          variant: "success",
        });
        closeUnlockDialog();
        queryClient.invalidateQueries({ queryKey: ["user-comic-detail", slug] });
        refetchChapter();
      },
      onError: (error) => {
        const message = error.response?.data?.message ?? "Không thể mở khóa chương. Vui lòng thử lại.";
        pushToast({
          title: "Mở khóa thất bại",
          description: message,
          variant: "error",
        });
      },
    });
  };

  if (!slug || !chapterNumber) {
    return null;
  }

  const isLoading = isDetailLoading || isChapterLoading;
  const comicTitle = chapterData?.comic_title ?? detailData?.comic.title ?? "";
  const authorName = chapterData?.author_name ?? detailData?.comic.author_name ?? "";
  const content = chapterData?.content ?? "";
  const previousHref = chapterData?.previous_chapter_number
    ? `/user/comic/${slug}/chap-${chapterData.previous_chapter_number}`
    : undefined;
  const nextHref = chapterData?.next_chapter_number
    ? `/user/comic/${slug}/chap-${chapterData.next_chapter_number}`
    : undefined;

  const recommendedTitle = chapterData?.recommended_comic_title ?? detailData?.related_by_author?.[0]?.title;
  const recommendedSlug = chapterData?.recommended_comic_slug ?? detailData?.related_by_author?.[0]?.slug;

  const advertisementPrimary = detailData?.advertisements.primary;
  const advertisementSecondary = detailData?.advertisements.secondary;

  return (
    <>
      <main className="mx-auto flex w-full max-w-6xl flex-1 flex-col gap-10 px-6 py-10">
        <AdsBanner
          advertisement={advertisementPrimary}
          variant="primary"
          isLoading={isLoading}
        />

        <section className="rounded-3xl border border-surface-muted/60 bg-surface px-6 py-6 shadow-sm">
          <div className="mb-6 text-center">
            <h1 className="text-2xl font-semibold text-primary-foreground">{comicTitle}</h1>
            <p className="text-sm text-surface-foreground/70">Tác giả: {authorName || "Đang cập nhật"}</p>
          </div>

          <div className="flex flex-wrap items-center justify-center gap-2 text-sm">
            <NavButton disabled={!previousHref} href={previousHref} label="Chương trước" />
            <span className="rounded-full bg-primary px-4 py-1 text-sm font-semibold text-white">
              Chương {chapterNumber}: {displayChapterTitle}
            </span>
            <NavButton disabled={!nextHref} href={nextHref} label="Chương sau" />
            <NavButton href={`/user/comic/${slug}`} label="Mục lục" />
            <button className="rounded-full border border-primary px-4 py-1 text-primary transition hover:bg-primary/10">
              Đánh dấu bookmark
            </button>
          </div>
        </section>

        {isChapterLocked ? (
          <LockedChapterPanel
            chapterNumber={chapterNumber}
            chapterTitle={displayChapterTitle}
            message={lockMessage}
            onUnlock={openUnlockDialog}
            disabled={!unlockPayload}
            isProcessing={unlockMutation.isPending}
          />
        ) : (
          <article className="rounded-3xl border border-surface-muted/60 bg-surface px-6 py-8 text-base leading-7 text-surface-foreground/90 whitespace-pre-wrap">
            {isLoading ? "Đang tải nội dung chương..." : content}
          </article>
        )}

        {recommendedTitle && (
          <section className="rounded-3xl border border-primary/20 bg-primary/5 px-6 py-4 text-center">
            <p className="text-sm font-medium uppercase text-primary">Truyện hay mời đọc</p>
            {recommendedSlug ? (
              <Link
                href={`/user/comic/${recommendedSlug}`}
                className="mt-1 inline-block text-lg font-semibold text-primary hover:underline"
              >
                {recommendedTitle}
              </Link>
            ) : (
              <span className="mt-1 inline-block text-lg font-semibold text-primary">{recommendedTitle}</span>
            )}
          </section>
        )}

        <AuthorOtherWorks
          items={detailData?.related_by_author}
          authorName={detailData?.comic.author_name}
          isLoading={isLoading}
        />

        <AdsBanner
          advertisement={advertisementSecondary}
          variant="secondary"
          isLoading={isLoading}
        />

        <section className="flex flex-wrap items-center justify-center gap-3">
          <NavButton disabled={!previousHref} href={previousHref} label="Chương trước" />
          <button className="rounded-full border border-primary px-5 py-2 text-sm font-medium text-primary transition hover:bg-primary/10">
            Thêm đánh giá
          </button>
          <button className="rounded-full border border-primary px-5 py-2 text-sm font-medium text-primary transition hover:bg-primary/10">
            Tặng quà (coin)
          </button>
          <NavButton href={`/user/comic/${slug}?report=${chapterIdForUnlock ?? ""}`} label="Báo cáo" />
          <div className="group relative flex items-center">
            <button
              type="button"
              onClick={openConfirmDialog}
              disabled={isRecommendDisabled}
              title={tooltipContent}
              className={`rounded-full px-5 py-2 text-sm font-medium transition ${
                hasRecommended
                  ? "bg-surface-muted/60 cursor-not-allowed text-surface-foreground/50"
                  : "bg-primary text-white hover:bg-primary/90"
              }`}
            >
              {recommendLabel}
            </button>
            <Info className="ml-2 h-4 w-4 text-surface-foreground/60" aria-hidden="true" />
            <div className="pointer-events-none absolute left-1/2 top-full z-20 mt-2 hidden w-64 -translate-x-1/2 rounded-xl border border-surface-muted bg-surface px-3 py-2 text-xs leading-relaxed text-surface-foreground shadow-lg transition duration-150 ease-out group-hover:block group-focus-within:block">
              {tooltipContent}
            </div>
          </div>
          <NavButton disabled={!nextHref} href={nextHref} label="Chương sau" />
        </section>

        <DiscussionsPanel discussions={detailData?.discussions} isLoading={isLoading} />
      </main>

      {isConfirmOpen && (
        <div
          className="fixed inset-0 z-[1500] flex items-center justify-center bg-black/60 px-4"
          role="dialog"
          aria-modal="true"
          aria-labelledby="recommend-confirm-title"
          aria-describedby="recommend-confirm-description"
        >
          <div className="w-full max-w-sm rounded-2xl border border-surface-muted bg-surface p-6 text-center shadow-2xl">
            <h2 id="recommend-confirm-title" className="text-lg font-semibold text-primary-foreground">
              Xác nhận đề cử
            </h2>
            <p id="recommend-confirm-description" className="mt-3 text-sm leading-relaxed text-surface-foreground/80">
              {tooltipContent}
            </p>
            <p className="mt-1 text-xs text-surface-foreground/60">Chi phí: 10 coin.</p>
            <div className="mt-6 flex items-center justify-center gap-3">
              <button
                type="button"
                onClick={closeConfirmDialog}
                className="rounded-full border border-surface-muted/70 px-4 py-2 text-sm font-medium text-surface-foreground transition hover:bg-surface-muted/40"
              >
                Hủy
              </button>
              <button
                type="button"
                onClick={confirmRecommend}
                disabled={recommendMutation.isPending}
                className="rounded-full bg-primary px-5 py-2 text-sm font-medium text-white transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
              >
                {recommendMutation.isPending ? "Đang xử lý..." : "Xác nhận"}
              </button>
            </div>
          </div>
        </div>
      )}

      {isUnlockConfirmOpen && (
        <div
          className="fixed inset-0 z-[1500] flex items-center justify-center bg-black/60 px-4"
          role="dialog"
          aria-modal="true"
          aria-labelledby="unlock-confirm-title"
          aria-describedby="unlock-confirm-description"
        >
          <div className="w-full max-w-sm rounded-2xl border border-surface-muted bg-surface p-6 text-center shadow-2xl">
            <h2 id="unlock-confirm-title" className="text-lg font-semibold text-primary-foreground">
              Xác nhận mở khóa
            </h2>
            <p id="unlock-confirm-description" className="mt-3 text-sm leading-relaxed text-surface-foreground/80">
              Việc mở khóa sẽ sử dụng chìa khóa trong tài khoản của bạn. Bạn có chắc chắn muốn tiếp tục?
            </p>
            <div className="mt-6 flex items-center justify-center gap-3">
              <button
                type="button"
                onClick={closeUnlockDialog}
                className="rounded-full border border-surface-muted/70 px-4 py-2 text-sm font-medium text-surface-foreground transition hover:bg-surface-muted/40"
              >
                Hủy
              </button>
              <button
                type="button"
                onClick={confirmUnlock}
                disabled={unlockMutation.isPending}
                className="rounded-full bg-primary px-5 py-2 text-sm font-medium text-white transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
              >
                {unlockMutation.isPending ? "Đang mở khóa..." : "Mở khóa"}
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

type LockedChapterPanelProps = {
  chapterNumber: number | null;
  chapterTitle: string;
  message: string;
  disabled: boolean;
  isProcessing: boolean;
  onUnlock: () => void;
};

const LockedChapterPanel = ({
  chapterNumber,
  chapterTitle,
  message,
  disabled,
  isProcessing,
  onUnlock,
}: LockedChapterPanelProps) => {
  return (
    <section className="flex flex-col items-center gap-4 rounded-3xl border border-primary/40 bg-gradient-to-b from-surface to-surface-muted/40 px-6 py-10 text-center text-surface-foreground">
      <p className="text-xl font-semibold text-primary-foreground">
        Chương {chapterNumber ?? "?"}: {chapterTitle}
      </p>
      <p className="text-sm text-surface-foreground/80">{message}</p>
      <button
        type="button"
        onClick={onUnlock}
        disabled={disabled || isProcessing}
        className="rounded-full bg-primary px-6 py-2 text-sm font-semibold text-white transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/40"
      >
        {isProcessing ? "Đang mở khóa..." : "Mở khóa chương"}
      </button>
    </section>
  );
};

type NavButtonProps = {
  href?: string;
  label: string;
  disabled?: boolean;
};

const NavButton = ({ href, label, disabled = false }: NavButtonProps) => {
  const commonClasses = "rounded-full border border-primary px-5 py-2 text-sm font-medium transition";
  const enabledClasses = "text-primary hover:bg-primary/10";
  const disabledClasses = "cursor-not-allowed border-surface-muted/60 text-surface-foreground/50";

  if (disabled || !href) {
    return <span className={`${commonClasses} ${disabledClasses}`}>{label}</span>;
  }

  return (
    <Link href={href} className={`${commonClasses} ${enabledClasses}`}>
      {label}
    </Link>
  );
};

export default ComicChapterPage;
