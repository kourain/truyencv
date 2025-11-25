"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import type { Route } from "next";
import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";

import { BookOpen, Filter, Search as SearchIcon, Star, Tag } from "lucide-react";

import EmptyState from "@components/user/home/EmptyState";
import { formatNumber } from "@helpers/format";
import { useComicSearchQuery, type SearchComicResult } from "@services/user/search.service";

const DEFAULT_PAGE_SIZE = 12;

export const UserSearchContent = () => {
  const router = useRouter();
  const searchParams = useSearchParams();

  const keywordParam = searchParams.get("keyword") ?? "";
  const pageParam = Number.parseInt(searchParams.get("page") ?? "1", 10);
  const currentPage = Number.isNaN(pageParam) || pageParam < 1 ? 1 : pageParam;

  const [searchValue, setSearchValue] = useState(keywordParam);

  useEffect(() => {
    setSearchValue(keywordParam);
  }, [keywordParam]);

  const { data, isLoading } = useComicSearchQuery({ keyword: keywordParam, page: currentPage, page_size: DEFAULT_PAGE_SIZE });

  const results = data?.results ?? [];
  const total = data?.total ?? 0;
  const pageSize = data?.page_size ?? DEFAULT_PAGE_SIZE;
  const totalPages = Math.max(1, Math.ceil(total / pageSize));

  const handleSearch = useCallback(
    (keyword: string) => {
      const href = buildSearchRoute(keyword, 1);
      router.push(href);
    },
    [router]
  );

  const handleSubmit = useCallback(
    (event: React.FormEvent<HTMLFormElement>) => {
      event.preventDefault();
      if (!searchValue.trim()) {
        return;
      }

      handleSearch(searchValue.trim());
    },
    [handleSearch, searchValue]
  );
  const handleChangePage = useCallback(
    (page: number) => {
      const href = buildSearchRoute(keywordParam, page);
      router.push(href);
    },
    [keywordParam, router]
  );

  const pagination = useMemo(() => buildPagination(currentPage, totalPages), [currentPage, totalPages]);

  return (
    <div className="relative flex min-h-screen flex-col bg-surface">
      <main className="mx-auto flex w-full max-w-6xl flex-1 flex-col gap-8 px-6 py-10">
        <header className="rounded-lg border border-surface-muted/60 bg-surface/80 p-6 shadow-xl">
          <form onSubmit={handleSubmit} className="flex flex-col gap-4 md:flex-row md:items-center md:gap-6">
            <div className="flex-1">
              <label htmlFor="search" className="text-xs uppercase tracking-[0.4em] text-surface-foreground/60">
                Tìm kiếm truyện
              </label>
            </div>
            <div className="flex items-center gap-3 text-xs text-surface-foreground/60">
              <Filter className="h-4 w-4" />
              <span>
                {total > 0
                  ? `Có ${formatNumber(total)} kết quả cho `
                  : "Chưa tìm thấy truyện phù hợp. Thử với từ khóa khác."}
                {keywordParam && total > 0 && <strong className="ml-1 text-primary">“{keywordParam}”</strong>}
              </span>
            </div>
          </form>
        </header>

        <section className="grid gap-4">
          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
            {isLoading
              ? Array.from({ length: 6 }).map((_, index) => <SearchSkeletonCard key={index} />)
              : results.map((comic) => <SearchResultCard key={comic.id} comic={comic} />)}
          </div>
          {!isLoading && results.length === 0 && <EmptyState message="Không tìm thấy truyện trùng khớp với từ khóa." />}
        </section>

        {totalPages > 1 && (
          <nav className="flex flex-wrap items-center justify-between gap-3 rounded-lg border border-surface-muted/60 bg-surface/80 px-4 py-3 text-sm text-surface-foreground/70">
            <div>
              Trang {currentPage} / {totalPages}
            </div>
            <div className="flex items-center gap-2">
              <button
                type="button"
                className="rounded-md border border-surface-muted/60 px-3 py-1.5 transition hover:border-primary disabled:cursor-not-allowed disabled:opacity-50"
                onClick={() => handleChangePage(Math.max(1, currentPage - 1))}
                disabled={currentPage === 1}
              >
                Trước
              </button>
              {pagination.map((item) => (
                <button
                  key={item.key}
                  type="button"
                  disabled={item.disabled}
                  className={`rounded-md px-3 py-1.5 text-sm transition ${item.active
                    ? "bg-primary text-primary-foreground"
                    : "border border-surface-muted/60 hover:border-primary"
                    }`}
                  onClick={() => item.page && handleChangePage(item.page)}
                >
                  {item.label}
                </button>
              ))}
              <button
                type="button"
                className="rounded-md border border-surface-muted/60 px-3 py-1.5 transition hover:border-primary disabled:cursor-not-allowed disabled:opacity-50"
                onClick={() => handleChangePage(Math.min(totalPages, currentPage + 1))}
                disabled={currentPage === totalPages}
              >
                Sau
              </button>
            </div>
          </nav>
        )}
      </main>

      <footer className="border-t border-surface-muted/60 bg-surface/80 py-6 text-center text-sm text-surface-foreground/60">
        © {new Date().getFullYear()} TruyenCV. Bản quyền thuộc về trang web.
      </footer>
    </div>
  );
};

