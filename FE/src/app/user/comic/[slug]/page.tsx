"use server";
import ComicDetailPage from "@components/user/comic/page/ComicDetailPage";
import { fetchUserComicSEO } from "@services/user/comic-detail.service";
import { Metadata, ResolvingMetadata } from "next";

type Props = {
  params: Promise<{ slug: string }>
  searchParams: Promise<{ [key: string]: string | string[] | undefined }>
}
export async function generateMetadata(
  { params, searchParams }: Props,
  parent: ResolvingMetadata
): Promise<Metadata> {
  const slug = (await params).slug
  const comic = await fetchUserComicSEO(slug); // Fetch comic data
  return {
    title: comic.title,
    description: comic.description,
    keywords: comic.keywords,
    openGraph: {
      title: comic.title,
      description: comic.description,
      images: [
        {
          url: comic.image,
          width: 800,
          height: 600,
          alt: comic.title,
        },
      ],
      type: "article",
    },
    twitter: {
      card: "summary_large_image",
      title: comic.title,
      description: comic.description,
      images: [comic.image],
    },
  };
}
const UserComicDetailPage = () => {
  return <ComicDetailPage />;
};

export default UserComicDetailPage;