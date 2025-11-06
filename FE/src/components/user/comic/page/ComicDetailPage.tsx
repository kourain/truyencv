"use client";

import { useMemo } from "react";
import { useParams } from "next/navigation";

import AdsBanner from "@components/user/comic/AdsBanner";
import AuthorOtherWorks from "@components/user/comic/AuthorOtherWorks";
import HeroSection from "@components/user/comic/HeroSection";
import LatestChapters from "@components/user/comic/LatestChapters";
import StoryIntroduction from "@components/user/comic/StoryIntroduction";
import TabViewSection from "@components/user/comic/TabViewSection";
import { useUserComicDetailQuery } from "@services/user/comic-detail.service";

const ComicDetailPage = () => {
  const params = useParams<{ slug: string }>();
  const slug = useMemo(() => params?.slug, [params]);
  const { data, isLoading } = useUserComicDetailQuery(slug ?? "");

  if (!slug) {
    return null;
  }

  return (
    <main className="mx-auto flex w-full max-w-6xl flex-1 flex-col gap-10 px-6 py-10">
      <HeroSection comic={data?.comic} isLoading={isLoading} />

      <AdsBanner advertisement={data?.advertisements.primary} variant="primary" isLoading={isLoading} />

      <LatestChapters chapters={data?.latest_chapters} slug={slug} isLoading={isLoading} />

      <AdsBanner advertisement={data?.advertisements.secondary} variant="secondary" isLoading={isLoading} />

      <StoryIntroduction introduction={data?.introduction} isLoading={isLoading} />

      <AuthorOtherWorks items={data?.related_by_author} authorName={data?.comic.author_name} slug={slug} isLoading={isLoading} />

      <AdsBanner advertisement={data?.advertisements.tertiary} variant="tertiary" isLoading={isLoading} />

      <TabViewSection
        highlights={data?.highlights}
        reviews={data?.reviews}
        discussions={data?.discussions}
        comicId={data?.comic?.id}
        slug={slug}
        isLoading={isLoading}
      />
    </main>
  );
};

export default ComicDetailPage;
