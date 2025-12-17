"use client";
import { useSearch } from "@components/providers/SearchProvider";
import clsx from "clsx";
import { SearchIcon } from "lucide-react";
import { useRouter } from "next/navigation";
import { useMemo } from "react";

type SearchBarProps = {
  className?: string;
};

export const SearchBar = ({ className }: SearchBarProps) => {
  const router = useRouter();
  const search = useSearch()
  const canSearch = search.keyword.length > 0;

  const handleSearch = (keyword: string) => {
    const value = keyword.trim();
    if (!value) {
      return;
    }

    search.setKeyword(value);
    search.setPage(1);
    const href = `/user/search`;
    router.push(href);
  };

  return (
    <div className={clsx("flex items-center gap-3", className)}>
      <div className="flex flex-1 items-center gap-2 rounded-md border border-surface-muted/70 bg-surface/90 px-3 py-2 shadow-sm transition focus-within:border-primary focus-within:ring-2 focus-within:ring-primary/20">
        <SearchIcon className="h-4 w-4 flex-none text-surface-foreground/50" />
        <input
          id="search"
          type="search"
          value={search.keyword}
          onChange={(event) => search.setKeyword(event.target.value)}
          onKeyDown={(event) => {
            if (event.key === "Enter") {
              handleSearch(search.keyword);
            }
          }}
          placeholder="Tìm kiếm truyện, tác giả hoặc thể loại..."
          className="w-full bg-transparent text-sm text-surface-foreground placeholder:text-surface-foreground/50 focus:outline-none"
        />
      </div>
      <button
        onClick={() => handleSearch(search.keyword)}
        type="submit"
        disabled={!canSearch}
        className="inline-flex h-11 w-11 items-center justify-center rounded-full border border-primary/60 bg-primary text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:opacity-60"
        aria-label="Tìm kiếm"
      >
        <SearchIcon className="h-4 w-4" />
        <span className="sr-only">Tìm kiếm</span>
      </button>
    </div>
  );
};
