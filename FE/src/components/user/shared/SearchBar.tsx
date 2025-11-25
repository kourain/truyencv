"use client";
import clsx from "clsx";
import { SearchIcon } from "lucide-react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { useMemo, useState } from "react";

type SearchBarProps = {
  className?: string;
};

export const SearchBar = ({ className }: SearchBarProps) => {
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const router = useRouter();
  const [searchValue, setSearchValue] = useState(searchParams.get("keyword") ?? "");

  const trimmedValue = useMemo(() => searchValue.trim(), [searchValue]);
  const canSearch = trimmedValue.length > 0;

  const handleSearch = (keyword: string) => {
    const value = keyword.trim();
    if (!value) {
      return;
    }

    const href = pathname === "/user/search" ? `?keyword=${encodeURIComponent(value)}` : `/user/search?keyword=${encodeURIComponent(value)}`;
    router.push(href);
  };

  return (
    <div className={clsx("flex items-center gap-3", className)}>
      <div className="flex flex-1 items-center gap-2 rounded-md border border-surface-muted/70 bg-surface/90 px-3 py-2 shadow-sm transition focus-within:border-primary focus-within:ring-2 focus-within:ring-primary/20">
        <SearchIcon className="h-4 w-4 flex-none text-surface-foreground/50" />
        <input
          id="search"
          type="search"
          value={searchValue}
          onChange={(event) => setSearchValue(event.target.value)}
          onKeyDown={(event) => {
            if (event.key === "Enter") {
              handleSearch(searchValue);
            }
          }}
          placeholder="Tìm kiếm truyện, tác giả hoặc thể loại..."
          className="w-full bg-transparent text-sm text-surface-foreground placeholder:text-surface-foreground/50 focus:outline-none"
        />
      </div>
      <button
        onClick={() => handleSearch(searchValue)}
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