const SearchResultCard = ({ comic }: { comic: SearchComicResult }) => (
  <Link href={`/user/comic/${comic.slug}`} className="block">
    <article className="flex h-full flex-col gap-3 rounded-lg border border-surface-muted/60 bg-surface/80 p-4 shadow-lg transition hover:-translate-y-1 hover:border-primary hover:shadow-2xl">
      <div className="flex gap-3">
        <div className="relative h-32 w-24 flex-shrink-0 overflow-hidden rounded-md bg-surface-muted/60">
          {comic.cover_url ? (
            <img src={comic.cover_url} alt={comic.name} className="h-full w-full object-cover" loading="lazy" />
          ) : (
            <div className="flex h-full w-full items-center justify-center text-xs text-surface-foreground/60">No cover</div>
          )}
        </div>
        <div className="flex flex-1 flex-col gap-2">
          <div>
            <h3 className="text-base font-semibold text-primary-foreground line-clamp-2">{comic.name}</h3>
            {comic.author && <p className="text-xs text-surface-foreground/60">Tác giả: {comic.author}</p>}
          </div>
          {comic.description && (
            <p className="text-xs leading-relaxed text-surface-foreground/70 line-clamp-2">{comic.description}</p>
          )}
          {comic.main_category && (
            <div className="flex items-center gap-2 text-[11px] text-surface-foreground/60">
              <Tag className="h-3 w-3" />
              <span className="rounded-md bg-surface-muted/60 px-2 py-1">
                {comic.main_category}
              </span>
            </div>
          )}
        </div>
      </div>
      <div className="flex items-center justify-between gap-2 text-xs text-surface-foreground/60">
        <span className="flex items-center gap-1">
          <BookOpen className="h-3.5 w-3.5" /> {comic.chap_count} chương
        </span>

        <span className="flex items-center gap-1">
          <Star className={`h-3.5 w-3.5 ${comic.rate > 0 ? 'text-primary' : 'text-surface-foreground/40'}`} />
          <span className={comic.rate > 0 ? 'text-primary' : 'text-surface-foreground/60'}>
            {comic.rate.toFixed(1)} ({formatNumber(comic.rate_count)})
          </span>
        </span>

        <span className="flex items-center gap-1 text-primary">
          <Tag className="h-3.5 w-3.5" />
          {(comic.match_score * 100).toFixed(0)}%
        </span>
      </div>
    </article>
  </Link>
);

const SearchSkeletonCard = () => (
  <div className="h-full rounded-lg border border-surface-muted/60 bg-surface-muted/40 p-4">
    <div className="mb-3 flex gap-3">
      <div className="h-32 w-24 animate-pulse rounded-md bg-surface-muted/80" />
      <div className="flex flex-1 flex-col gap-2">
        <div className="h-4 w-3/4 animate-pulse rounded-full bg-surface-muted/80" />
        <div className="h-3 w-full animate-pulse rounded-full bg-surface-muted/60" />
        <div className="h-3 w-2/3 animate-pulse rounded-full bg-surface-muted/60" />
      </div>
    </div>
    <div className="mt-auto flex items-center justify-between text-xs">
      <div className="h-3 w-20 animate-pulse rounded-full bg-surface-muted/60" />
      <div className="h-3 w-24 animate-pulse rounded-full bg-surface-muted/60" />
      <div className="h-3 w-12 animate-pulse rounded-full bg-surface-muted/60" />
    </div>
  </div>
);

const buildSearchRoute = (keyword: string, page: number): Route => {
  const search = new URLSearchParams();
  if (keyword.trim()) {
    search.set("keyword", keyword.trim());
  }
  if (page > 1) {
    search.set("page", page.toString());
  }

  const query = search.toString();
  return (`/user/search${query ? `?${query}` : ""}`) as unknown as Route;
};

const buildPagination = (current: number, total: number): PaginationButton[] => {
  const buttons: PaginationButton[] = [];

  if (total <= 7) {
    for (let page = 1; page <= total; page += 1) {
      buttons.push({
        key: `page-${page}`,
        label: page.toString(),
        page,
        active: page === current,
      });
    }
    return buttons;
  }

  const addPage = (page: number) => {
    buttons.push({
      key: `page-${page}`,
      label: page.toString(),
      page,
      active: page === current,
    });
  };

  addPage(1);

  const start = Math.max(2, current - 1);
  const end = Math.min(total - 1, current + 1);

  if (start > 2) {
    buttons.push({ key: "ellipsis-start", label: "...", disabled: true });
  }

  for (let page = start; page <= end; page += 1) {
    addPage(page);
  }

  if (end < total - 1) {
    buttons.push({ key: "ellipsis-end", label: "...", disabled: true });
  }

  addPage(total);

  return buttons;
};
