"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";

import { LogOut, Search, Settings, Sparkles, User as UserIcon } from "lucide-react";

interface UserHomeNavbarProps {
  onSearch: (keyword: string) => void;
  onLogout: () => void;
  onOpenSettings?: () => void;
  initialKeyword?: string;
}

const UserHomeNavbar = ({ onSearch, onLogout, onOpenSettings, initialKeyword = "" }: UserHomeNavbarProps) => {
  const [searchValue, setSearchValue] = useState(initialKeyword);
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);

  const trimmedKeyword = useMemo(() => searchValue.trim(), [searchValue]);

  useEffect(() => {
    setSearchValue(initialKeyword);
  }, [initialKeyword]);

  const triggerSearch = () => {
    if (!trimmedKeyword) {
      return;
    }

    onSearch(trimmedKeyword);
  };

  return (
    <header className="sticky top-0 z-40 border-b border-surface-muted/60 bg-surface/90 backdrop-blur">
      <div className="mx-auto flex max-w-6xl items-center justify-between gap-6 px-6 py-4">
        <Link href="/user" className="flex items-center gap-3 text-primary-foreground transition hover:text-primary">
          <Sparkles className="h-8 w-8 text-primary" />
          <div className="flex flex-col leading-tight">
            <span className="text-lg font-semibold">TruyenCV</span>
            <span className="text-xs uppercase tracking-[0.4em] text-surface-foreground/60">Cổng truyện</span>
          </div>
        </Link>

        <div className="hidden flex-1 items-center justify-center sm:flex">
          <div className="relative w-full max-w-xl">
            <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-surface-foreground/50" />
            <input
              type="search"
              value={searchValue}
              onChange={(event) => setSearchValue(event.target.value)}
              onKeyDown={(event) => event.key === "Enter" && triggerSearch()}
              placeholder="Tìm kiếm truyện, tác giả hoặc thể loại..."
              className="w-full rounded-full border border-surface-muted/80 bg-surface px-10 py-2 text-sm text-surface-foreground shadow-inner outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
            />
          </div>
        </div>

        <div className="relative flex items-center gap-3">
          <button
            type="button"
            className="inline-flex h-10 w-10 items-center justify-center rounded-full border border-surface-muted/80 bg-surface transition hover:border-primary"
            onClick={() => setIsUserMenuOpen((prev) => !prev)}
          >
            <UserIcon className="h-5 w-5 text-primary-foreground" />
          </button>

          {isUserMenuOpen && (
            <div className="absolute right-0 top-12 w-56 overflow-hidden rounded-2xl border border-surface-muted/60 bg-surface shadow-2xl">
              <div className="border-b border-surface-muted/60 px-4 py-3">
                <p className="text-sm font-semibold text-primary-foreground">Xin chào!</p>
                <p className="text-xs text-surface-foreground/60">Quản lý tài khoản của bạn</p>
              </div>
              <nav className="flex flex-col py-1 text-sm text-surface-foreground/80">
                <button
                  type="button"
                  className="flex items-center gap-2 px-4 py-2.5 text-left transition hover:bg-surface-muted/50"
                  onClick={() => {
                    setIsUserMenuOpen(false);
                    onOpenSettings?.();
                  }}
                >
                  <Settings className="h-4 w-4" />
                  Cài đặt
                </button>
                <button
                  type="button"
                  className="flex items-center gap-2 px-4 py-2.5 text-left transition hover:bg-surface-muted/50"
                  onClick={() => {
                    setIsUserMenuOpen(false);
                    onLogout();
                  }}
                >
                  <LogOut className="h-4 w-4" />
                  Đăng xuất
                </button>
              </nav>
            </div>
          )}
        </div>
      </div>

      <div className="border-t border-surface-muted/60 px-6 py-3 sm:hidden">
        <div className="relative">
          <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-surface-foreground/50" />
          <input
            type="search"
            value={searchValue}
            onChange={(event) => setSearchValue(event.target.value)}
            onKeyDown={(event) => event.key === "Enter" && triggerSearch()}
            placeholder="Tìm kiếm truyện, tác giả hoặc thể loại..."
            className="w-full rounded-full border border-surface-muted/80 bg-surface px-10 py-2 text-sm text-surface-foreground shadow-inner outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
          />
        </div>
      </div>
    </header>
  );
};

export default UserHomeNavbar;
