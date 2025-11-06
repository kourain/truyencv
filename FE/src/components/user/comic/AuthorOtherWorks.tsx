"use client";

import Link from "next/link";
import Image from "next/image";


interface AuthorOtherWorksProps {
  items?: ComicDetailRelatedComic[];
  authorName?: string;
  slug?: string;
  isLoading?: boolean;
}

import { useComicsEmbeddedBySlugQuery } from "@services/user/comic-embedded.service";

const AuthorOtherWorks = ({ items, authorName, slug, isLoading = false }: AuthorOtherWorksProps) => {
  const { data: embeddedItems, isLoading: isLoadingEmbedded } = useComicsEmbeddedBySlugQuery(slug ?? "");
  if (isLoading) {
    return (
      <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
        <div className="h-6 w-64 animate-pulse rounded bg-surface-muted/60" />
        <div className="mt-6 grid gap-4 sm:grid-cols-3">
          {Array.from({ length: 3 }).map((_, index) => (
            <div key={index} className="flex flex-col gap-3">
              <div className="h-36 w-full animate-pulse rounded-2xl bg-surface-muted/40" />
              <div className="h-4 w-3/4 animate-pulse rounded bg-surface-muted/40" />
            </div>
          ))}
        </div>
      </section>
    );
  }

  const effectiveItems = items ?? embeddedItems;

  if (!effectiveItems?.length) {
    return null;
  }

  return (
    <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
      <header className="mb-6 flex flex-col gap-1">
        <p className="text-xs uppercase tracking-[0.3em] text-primary-foreground/60">Cùng đăng bởi tác giả này</p>
        <h2 className="text-xl font-semibold text-primary-foreground">
          Những tác phẩm khác của {authorName ?? "tác giả"}
        </h2>
      </header>
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
  {effectiveItems.map((item) => (
          <Link
            key={item.id}
            href={`/user/comic/${item.slug}`}
            className="group flex flex-col gap-3 rounded-2xl border border-surface-muted/60 bg-surface p-4 transition hover:border-primary/40 hover:bg-primary/5"
          >
            <div className="relative h-40 w-full overflow-hidden rounded-xl border border-surface-muted/60">
              <Image
                src={item.cover_url ?? "https://picsum.photos/seed/related-fallback/200/260"}
                alt={item.title}
                fill
                sizes="(min-width: 1024px) 220px, (min-width: 640px) 280px, 100vw"
                className="object-cover transition duration-500 group-hover:scale-105"
                unoptimized
              />
            </div>
            <div className="flex flex-col gap-1">
              <h3 className="text-base font-semibold text-primary-foreground line-clamp-2">{item.title}</h3>
              {item.latest_chapter && (
                <span className="text-xs uppercase tracking-wide text-surface-foreground/60">
                  Chương mới nhất: {item.latest_chapter}
                </span>
              )}
            </div>
          </Link>
        ))}
      </div>
    </section>
  );
};

export default AuthorOtherWorks;
