"use client";

import { Bookmark, BookOpen, Coins, LibraryBig } from "lucide-react";
import { formatNumber } from "@helpers/format";

interface ProfileStatsGridProps {
  profile: UserProfileResponse;
}

const statsConfig = [
  {
    key: "read_comic_count" as const,
    title: "Truyện đã đọc",
    description: "Số đầu truyện bạn đã khám phá",
    icon: LibraryBig,
  },
  {
    key: "read_chapter_count" as const,
    title: "Chương đã đọc",
    description: "Tổng số chương đã hoàn thành",
    icon: BookOpen,
  },
  {
    key: "bookmark_count" as const,
    title: "Truyện theo dõi",
    description: "Danh sách truyện bạn yêu thích",
    icon: Bookmark,
  },
  {
    key: "coin" as const,
    title: "Xu hiện có",
    description: "Số xu đã nạp trong tài khoản",
    icon: Coins,
  },
];

const ProfileStatsGrid = ({ profile }: ProfileStatsGridProps) => {
  return (
    <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
      {statsConfig.map(({ key, title, description, icon: Icon }) => (
        <article
          key={key}
          className="flex flex-col gap-4 rounded-3xl border border-surface-muted/80 bg-surface/80 p-6 shadow-lg transition hover:border-primary/50 hover:shadow-primary/10"
        >
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-sm font-semibold uppercase tracking-wide text-surface-foreground/70">{title}</h2>
              <p className="text-2xl font-bold text-primary-foreground">{formatNumber(Number(profile[key]))}</p>
            </div>
            <span className="inline-flex h-11 w-11 items-center justify-center rounded-2xl bg-primary/10 text-primary">
              <Icon className="h-5 w-5" />
            </span>
          </div>
          <p className="text-sm text-surface-foreground/70">{description}</p>
        </article>
      ))}
    </section>
  );
};

export default ProfileStatsGrid;
