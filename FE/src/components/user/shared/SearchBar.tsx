"use client";
import { SearchIcon } from "lucide-react"
import { useRouter } from "next/navigation";
import { useState } from "react";

export const SearchBar = () => {
    const router = useRouter();

    const handleSearch = (keyword: string) => {
        const href = `/user/search?keyword=${encodeURIComponent(keyword)}`;
        router.push(href);
    };
    const [searchValue, setSearchValue] = useState("");
    return (
        <div className="mt-2 flex items-center gap-3">
            <div className="relative flex-1">
                <SearchIcon className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-surface-foreground/50" />
                <input
                    id="search"
                    type="search"
                    value={searchValue}
                    onChange={(event) => setSearchValue(event.target.value)}
                    onKeyDown={(event) => event.key === "Enter" && handleSearch(searchValue)}
                    placeholder="Tìm kiếm truyện, tác giả hoặc thể loại..."
                    className="w-full rounded-full border border-surface-muted/80 bg-surface px-10 py-2 text-sm text-surface-foreground shadow-inner outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
                />
            </div>
            <button
                onClick={() => handleSearch(searchValue)}
                type="submit"
                className="inline-flex items-center gap-2 rounded-full bg-primary px-5 py-2 text-sm font-semibold text-primary-foreground transition hover:shadow-glow"
            >
                <SearchIcon className="h-4 w-4" />
                Tìm kiếm
            </button>
        </div>
    )
}
