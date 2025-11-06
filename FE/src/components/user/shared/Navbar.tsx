"use client";

import Image from "next/image";
import Link from "next/link";
import { useCallback, useEffect, useRef, useState } from "react";
import clsx from "clsx";

import { Coins, Crown, Loader2, LogOut, Settings, Sparkles, Ticket, User as UserIcon } from "lucide-react";
import { SearchBar } from "./SearchBar";
import { clearAuthTokens } from "@helpers/authTokens";
import { usePathname, useRouter } from "next/navigation";
import { useAuth } from "@hooks/useAuth";
import { redirectToLogin } from "@helpers/httpClient";
import { useUserProfileQuery } from "@services/user/profile.service";
import { formatNumber } from "@helpers/format";
const UserNavbar = () => {
  const router = useRouter();
  const auth = useAuth();
  const { data: profile, isFetching: isProfileFetching } = useUserProfileQuery({ enabled: auth.isAuthenticated });
  const pathname = usePathname();
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [showMobileSearch, setShowMobileSearch] = useState(true);
  const searchVisibilityRef = useRef(true);
  const avatarSrc = auth.avatar?.trim() || null;
  const handleLogout = useCallback(async () => {
    console.log("UserNavbar: Logging out...");
    await clearAuthTokens();
    window.location.href = "/user/auth/login";
  }, []);

  const handleOpenSettings = useCallback(() => {
    router.push("/user/profile");
  }, [router]);

  useEffect(() => {
    if (typeof window === "undefined") {
      return;
    }

    const mediaQuery = window.matchMedia("(max-width: 767px)");
    let lastScrollY = window.scrollY;
    let ticking = false;

    const revealSearch = () => {
      if (!searchVisibilityRef.current) {
        searchVisibilityRef.current = true;
        setShowMobileSearch(true);
      }
    };

    const hideSearch = () => {
      if (searchVisibilityRef.current) {
        searchVisibilityRef.current = false;
        setShowMobileSearch(false);
      }
    };

    const evaluateScroll = () => {
      if (!mediaQuery.matches) {
        revealSearch();
        lastScrollY = window.scrollY;
        return;
      }

      const currentY = window.scrollY;
      const delta = currentY - lastScrollY;

      if (delta > 10) {
        hideSearch();
      } else if (delta < -10) {
        revealSearch();
      }

      lastScrollY = currentY;
    };

    const handleScroll = () => {
      if (!ticking) {
        window.requestAnimationFrame(() => {
          evaluateScroll();
          ticking = false;
        });
        ticking = true;
      }
    };

    const handleMediaChange = () => {
      revealSearch();
      lastScrollY = window.scrollY;
    };

    window.addEventListener("scroll", handleScroll, { passive: true });
    if (mediaQuery.addEventListener) {
      mediaQuery.addEventListener("change", handleMediaChange);
    } else {
      mediaQuery.addListener(handleMediaChange);
    }

    return () => {
      window.removeEventListener("scroll", handleScroll);
      if (mediaQuery.removeEventListener) {
        mediaQuery.removeEventListener("change", handleMediaChange);
      } else {
        mediaQuery.removeListener(handleMediaChange);
      }
    };
  }, []);

  const renderUserControls = (wrapperClass: string) => (
    <div className={clsx("relative flex items-center gap-3", wrapperClass)}>
      {auth.isAuthenticated ? (
        <button
          type="button"
          className="inline-flex h-10 w-10 items-center justify-center overflow-hidden rounded-full border border-surface-muted/80 bg-surface transition hover:border-primary"
          onClick={() => setIsUserMenuOpen((prev) => !prev)}
        >
          {avatarSrc ? (
            <Image
              src={avatarSrc}
              alt={auth.name ? `Ảnh đại diện của ${auth.name}` : "Ảnh đại diện người dùng"}
              width={40}
              height={40}
              className="h-full w-full object-cover"
              unoptimized
            />
          ) : (
            <UserIcon className="h-5 w-5 text-primary-foreground" />
          )}
        </button>
      ) : (
        <button className="inline-flex flex-row justify-center" onClick={() => { redirectToLogin(); }}>
          <UserIcon className="h-5 w-5 text-primary-foreground" />
          Đăng nhập
        </button>
      )}

      {isUserMenuOpen && (
        <div className="absolute right-0 top-12 w-56 overflow-hidden rounded-2xl border border-surface-muted/60 bg-surface shadow-2xl">
          <div className="border-b border-surface-muted/60 px-4 py-3">
            <p className="text-sm font-semibold text-primary-foreground">Xin chào! {auth.name}</p>
            <p className="text-xs text-surface-foreground/60">Quản lý tài khoản của bạn</p>
          </div>
          <nav className="flex flex-col py-1 text-sm text-surface-foreground/80">
            {auth.isAuthenticated && (
              <div className="border-b border-surface-muted/60 px-4 pb-3 pt-3 text-xs text-surface-foreground/70">
                <div className="mb-2 flex items-center justify-between">
                  <span className="text-[11px] font-semibold uppercase tracking-wide text-surface-foreground/60">Ví của bạn</span>
                  {isProfileFetching && <Loader2 className="h-3.5 w-3.5 animate-spin text-surface-foreground/40" />}
                </div>
                <ul className="space-y-1.5">
                  <li className="flex items-center justify-between">
                    <span className="inline-flex items-center gap-2">
                      <Coins className="h-4 w-4 text-primary" />
                      <span>Xu</span>
                    </span>
                    <span className="font-semibold text-primary-foreground">
                      {profile ? formatNumber(Number(profile.coin ?? 0)) : isProfileFetching ? "..." : "—"}
                    </span>
                  </li>
                  <li className="flex items-center justify-between">
                    <span className="inline-flex items-center gap-2">
                      <Ticket className="h-4 w-4 text-primary" />
                      <span>Vé</span>
                    </span>
                    <span className="font-semibold text-primary-foreground">
                      {profile ? formatNumber(Number(profile.key ?? 0)) : isProfileFetching ? "..." : "—"}
                    </span>
                  </li>
                  <li className="flex items-start justify-between">
                    <span className="inline-flex items-center gap-2 pt-0.5">
                      <Crown className="h-4 w-4 text-primary" />
                      <span>Gói hiện tại</span>
                    </span>
                    <span className="text-right font-medium text-primary-foreground">
                      {profile ? profile.active_subscription_name ?? "Chưa đăng ký" : isProfileFetching ? "..." : "Chưa đăng ký"}
                    </span>
                  </li>
                </ul>
              </div>
            )}
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
  );
  if (pathname.match(/login|register|reset-password|verify-email/)) {
    return null;
  }
  return (
    <header className="sticky top-0 z-40 border-b border-surface-muted/60 bg-surface/90 backdrop-blur">
      <div className="mx-auto flex max-w-6xl flex-col gap-3 px-6 py-4 md:flex-row md:items-center md:justify-between md:gap-6">
        <div className="flex items-center justify-between gap-3 md:justify-start">
          <Link href="/user" className="flex items-center gap-3 text-primary-foreground transition hover:text-primary">
            <Sparkles className="h-8 w-8 text-primary" />
            <div className="flex flex-col leading-tight">
              <span className="text-lg font-semibold">TruyenCV</span>
              <span className="text-xs uppercase tracking-[0.4em] text-surface-foreground/60">Cổng truyện</span>
            </div>
          </Link>
          {renderUserControls("md:hidden")}
        </div>

        <div
          className={clsx(
            "w-full overflow-hidden transition-all duration-200 ease-out md:flex md:flex-1 md:items-center md:overflow-visible md:opacity-100 md:pointer-events-auto",
            showMobileSearch ? "max-h-24 opacity-100 pointer-events-auto" : "pointer-events-none max-h-0 opacity-0 -translate-y-2",
            "md:translate-y-0"
          )}
        >
          <SearchBar className="w-full" />
        </div>

        {renderUserControls("hidden md:flex")}
      </div>
    </header>
  );
};

export default UserNavbar;
