"use client";

import { ReactNode, useMemo, useState } from "react";
import { usePathname, useRouter } from "next/navigation";
import type { Route } from "next";
import Link from "next/link";
import {
    Activity,
    BarChart3,
    BookOpen,
    Layers,
    LogOut,
    MessageCircle,
    ScrollText,
    Settings,
    ShieldCheck,
    Tags,
    Users
} from "lucide-react";
import { useMutation } from "@tanstack/react-query";
import { logout, logoutAll } from "@services/auth.service";

interface AdminShellProps {
    children: ReactNode;
}

type NavItem = {
    label: string;
    href: Route;
    icon: typeof Activity;
    badge?: string;
};

const navItems: NavItem[] = [
    { label: "Bảng điều khiển", href: "/admin" as Route, icon: BarChart3 },
    { label: "Quản lý truyện", href: "/admin/comics" as Route, icon: BookOpen },
    { label: "Thể loại", href: "/admin/categories" as Route, icon: Tags },
    { label: "Chương truyện", href: "/admin/chapters" as Route, icon: ScrollText },
    { label: "Bình luận", href: "/admin/comments" as Route, icon: MessageCircle },
    { label: "Người dùng", href: "/admin/users" as Route, icon: Users },
    { label: "Phân quyền", href: "/admin/user-roles" as Route, icon: ShieldCheck }
];

