"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  BookOpen,
  ChartBarBig,
  Layers,
  LayoutDashboard,
  MessageSquareText,
  Users,
  ShieldCheck,
  X,
  PanelLeftOpen,
  PanelLeftClose,
  Menu,
} from "lucide-react";
import { useState } from "react";

const AUTH_ROUTE_REGEX = /^\/admin\/auth\//;
interface navigationItem {
  label: string;
  href: string;
  icon: (props: React.ComponentProps<"svg">) => JSX.Element | any;
}
const navigationItems: navigationItem[] = [
  {
    label: "Tổng quan",
    href: "/admin",
    icon: LayoutDashboard,
  },
  {
    label: "Truyện",
    href: "/admin/comics",
    icon: BookOpen,
  },
  {
    label: "Thể loại",
    href: "/admin/categories",
    icon: Layers,
  },
  {
    label: "Chương",
    href: "/admin/chapters",
    icon: ChartBarBig,
  },
  {
    label: "Bình luận",
    href: "/admin/comments",
    icon: MessageSquareText,
  },
  {
    label: "Người dùng",
    href: "/admin/users",
    icon: Users,
  },
  {
    label: "Phân quyền",
    href: "/admin/user-roles",
    icon: ShieldCheck,
  },
];

const AdminSidebar = () => {
  const pathname = usePathname();
  const [collapsed, setCollapsed] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);

  const onClose = () => setMobileOpen(false);
  if (AUTH_ROUTE_REGEX.test(pathname)) {
    return null;
  }

  const isActive = (href: string) => pathname === href || pathname.startsWith(`${href}/`);
  const renderNavigationItem = (item: navigationItem, options?: { onItemClick?: () => void; forceExpanded?: boolean }) => {
    const Icon = item.icon;
    const active = isActive(item.href);
    const hideLabel = collapsed && !options?.forceExpanded;
    return (
      <Link
        key={item.href}
        href={item.href}
        onClick={options?.onItemClick}
        className={`group flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium transition ${active
          ? "bg-primary/15 text-primary"
          : "text-surface-foreground/70 hover:bg-surface-muted/40 hover:text-primary"
          }`}
      >
        <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary/10 text-primary">
          <button
            type="button"
            className="hidden h-9 w-9 items-center justify-center rounded-full border border-surface-muted/80 transition hover:border-primary lg:inline-flex"
            aria-label={item.label}
          >
            <Icon className="h-5 w-5" />
          </button>
        </span>
        <span
          className={`whitespace-nowrap transition-opacity duration-200 ${hideLabel ? "opacity-0" : "opacity-100"}`}
        >
          {item.label}
        </span>
      </Link>
    );
  }
  const renderNavigationItems = (options?: { onItemClick?: () => void; forceExpanded?: boolean }) => (
    <nav className="flex flex-1 flex-col gap-1">
      {navigationItems.map((item) => {
        return renderNavigationItem(item, options);
      })}
    </nav>
  );

  return (
    <>
      <aside
        className={`hidden border-r border-surface-muted/60 bg-surface-muted/20 transition-all duration-300 lg:sticky lg:top-24 lg:flex lg:max-h-[calc(100vh-6rem)] lg:flex-col ${collapsed ? "w-[80px]" : "w-[260px]"
          } overflow-x-hidden`}
      >
        <div className="flex flex-col gap-2 px-3 py-4">
          <div className="relative h-[calc(100vh-200px)] overflow-y-auto">
            {renderNavigationItems()}
          </div>
          <div className={`group flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium transition ${collapsed
            ? "bg-primary/15 text-primary"
            : "text-surface-foreground/70 hover:bg-surface-muted/40 hover:text-primary"
            }`}
            onClick={() => {
              setMobileOpen(true);
              setCollapsed((prev) => !prev);
            }}
          >
            <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary/10 text-primary">
              <button
                type="button"
                className="inline-flex h-5 w-5 items-center justify-center rounded-full border border-surface-muted/80 bg-surface transition hover:border-primary lg:hidden"
                aria-label="Mở menu điều hướng"
              >
                <Menu className="h-5 w-5 text-primary-foreground" />
              </button>
              <button
                type="button"
                className="hidden h-5 w-5 items-center justify-center rounded-full border border-surface-muted/80 bg-surface transition hover:border-primary lg:inline-flex"
                aria-label={collapsed ? "Mở thanh điều hướng" : "Thu gọn thanh điều hướng"}
              >
                {collapsed ? <PanelLeftOpen className="h-5 w-5 text-primary-foreground" /> : <PanelLeftClose className="h-5 w-5 text-primary-foreground" />}
              </button>
            </span>
            <span
              className={`whitespace-nowrap transition-opacity duration-200 ${collapsed ? "hidden" : "opacity-100"}`}
            >
              Thu gọn
            </span>
          </div>
        </div>
      </aside>

      {mobileOpen && (
        <div className="fixed inset-0 z-50 flex lg:hidden">
          <button type="button" className="absolute inset-0 bg-black/40" onClick={onClose} aria-label="Đóng menu" />
          <div className="relative z-10 flex w-72 max-w-[85%] flex-col border-r border-surface-muted/60 bg-surface px-4 pb-6 pt-8 shadow-2xl">
            <div className="mb-6 flex items-center justify-between">
              <span className="text-sm font-semibold uppercase tracking-wide text-primary-foreground">Danh mục</span>
              <button
                type="button"
                onClick={onClose}
                className="inline-flex h-9 w-9 items-center justify-center rounded-full border border-surface-muted/60 text-surface-foreground/70 transition hover:border-primary hover:text-primary"
                aria-label="Thu gọn menu"
              >
                <X className="h-4 w-4" />
              </button>
            </div>
            <div className="flex flex-1 flex-col gap-1">{renderNavigationItems({ onItemClick: onClose, forceExpanded: true })}</div>
            <div className="mt-auto border-t border-surface-muted/60 pt-4 text-xs text-surface-foreground/60">
              © 2025 TruyenCV Admin
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default AdminSidebar;
