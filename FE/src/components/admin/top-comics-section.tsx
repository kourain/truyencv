import { useMemo } from "react";
import { ArrowRight } from "lucide-react";

import { ComicStatus } from "@const/comic-status";
import { formatNumber, formatRelativeTime } from "@helpers/format";
import type { ComicResponse } from "../../types/comic";

const comicStatusLabel: Record<ComicStatus, string> = {
  [ComicStatus.Continuing]: "Đang cập nhật",
  [ComicStatus.Paused]: "Tạm dừng",
  [ComicStatus.Stopped]: "Ngừng xuất bản",
  [ComicStatus.Completed]: "Đã hoàn thành"
};

type TopComicsSectionProps = {
  topComics: ComicResponse[];
  showLoading: boolean;
};

const TopComicsSection = ({ topComics, showLoading }: TopComicsSectionProps) => {
  const placeholderItems = useMemo(() => Array.from({ length: 4 }), []);

  return (
    <div className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
      <header className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <p className="text-xs uppercase tracking-[0.3em] text-primary/70">Top truyện nổi bật</p>
          <h3 className="text-xl font-semibold text-primary-foreground">Xếp hạng theo điểm đánh giá</h3>
        </div>
        <div className="text-xs text-surface-foreground/60">
          Cập nhật từ {showLoading ? "--" : `${formatNumber(topComics.length)} mục`} mới nhất
        </div>
      </header>

      <div className="mt-6 space-y-4">
        {showLoading && (
          <div className="space-y-3">
            {placeholderItems.map((_, index) => (
              <div key={index} className="h-20 animate-pulse rounded-2xl bg-surface-muted/40" />
            ))}
          </div>
        )}

        {!showLoading && topComics.length === 0 && (
          <div className="rounded-2xl border border-dashed border-surface-muted/70 bg-surface px-4 py-6 text-center text-sm text-surface-foreground/60">
            Chưa có dữ liệu truyện. Hãy thêm truyện mới để bắt đầu theo dõi.
          </div>
        )}

        {!showLoading &&
          topComics.map((comic) => (
            <article
              key={comic.id}
              className="rounded-2xl border border-surface-muted/60 bg-surface px-4 py-4 transition hover:border-primary/60"
            >
              <div className="flex flex-wrap items-center justify-between gap-4">
                <div>
                  <h4 className="text-lg font-semibold text-primary-foreground">{comic.name}</h4>
                  <p className="text-sm text-surface-foreground/70">
                    {comic.author} • {comicStatusLabel[comic.status]}
                  </p>
                </div>
                <div className="text-right">
                  <p className="text-xs uppercase tracking-wide text-surface-foreground/60">Điểm đánh giá</p>
                  <p className="text-base font-semibold text-primary-foreground">{comic.rate?.toFixed(2) ?? "--"}</p>
                </div>
              </div>
              <div className="mt-4 flex flex-wrap items-center gap-4 text-xs text-surface-foreground/60">
                <span>{formatNumber(comic.chap_count)} chương</span>
                <span>•</span>
                <span>Cập nhật {formatRelativeTime(comic.updated_at)}</span>
                <span>•</span>
                <span className="flex items-center gap-1">
                  Slug: {comic.slug}
                  <ArrowRight className="h-3.5 w-3.5 text-surface-foreground/40" />
                </span>
              </div>
            </article>
          ))}
      </div>
    </div>
  );
};

export default TopComicsSection;