const AdminShell = ({ children }: AdminShellProps) => {
    const router = useRouter();
    const pathname = usePathname();
    const activeHref = useMemo(() => {
        if (!pathname) {
            return navItems[0]?.href;
        }

        const matched = navItems.find((item) => pathname.toLowerCase().startsWith(item.href.toLowerCase()));

        return matched?.href ?? ("/admin" as Route);
    }, [pathname]);

    const logoutMutation = useMutation({
        mutationFn: () => logout(),
        onSettled: async () => {
            router.replace("/admin/auth/login" as Route);
        }
    });

    return (
        <div className="min-h-screen bg-surface text-surface-foreground">
            <div className="mx-auto flex w-full max-w-[1440px] flex-col lg:flex-row">
                <aside className="hidden w-full max-w-xs flex-col border-b border-surface-muted bg-surface-muted/60 px-6 py-8 lg:flex lg:min-h-screen lg:border-b-0 lg:border-r">
                    <div>
                        <p className="text-xs uppercase tracking-[0.4em] text-primary/80">TruyenCV</p>
                        <h1 className="mt-3 text-2xl font-semibold text-primary-foreground">Trang quản trị</h1>
                        <p className="mt-1 text-sm text-surface-foreground/70">
                            Theo dõi sát sao tình trạng nền tảng và quản trị nội dung chỉ trong một nơi.
                        </p>
                    </div>

                    <nav className="mt-8 flex flex-1 flex-col gap-2">
                        {navItems.map(({ label, href, icon: Icon }) => {
                            const isActive = activeHref === href;

                            return (
                                <Link
                                    key={label}
                                    href={href}
                                    className={`group flex items-center gap-3 rounded-xl px-4 py-3 text-sm font-medium transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary/60 ${isActive
                                        ? "bg-surface text-primary-foreground shadow-glow"
                                        : "text-surface-foreground/80 hover:bg-surface hover:text-primary-foreground"
                                        }`}
                                >
                                    <Icon className={`h-5 w-5 transition ${isActive ? "text-primary" : "text-primary group-hover:scale-110 group-hover:text-primary-foreground"}`} />
                                    <span>{label}</span>
                                </Link>
                            );
                        })}
                    </nav>

                    <div className="mt-6 rounded-2xl border border-surface bg-surface/60 p-4 text-sm text-surface-foreground/80">
                        <p className="font-semibold text-primary-foreground">Nhật ký hệ thống</p>
                        <p className="mt-2 text-xs leading-relaxed text-surface-foreground/60">
                            Ghi nhận các cảnh báo quan trọng về hệ thống và kết nối CDN. Luôn kiểm tra khu vực này để chủ động xử lý sự cố.
                        </p>
                    </div>
                </aside>

                <div className="flex w-full flex-1 flex-col">
                    <header className="border-b border-surface-muted bg-surface/80 backdrop-blur">
                        <div className="flex flex-col gap-3 px-4 py-4 sm:flex-row sm:items-center sm:justify-between sm:px-8">
                            <div>
                                <h2 className="text-lg font-semibold text-primary-foreground">Trung tâm điều phối</h2>
                                <p className="text-sm text-surface-foreground/70">
                                    Ống kính toàn cảnh về lưu lượng truy cập, truyện, người dùng và hiệu năng.
                                </p>
                            </div>
                            <div className="flex flex-wrap items-center gap-2">
                                <Link
                                    href={{ pathname: "/admin/user-roles", hash: "he-thong" }}
                                    className="inline-flex items-center gap-2 rounded-full border border-primary/40 bg-primary/10 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/20"
                                >
                                    <Settings className="h-4 w-4" />
                                    Cấu hình hệ thống
                                </Link>
                                <Link
                                    href={"/admin/users" as Route}
                                    className="inline-flex items-center gap-2 rounded-full border border-surface-muted/70 bg-surface px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/80 transition hover:bg-surface-muted"
                                >
                                    <Users className="h-4 w-4" />
                                    Quản lý quản trị viên
                                </Link>
                                <button
                                    type="button"
                                    onClick={() => logoutMutation.mutate()}
                                    className="inline-flex items-center gap-2 rounded-full border border-red-500/50 bg-red-500/10 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-red-200 transition hover:bg-red-500/20 disabled:cursor-not-allowed disabled:opacity-60"
                                    disabled={logoutMutation.isPending}
                                >
                                    {logoutMutation.isPending ? (
                                        <svg className="h-4 w-4 animate-spin text-red-200" viewBox="0 0 24 24">
                                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                                            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4l3-3-3-3v4a12 12 0 00-12 12h4z" />
                                        </svg>
                                    ) : (
                                        <LogOut className="h-4 w-4" />
                                    )}
                                    Đăng xuất
                                </button>
                            </div>
                        </div>
                        <div className="flex items-center gap-3 px-4 pb-4 sm:px-8">
                            <div className="relative flex-1">
                                <input
                                    type="search"
                                    placeholder="Tìm kiếm truyện, người dùng hoặc điều chỉnh nhanh..."
                                    className="w-full rounded-full border border-surface-muted bg-surface px-5 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/50 focus:outline-none focus:ring-2 focus:ring-primary/50"
                                    aria-label="Tìm kiếm trong khu vực quản trị"
                                />
                            </div>
                            <div className="hidden flex-none items-center gap-2 sm:flex">
                                <span className="rounded-full bg-primary/10 px-3 py-2 text-xs font-medium uppercase tracking-wide text-primary">
                                    CDN ổn định
                                </span>
                                <span className="rounded-full border border-surface-muted px-3 py-2 text-xs font-medium uppercase tracking-wide text-surface-foreground/70">
                                    API phản hồi 98ms
                                </span>
                            </div>
                        </div>
                        <nav className="flex items-center gap-2 overflow-x-auto border-t border-surface-muted/60 px-4 py-3 text-sm sm:px-8 lg:hidden">
                            {navItems.map(({ label, href }) => {
                                const isActive = activeHref === href;

                                return (
                                    <Link
                                        key={label}
                                        href={href}
                                        className={`rounded-full border px-3 py-1 text-xs font-medium uppercase tracking-wide transition ${isActive
                                            ? "border-primary/80 text-primary-foreground"
                                            : "border-surface-muted/70 text-surface-foreground/70 hover:border-primary/80 hover:text-primary-foreground"
                                            }`}
                                    >
                                        {label}
                                    </Link>
                                );
                            })}
                        </nav>
                    </header>
                    <main className="flex-1 px-4 py-8 sm:px-8">{children}</main>
                </div>
            </div>
        </div>
    );
};

export default AdminShell;
