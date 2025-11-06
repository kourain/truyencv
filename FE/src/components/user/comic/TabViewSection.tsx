"use client";

import { useState } from "react";

import ReviewsPanel from "@components/user/comic/tabs/ReviewsPanel";
import DiscussionsPanel from "@components/user/comic/tabs/DiscussionsPanel";
import HighlightsPanel from "@components/user/comic/tabs/HighlightsPanel";

interface TabViewSectionProps {
  reviews?: ComicDetailReview[];
  discussions?: ComicDetailDiscussion[];
  highlights?: string[];
  comicId?: string;
  slug?: string;
  isLoading?: boolean;
}

const tabs = [
  { key: "highlights", label: "Điểm nổi bật" },
  { key: "reviews", label: "Đánh giá" },
  { key: "discussions", label: "Thảo luận" },
] as const;

type TabKey = (typeof tabs)[number]["key"];

const TabViewSection = ({ reviews, discussions, highlights, comicId, slug, isLoading = false }: TabViewSectionProps) => {
  const [activeTab, setActiveTab] = useState<TabKey>("highlights");

  return (
    <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
      <nav className="mb-6 flex flex-wrap justify-center gap-3">
        {tabs.map((tab) => {
          const isActive = tab.key === activeTab;
          return (
            <button
              key={tab.key}
              type="button"
              className={`rounded-full px-5 py-2 text-sm font-semibold transition ${
                isActive ? "bg-primary text-primary-foreground shadow-lg" : "border border-surface-muted/60 text-surface-foreground/70 hover:border-primary/40 hover:text-primary"
              }`}
              onClick={() => setActiveTab(tab.key)}
            >
              {tab.label}
            </button>
          );
        })}
      </nav>

      <div>
        {activeTab === "highlights" && (
          <HighlightsPanel highlights={highlights} isLoading={isLoading} />
        )}
        {activeTab === "reviews" && (
          <ReviewsPanel reviews={reviews} isLoading={isLoading} />
        )}
        {activeTab === "discussions" && (
          <DiscussionsPanel discussions={discussions} isLoading={isLoading} comicId={comicId} slug={slug} />
        )}
      </div>
    </section>
  );
};

export default TabViewSection;
