"use client";

import Link from "next/link";
import { useCallback, useState } from "react";

import { LogOut, Settings, Sparkles, User as UserIcon } from "lucide-react";
import { SearchBar } from "./SearchBar";
import { clearAuthTokens } from "@helpers/authTokens";
import { usePathname, useRouter } from "next/navigation";
import { useAuth } from "@hooks/useAuth";
const UserNavbar = () => {
  const router = useRouter();
  const auth = useAuth();
  const pathname = usePathname();
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const handleLogout = useCallback(async () => {
    await clearAuthTokens();
    router.push("/user/auth/login");
  }, [router]);

  const handleOpenSettings = useCallback(() => {
    router.push("/user/profile");
  }, [router]);
  if (pathname.match(/login|register/)) {
    return null;
  }
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

        <SearchBar />

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
                <p className="text-sm font-semibold text-primary-foreground">Xin chào! {auth.name}</p>
                <p className="text-xs text-surface-foreground/60">Quản lý tài khoản của bạn</p>
              </div>
              <nav className="flex flex-col py-1 text-sm text-surface-foreground/80">
                <button
                  type="button"
                  className="flex items-center gap-2 px-4 py-2.5 text-left transition hover:bg-surface-muted/50"
                  onClick={() => {
                    setIsUserMenuOpen(false);
                    handleOpenSettings();
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
                    handleLogout();
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
    </header>
  );
};

export default UserNavbar;
