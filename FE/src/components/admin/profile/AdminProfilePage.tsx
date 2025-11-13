"use client";

import Image from "next/image";
import { useMemo } from "react";
import clsx from "clsx";
import {
  AlertTriangle,
  BookMarked,
  CheckCircle2,
  CalendarDays,
  Clock3,
  Crown,
  KeyRound,
  Mail,
  Phone,
  RefreshCcw,
  ShieldCheck,
  ShieldX,
  Sparkles,
  UserCircle2
} from "lucide-react";

import { useAdminProfileQuery } from "@services/admin";

const formatNumber = (value?: string | number | null) => {
  if (value === null || value === undefined) {
    return "0";
  }

  const numeric = typeof value === "string" ? Number(value) : value;
  if (Number.isNaN(numeric)) {
    return "0";
  }

  return numeric.toLocaleString("vi-VN");
};

const formatDateTime = (value?: string | null) => {
  if (!value) {
    return "Chưa cập nhật";
  }

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return "Không xác định";
  }

  return date.toLocaleString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit"
  });
};

const AdminProfilePage = () => {
  const { data, isLoading, isError, refetch, isFetching } = useAdminProfileQuery();

  const stats = useMemo(
    () =>
      data
        ? [
            {
              label: "Tổng truyện đã đọc",
              value: formatNumber(data.read_comic_count),
              icon: BookMarked,
              accent: "bg-primary/10 text-primary"
            },
            {
              label: "Tổng chương đã đọc",
              value: formatNumber(data.read_chapter_count),
              icon: Sparkles,
              accent: "bg-amber-500/10 text-amber-400"
            },
            {
              label: "Truyện theo dõi",
              value: formatNumber(data.bookmark_count),
              icon: Crown,
              accent: "bg-emerald-500/10 text-emerald-400"
            },
            {
              label: "Số dư coin",
              value: formatNumber(data.coin),
              icon: ShieldCheck,
              accent: "bg-sky-500/10 text-sky-400"
            },
            {
              label: "Số dư key",
              value: formatNumber(data.key),
              icon: KeyRound,
              accent: "bg-violet-500/10 text-violet-400"
            }
          ]
        : [],
    [data]
  );

  const roles = data?.roles ?? [];
  const permissions = data?.permissions ?? [];
  const isEmailVerified = Boolean(data?.email_verified_at);
  const isBanned = Boolean(data?.is_banned);

  return (
    <div className="space-y-10">
      <header className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <p className="text-xs uppercase tracking-[0.4em] text-primary/70">Hồ sơ quản trị viên</p>
          <h1 className="text-2xl font-semibold text-primary-foreground">Thông tin tài khoản hiện tại</h1>
        </div>
        <button
          type="button"
          onClick={() => refetch()}
          disabled={isFetching}
          className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground disabled:opacity-60"
        >
          <RefreshCcw className={clsx("h-4 w-4", isFetching && "animate-spin")}/>
          Làm mới
        </button>
      </header>

      {isLoading ? (
        <section className="grid gap-6 md:grid-cols-[0.4fr_0.6fr]">
          <div className="space-y-4 rounded-2xl border border-surface-muted bg-surface/70 p-6">
            <div className="h-10 w-32 rounded-full bg-surface-muted/70 animate-pulse" />
            <div className="h-7 w-48 rounded-full bg-surface-muted/70 animate-pulse" />
            <div className="space-y-3">
              <div className="h-4 w-40 rounded-full bg-surface-muted/60 animate-pulse" />
              <div className="h-4 w-56 rounded-full bg-surface-muted/60 animate-pulse" />
            </div>
          </div>
          <div className="grid gap-4 sm:grid-cols-2">
            {Array.from({ length: 4 }).map((_, index) => (
              <div
                key={index}
                className="h-28 rounded-2xl border border-surface-muted bg-surface/60 animate-pulse"
              />
            ))}
          </div>
        </section>
      ) : isError ? (
        <section className="rounded-2xl border border-red-500/40 bg-red-500/10 p-6 text-sm text-red-100">
          <div className="flex items-center gap-3">
            <AlertTriangle className="h-5 w-5" />
            <div>
              <p className="font-semibold uppercase tracking-wide">Không thể tải hồ sơ</p>
              <p className="text-red-200">Vui lòng kiểm tra kết nối hoặc thử lại sau.</p>
            </div>
          </div>
        </section>
      ) : data ? (
        <>
          <section className="grid gap-6 md:grid-cols-[0.45fr_0.55fr]">
            <article className="space-y-6 rounded-2xl border border-surface-muted bg-surface/80 p-6 shadow">
              <div className="flex items-start gap-4">
                <div className="flex h-16 w-16 items-center justify-center overflow-hidden rounded-2xl bg-primary/15 text-primary">
                  {data.avatar ? (
                    <Image
                      src={data.avatar}
                      alt={data.name || "Avatar"}
                      width={64}
                      height={64}
                      className="h-full w-full object-cover"
                    />
                  ) : (
                    <UserCircle2 className="h-8 w-8" />
                  )}
                </div>
                <div className="flex-1 space-y-2">
                  <h2 className="text-xl font-semibold text-primary-foreground">{data.name || "Chưa cập nhật"}</h2>
                  <p className="text-xs uppercase tracking-[0.3em] text-surface-foreground/50">ID: {data.id}</p>
                  <div className="space-y-1 text-sm text-surface-foreground/70">
                    <p className="flex items-center gap-2">
                      <Mail className="h-4 w-4 text-primary/70" />
                      {data.email || "Chưa cập nhật"}
                    </p>
                    <p className="flex items-center gap-2">
                      <Phone className="h-4 w-4 text-primary/70" />
                      {data.phone || "Chưa cập nhật"}
                    </p>
                    <p className="flex items-center gap-2">
                      <Clock3 className="h-4 w-4 text-primary/70" />
                      Tạo lúc: {formatDateTime(data.created_at)}
                    </p>
                  </div>
                </div>
              </div>

              <div className="grid gap-3 sm:grid-cols-2">
                <div className="rounded-2xl border border-surface-muted/70 bg-surface px-4 py-3 text-sm">
                  <p className="text-xs uppercase tracking-wide text-surface-foreground/60">Trạng thái email</p>
                  <div className="mt-2 flex items-center gap-2 text-primary-foreground">
                    {isEmailVerified ? (
                      <CheckCircle2 className="h-4 w-4 text-emerald-400" />
                    ) : (
                      <ShieldX className="h-4 w-4 text-amber-400" />
                    )}
                    <span>
                      {isEmailVerified ? `Đã xác minh (${formatDateTime(data.email_verified_at)})` : "Chưa xác minh"}
                    </span>
                  </div>
                </div>
                <div className="rounded-2xl border border-surface-muted/70 bg-surface px-4 py-3 text-sm">
                  <p className="text-xs uppercase tracking-wide text-surface-foreground/60">Trạng thái tài khoản</p>
                  <div className="mt-2 flex items-center gap-2 text-primary-foreground">
                    {isBanned ? (
                      <ShieldX className="h-4 w-4 text-red-400" />
                    ) : (
                      <ShieldCheck className="h-4 w-4 text-emerald-400" />
                    )}
                    <span>
                      {isBanned ? `Đã khóa (${formatDateTime(data.banned_at)})` : "Đang hoạt động"}
                    </span>
                  </div>
                </div>
              </div>
            </article>

            <article className="rounded-2xl border border-surface-muted bg-surface/70 p-6">
              <h3 className="text-sm font-semibold uppercase tracking-[0.3em] text-primary/80">Chỉ số tài khoản</h3>
              <div className="mt-5 grid gap-4 sm:grid-cols-2">
                {stats.map(({ label, value, icon: Icon, accent }) => (
                  <div
                    key={label}
                    className="flex items-center gap-3 rounded-2xl border border-surface-muted/70 bg-surface px-4 py-4 shadow-sm"
                  >
                    <span className={clsx("flex h-12 w-12 items-center justify-center rounded-xl", accent)}>
                      <Icon className="h-5 w-5" />
                    </span>
                    <div>
                      <p className="text-lg font-semibold text-primary-foreground">{value}</p>
                      <p className="text-xs uppercase tracking-wide text-surface-foreground/60">{label}</p>
                    </div>
                  </div>
                ))}
              </div>
            </article>
          </section>

          <section className="grid gap-6 lg:grid-cols-2">
            <article className="rounded-2xl border border-surface-muted bg-surface/70 p-6">
              <header className="mb-4 flex items-center gap-2 text-primary-foreground">
                <ShieldCheck className="h-5 w-5" />
                <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Vai trò quản trị</h3>
              </header>
              {roles.length === 0 ? (
                <p className="text-sm text-surface-foreground/60">Tài khoản chưa được gán vai trò nào.</p>
              ) : (
                <div className="flex flex-wrap gap-2">
                  {roles.map((role) => (
                    <span
                      key={role}
                      className="rounded-full border border-primary/40 bg-primary/10 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-primary"
                    >
                      {role}
                    </span>
                  ))}
                </div>
              )}
            </article>

            <article className="rounded-2xl border border-surface-muted bg-surface/70 p-6">
              <header className="mb-4 flex items-center gap-2 text-primary-foreground">
                <Sparkles className="h-5 w-5" />
                <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Quyền hạn</h3>
              </header>
              {permissions.length === 0 ? (
                <p className="text-sm text-surface-foreground/60">Chưa có quyền hạn cụ thể.</p>
              ) : (
                <div className="flex flex-wrap gap-2">
                  {permissions.map((permission) => (
                    <span
                      key={permission}
                      className="rounded-full border border-emerald-400/50 bg-emerald-500/10 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-emerald-300"
                    >
                      {permission}
                    </span>
                  ))}
                </div>
              )}
            </article>
          </section>

          <section className="rounded-2xl border border-surface-muted bg-surface/70 p-6">
            <header className="flex items-center gap-2 text-primary-foreground">
              <CalendarDays className="h-5 w-5" />
              <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Mốc thời gian</h3>
            </header>
            <div className="mt-5 grid gap-4 md:grid-cols-3">
              <div className="rounded-2xl border border-surface-muted/70 bg-surface px-4 py-4 text-sm text-surface-foreground/70">
                <p className="text-xs uppercase tracking-wide text-surface-foreground/40">Tài khoản tạo lúc</p>
                <p className="mt-2 text-primary-foreground">{formatDateTime(data.created_at)}</p>
              </div>
              <div className="rounded-2xl border border-surface-muted/70 bg-surface px-4 py-4 text-sm text-surface-foreground/70">
                <p className="text-xs uppercase tracking-wide text-surface-foreground/40">Cập nhật lần cuối</p>
                <p className="mt-2 text-primary-foreground">{formatDateTime(data.updated_at)}</p>
              </div>
              <div className="rounded-2xl border border-surface-muted/70 bg-surface px-4 py-4 text-sm text-surface-foreground/70">
                <p className="text-xs uppercase tracking-wide text-surface-foreground/40">Gói đăng ký đang dùng</p>
                <p className="mt-2 text-primary-foreground">
                  {data.active_subscription_name || "Chưa đăng ký"}
                </p>
              </div>
            </div>
          </section>
        </>
      ) : null}
    </div>
  );
};

export default AdminProfilePage;
