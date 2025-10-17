"use client";

import Link from "next/link";
import { useMemo } from "react";
import { useParams } from "next/navigation";

import AdvertisementBanner from "@components/user/comic/AdvertisementBanner";
import AuthorOtherWorks from "@components/user/comic/AuthorOtherWorks";
import { useUserComicChapterQuery } from "@services/user/comic-chapter.service";
import { useUserComicDetailQuery } from "@services/user/comic-detail.service";
import DiscussionsPanel from "../tabs/DiscussionsPanel";

const ComicChapterPage = () => {
  const params = useParams<{ slug: string; id: string }>();

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
  } = useUserComicChapterQuery(slug, chapterNumber ?? 0);

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
    <main className="mx-auto flex w-full max-w-6xl flex-1 flex-col gap-10 px-6 py-10">
      <AdvertisementBanner
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
            Chương {chapterNumber}: {chapterData?.chapter_title ?? "Đang cập nhật"}
          </span>
          <NavButton disabled={!nextHref} href={nextHref} label="Chương sau" />
          <NavButton href={`/user/comic/${slug}`} label="Mục lục" />
          <button className="rounded-full border border-primary px-4 py-1 text-primary transition hover:bg-primary/10">
            Đánh dấu bookmark
          </button>
        </div>
      </section>

      <article className="rounded-3xl border border-surface-muted/60 bg-surface px-6 py-8 text-base leading-7 text-surface-foreground/90 whitespace-pre-wrap">
        {isLoading ? "Đang tải nội dung chương..." : content}
      </article>

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

      <AdvertisementBanner
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
        <NavButton href={`/user/comic/${slug}?report=${chapterData?.chapter_id ?? ""}`} label="Báo cáo" />
        <button className="rounded-full bg-primary px-5 py-2 text-sm font-medium text-white transition hover:bg-primary/90">
          Đề cử (+1)
        </button>
        <NavButton disabled={!nextHref} href={nextHref} label="Chương sau" />
      </section>

      <DiscussionsPanel discussions={detailData?.discussions} isLoading={isLoading} />
    </main>
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
