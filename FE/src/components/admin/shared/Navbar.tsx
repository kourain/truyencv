"use client";

import Link from "next/link";
import { useCallback, useState } from "react";

import { Coins, Crown, Loader2, LogOut, Settings, Sparkles, Ticket, User as UserIcon } from "lucide-react";
import { clearAuthTokens } from "@helpers/authTokens";
import { usePathname, useRouter } from "next/navigation";
import { useAuth } from "@hooks/useAuth";
import Image from "next/image";
import { useUserProfileQuery } from "@services/user/profile.service";
import { formatNumber } from "@helpers/format";

const AdminNavbar = () => {
  const pathname = usePathname();
  if (pathname.match(/login|register|reset-password|verify-email/)) {
    return null;
  }
  else return <AdminNavBarRender />;
};
const AdminNavBarRender = () => {
  const router = useRouter();
  const auth = useAuth();
  const { data: profile, isFetching: isProfileFetching } = useUserProfileQuery({ enabled: auth.isAuthenticated });
  const avatarSrc = auth.userProfile.avatar?.trim() || null;
  const [isAdminMenuOpen, setIsAdminMenuOpen] = useState(false);
  const handleLogout = useCallback(async () => {
    await clearAuthTokens();
    window.location.href = "/admin/auth/login";
  }, []);
  const handleOpenSettings = useCallback(() => {
    router.push("/admin/profile");
  }, [router]);
  return (
    <header className="sticky top-0 z-40 border-b border-surface-muted/60 bg-surface/90 backdrop-blur">
      <div className="mx-auto flex max-w-6xl items-center justify-between gap-6 px-6 py-4">
        <div className="flex items-center gap-4">
          <Link href="/admin" className="flex items-center gap-3 text-primary-foreground transition hover:text-primary">
            <Sparkles className="h-8 w-8 text-primary" />
            <div className="flex flex-col leading-tight">
              <span className="text-lg font-semibold">TruyenCV</span>
              <span className="text-xs uppercase tracking-[0.4em] text-surface-foreground/60">Admin Cổng truyện</span>
            </div>
          </Link>
        </div>

        <div className="relative flex items-center gap-3">
          <button
            type="button"
            className="inline-flex h-10 w-10 items-center justify-center overflow-hidden rounded-full border border-surface-muted/80 bg-surface transition hover:border-primary"
            onClick={() => setIsAdminMenuOpen((prev) => !prev)}
          >
            {avatarSrc ? (
              <Image
                src={avatarSrc}
                alt={auth.userProfile.name ? `Ảnh đại diện của ${auth.userProfile.name}` : "Ảnh đại diện người dùng"}
                width={40}
                height={40}
                className="h-full w-full object-cover"
                unoptimized
              />
            ) : (
              <UserIcon className="h-5 w-5 text-primary-foreground" />
            )}
          </button>

          {isAdminMenuOpen && (
            <div className="absolute right-0 top-12 w-56 overflow-hidden rounded-2xl border border-surface-muted/60 bg-surface shadow-2xl">
              <div className="border-b border-surface-muted/60 px-4 py-3">
                <p className="text-sm font-semibold text-primary-foreground">Xin chào! {auth.userProfile.name}</p>
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
                    setIsAdminMenuOpen(false);
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
                    setIsAdminMenuOpen(false);
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
export default AdminNavbar;
