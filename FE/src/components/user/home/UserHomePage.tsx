"use client";

import { BookMarked, Flame } from "lucide-react";

import CompletedCarouselSection from "@components/user/home/CompletedCarouselSection";
import EditorPicksSection from "@components/user/home/EditorPicksSection";
import HistorySection from "@components/user/home/HistorySection";
import LatestReviewsSection from "@components/user/home/LatestReviewsSection";
import LatestUpdatesSection from "@components/user/home/LatestUpdatesSection";
import RankingSection from "@components/user/home/RankingSection";
import { formatNumber } from "@helpers/format";
import { useUserHomeQuery } from "@services/user/home.service";

const UserHomePage = () => {
  const { data, isLoading } = useUserHomeQuery();

  const history = (data?.history ?? []).slice(0, 5);
  const editorPicks = data?.editor_picks ?? [];
  const topRecommended = data?.top_recommended ?? [];
  const topWeeklyReads = data?.top_weekly_reads ?? [];
  const latestUpdates = data?.latest_updates ?? [];
  const recentlyCompleted = data?.recently_completed ?? [];
  const latestReviews = data?.latest_reviews ?? [];

  return (
    <main className="mx-auto flex w-full max-w-6xl flex-1 flex-col gap-10 px-6 py-10">
      <HistorySection items={history} isLoading={isLoading} />

      <EditorPicksSection items={editorPicks} isLoading={isLoading} />

      <section className="grid gap-6 lg:grid-cols-2">
        <RankingSection
          title="Top truyện được đề cử"
          description="Đánh giá cao bởi cộng đồng độc giả"
          icon={<BookMarked className="h-5 w-5 text-primary" />}
          items={topRecommended}
          isLoading={isLoading}
          valueFormatter={(comic) => `${formatNumber(comic.recommendation_score ?? 0)} điểm`}
          emptyMessage="Chưa có dữ liệu đề cử."
        />

        <RankingSection
          title="Đọc nhiều tuần này"
          description="Danh sách nóng với lượt xem cao nhất"
          icon={<Flame className="h-5 w-5 text-primary" />}
          items={topWeeklyReads}
          isLoading={isLoading}
          valueFormatter={(comic) => `${formatNumber(comic.weekly_views ?? comic.total_views)} lượt đọc`}
          emptyMessage="Chưa có thống kê tuần này."
        />
      </section>

      <LatestUpdatesSection items={latestUpdates} isLoading={isLoading} />

      <CompletedCarouselSection items={recentlyCompleted} isLoading={isLoading} />

      <LatestReviewsSection items={latestReviews} isLoading={isLoading} />
    </main>
  );
};

export default UserHomePage;
