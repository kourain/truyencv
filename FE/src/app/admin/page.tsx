"use client";

import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import {
  Activity,
  AlertCircle,
  ArrowRight,
  BookOpen,
  CalendarClock,
  LayoutDashboard,
  Loader2,
  ShieldCheck,
  Sparkles,
  Users
} from "lucide-react";

import { ComicStatus } from "@const/comic-status";
import { formatNumber, formatRelativeTime } from "@helpers/format";
import { fetchPing } from "@services/health.service";
import { fetchComicsWithFallback } from "@services/admin/comic.service";
import { fetchUsersWithFallback } from "@services/admin/user.service";
import { fetchComicCategoriesWithFallback } from "@services/admin/comic-category.service";
import { ComicResponse } from "@types/comic";

type DashboardData = {
  comics: ComicResponse[];
  users: UserResponse[];
  categories: ComicCategoryResponse[];
  isMock?: boolean;
};

const comicStatusLabel: Record<ComicStatus, string> = {
  [ComicStatus.Continuing]: "Đang cập nhật",
  [ComicStatus.Paused]: "Tạm dừng",
  [ComicStatus.Stopped]: "Ngừng xuất bản",
  [ComicStatus.Completed]: "Đã hoàn thành"
};

const fetchDashboardData = async (): Promise<DashboardData> => {
  const [comicResult, userResult, categoryResult] = await Promise.all([
    fetchComicsWithFallback({ limit: 50 }),
    fetchUsersWithFallback({ limit: 20 }),
    fetchComicCategoriesWithFallback({ limit: 30 })
  ]);

  const isMock = comicResult.isMock || userResult.isMock || categoryResult.isMock;

  return {
    comics: comicResult.data,
    users: userResult.data,
    categories: categoryResult.data,
    isMock
  };
};

const AdminDashboardPage = () => {
  const { data, isLoading, isFetching, refetch } = useQuery<DashboardData>({
    queryKey: ["admin-dashboard"],
    queryFn: fetchDashboardData,
    staleTime: 60_000
  });

  const { data: pingData, isLoading: isPingLoading, isError: isPingError, refetch: refetchPing } = useQuery({
    queryKey: ["admin-dashboard", "ping"],
    queryFn: fetchPing,
    refetchInterval: 60_000
  });

  const comics = data?.comics ?? [];
  const users = data?.users ?? [];
  const categories = data?.categories ?? [];

  const overview = useMemo(() => {
    const totalComics = comics.length;
    const continuing = comics.filter((comic) => comic.status === ComicStatus.Continuing).length;
    const completed = comics.filter((comic) => comic.status === ComicStatus.Completed).length;
    const averageRate =
      totalComics === 0
        ? 0
        : comics.reduce((sum, comic) => sum + (Number.isFinite(comic.rate) ? comic.rate : 0), 0) / totalComics;

    const totalUsers = users.length;
    const sevenDaysAgo = Date.now() - 7 * 24 * 60 * 60 * 1000;
    const newUsers = users.filter((user) => new Date(user.created_at).getTime() >= sevenDaysAgo).length;

    const categoriesCount = categories.length;

    return {
      totalComics,
      continuing,
      completed,
      averageRate,
      totalUsers,
      newUsers,
      categoriesCount
    };
  }, [categories, comics, users]);

  const topComics = useMemo(() => {
    return [...comics]
      .sort((a, b) => {
        const rateDiff = (b.rate ?? 0) - (a.rate ?? 0);
        if (Math.abs(rateDiff) > 0.01) {
          return rateDiff;
        }

        return new Date(b.updated_at).getTime() - new Date(a.updated_at).getTime();
      })
      .slice(0, 5);
  }, [comics]);

  const recentUsers = useMemo(() => {
    return [...users]
      .sort((a, b) => new Date(b.created_at).getTime() - new Date(a.created_at).getTime())
      .slice(0, 6);
  }, [users]);

  const categoryShowcase = useMemo(() => {
    return categories.slice(0, 8);
  }, [categories]);

  const metricCards = useMemo(
    () => [
      {
        label: "Tổng số truyện",
        value: formatNumber(overview.totalComics),
        description: `${formatNumber(overview.categoriesCount)} nhóm thể loại được quản lý`,
        icon: BookOpen
      },
      {
        label: "Đang cập nhật",
        value: formatNumber(overview.continuing),
        description:
          overview.totalComics > 0
            ? `${Math.round((overview.continuing / overview.totalComics) * 100)}% tổng số truyện`
            : "Chưa có dữ liệu",
        icon: Activity
      },
      {
        label: "Truyện hoàn thành",
        value: formatNumber(overview.completed),
        description:
          overview.totalComics > 0
            ? `${Math.round((overview.completed / overview.totalComics) * 100)}% kho truyện`
            : "Đang chờ cập nhật",
        icon: Sparkles
      },
      {
        label: "Người dùng mới (7 ngày)",
        value: formatNumber(overview.newUsers),
        description: `${formatNumber(overview.totalUsers)} người dùng được phân quyền`,
        icon: Users
      }
    ],
    [overview]
  );

  const handleRefetch = () => {
    refetch();
    refetchPing();
  };

  const showLoading = isLoading && comics.length === 0;

  return (
    <div className="space-y-10">
      <section className="space-y-4">
        <header className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
          <div>
            <p className="flex items-center gap-2 text-xs uppercase tracking-[0.4em] text-primary/80">
              <LayoutDashboard className="h-3.5 w-3.5" />
              Bảng điều khiển tổng quan
            </p>
            <h2 className="mt-2 text-2xl font-semibold text-primary-foreground">Tình trạng hệ thống và nội dung</h2>
            <p className="mt-1 text-sm text-surface-foreground/60">
              Theo dõi nhanh kho truyện, danh mục và hoạt động người dùng với dữ liệu trực tiếp từ API quản trị.
            </p>
          </div>
          <button
            type="button"
            onClick={handleRefetch}
            className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary/15 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/25"
            disabled={isFetching}
          >
            {isFetching ? <Loader2 className="h-3.5 w-3.5 animate-spin" /> : <CalendarClock className="h-4 w-4" />}
            {isFetching ? "Đang làm mới" : "Làm mới số liệu"}
          </button>
        </header>

        {data?.isMock && (
          <div className="rounded-xl border border-amber-400/50 bg-amber-100/10 px-4 py-3 text-sm text-amber-500">
            Hiện đang hiển thị dữ liệu minh họa vì API trả về lỗi. Hãy kiểm tra lại cấu hình backend nếu cần.
          </div>
        )}
      </section>

      <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        {metricCards.map(({ label, value, description, icon: Icon }) => (
          <article key={label} className="rounded-2xl border border-surface-muted bg-surface/60 p-5 shadow-glow">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-xs uppercase tracking-wide text-surface-foreground/60">{label}</p>
                <p className="mt-3 text-3xl font-semibold text-primary-foreground">{showLoading ? "--" : value}</p>
              </div>
              <span className="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10 text-primary">
                <Icon className="h-6 w-6" />
              </span>
            </div>
            <p className="mt-4 text-xs text-surface-foreground/70">{showLoading ? "Đang tải dữ liệu..." : description}</p>
          </article>
        ))}
      </section>

      <section className="grid gap-6 xl:grid-cols-[1.15fr_0.85fr]">
        <div className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
          <header className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <p className="text-xs uppercase tracking-[0.3em] text-primary/70">Top truyện nổi bật</p>
              <h3 className="text-xl font-semibold text-primary-foreground">Xếp hạng theo điểm đánh giá</h3>
            </div>
            <div className="text-xs text-surface-foreground/60">
              Cập nhật từ {showLoading ? "--" : `${formatNumber(topComics.length)} mục`} mới nhất
            </div>
          </header>

          <div className="mt-6 space-y-4">
            {showLoading && (
              <div className="space-y-3">
                {Array.from({ length: 4 }).map((_, index) => (
                  <div key={index} className="h-20 animate-pulse rounded-2xl bg-surface-muted/40" />
                ))}
              </div>
            )}

            {!showLoading && topComics.length === 0 && (
              <div className="rounded-2xl border border-dashed border-surface-muted/70 bg-surface px-4 py-6 text-center text-sm text-surface-foreground/60">
                Chưa có dữ liệu truyện. Hãy thêm truyện mới để bắt đầu theo dõi.
              </div>
            )}

            {!showLoading &&
              topComics.map((comic) => (
                <article
                  key={comic.id}
                  className="rounded-2xl border border-surface-muted/60 bg-surface px-4 py-4 transition hover:border-primary/60"
                >
                  <div className="flex flex-wrap items-center justify-between gap-4">
                    <div>
                      <h4 className="text-lg font-semibold text-primary-foreground">{comic.name}</h4>
                      <p className="text-sm text-surface-foreground/70">
                        {comic.author} • {comicStatusLabel[comic.status]}
                      </p>
                    </div>
                    <div className="text-right">
                      <p className="text-xs uppercase tracking-wide text-surface-foreground/60">Điểm đánh giá</p>
                      <p className="text-base font-semibold text-primary-foreground">{comic.rate?.toFixed(2) ?? "--"}</p>
                    </div>
                  </div>
                  <div className="mt-4 flex flex-wrap items-center gap-4 text-xs text-surface-foreground/60">
                    <span>{formatNumber(comic.chap_count)} chương</span>
                    <span>•</span>
                    <span>Cập nhật {formatRelativeTime(comic.updated_at)}</span>
                    <span>•</span>
                    <span>Slug: {comic.slug}</span>
                  </div>
                </article>
              ))}
          </div>
        </div>

        <div className="space-y-6">
          <article className="rounded-2xl border border-primary/40 bg-primary/10 p-6">
            <header className="flex items-center gap-3">
              <Sparkles className="h-5 w-5 text-primary" />
              <div>
                <h3 className="text-lg font-semibold text-primary-foreground">Gợi ý hành động</h3>
                <p className="text-xs text-primary/80">Tối ưu dựa trên dữ liệu mới nhất</p>
              </div>
            </header>
            <ul className="mt-4 space-y-3 text-sm text-surface-foreground/80">
              <li>• Ưu tiên cập nhật thêm chương cho {overview.continuing > 0 ? `${formatNumber(overview.continuing)} truyện đang phát hành.` : "các truyện mới được thêm."}</li>
              <li>• Xem lại chất lượng nội dung của nhóm truyện có điểm đánh giá dưới 3.5.</li>
              <li>• Duyệt phân quyền cho {overview.newUsers > 0 ? `${formatNumber(overview.newUsers)} người dùng mới` : "người dùng mới"} trong 7 ngày gần nhất.</li>
            </ul>
          </article>

          <article className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
            <header className="flex items-center gap-2 text-primary-foreground">
              <Users className="h-5 w-5" />
              <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Người dùng mới nhất</h3>
            </header>
            <div className="mt-4 space-y-3">
              {showLoading && (
                <div className="space-y-3">
                  {Array.from({ length: 4 }).map((_, index) => (
                    <div key={index} className="h-16 animate-pulse rounded-xl bg-surface-muted/40" />
                  ))}
                </div>
              )}

              {!showLoading && recentUsers.length === 0 && (
                <div className="rounded-xl border border-dashed border-surface-muted/60 bg-surface px-4 py-6 text-center text-sm text-surface-foreground/60">
                  Chưa có người dùng mới trong danh sách hiển thị.
                </div>
              )}

              {!showLoading &&
                recentUsers.map((user) => (
                  <div
                    key={user.id}
                    className="flex flex-wrap items-center justify-between gap-3 rounded-xl border border-surface-muted/70 bg-surface px-4 py-3"
                  >
                    <div>
                      <p className="text-sm font-semibold text-primary-foreground">{user.full_name || user.name}</p>
                      <p className="text-xs text-surface-foreground/60">{user.email}</p>
                    </div>
                    <p className="text-xs text-surface-foreground/60">{formatRelativeTime(user.created_at)}</p>
                  </div>
                ))}
            </div>
          </article>
        </div>
      </section>

      <section className="grid gap-6 lg:grid-cols-[1.05fr_0.95fr]">
        <article className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
          <header className="flex items-center justify-between">
            <div>
              <p className="text-xs uppercase tracking-[0.3em] text-primary/70">Danh mục truyện</p>
              <h3 className="text-xl font-semibold text-primary-foreground">Phân bổ theo thể loại</h3>
            </div>
            <div className="text-xs text-surface-foreground/60">
              Tổng cộng {formatNumber(categories.length)} thể loại
            </div>
          </header>
          <div className="mt-6 space-y-3">
            {showLoading && (
              <div className="space-y-3">
                {Array.from({ length: 5 }).map((_, index) => (
                  <div key={index} className="h-14 animate-pulse rounded-xl bg-surface-muted/40" />
                ))}
              </div>
            )}

            {!showLoading && categoryShowcase.length === 0 && (
              <div className="rounded-xl border border-dashed border-surface-muted/60 bg-surface px-4 py-6 text-center text-sm text-surface-foreground/60">
                Chưa có danh mục nào được tạo.
              </div>
            )}

            {!showLoading &&
              categoryShowcase.map((category) => (
                <div
                  key={category.id}
                  className="flex items-center justify-between rounded-xl border border-surface-muted/70 bg-surface px-4 py-3"
                >
                  <div>
                    <p className="text-sm font-semibold text-primary-foreground">{category.name}</p>
                    <p className="text-xs text-surface-foreground/60">Tạo {formatRelativeTime(category.created_at)}</p>
                  </div>
                  <ArrowRight className="h-4 w-4 text-surface-foreground/40" />
                </div>
              ))}
          </div>
        </article>

        <article className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
          <header className="flex items-center gap-2 text-primary-foreground">
            <ShieldCheck className="h-5 w-5" />
            <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Tình trạng dịch vụ</h3>
          </header>
          <div className="mt-5 space-y-3">
            <div className="flex items-center justify-between rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
              <div className="flex items-center gap-3">
                <span className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-primary">
                  <Activity className="h-5 w-5" />
                </span>
                <div>
                  <p className="text-sm font-semibold text-primary-foreground">API quản trị</p>
                  <p className="text-xs text-surface-foreground/60">
                    {isPingLoading ? "Đang kiểm tra kết nối..." : isPingError ? "Không phản hồi" : pingData?.message ?? "Hoạt động ổn định"}
                  </p>
                </div>
              </div>
              <span
                className={`text-xs font-semibold uppercase tracking-wide ${
                  isPingError ? "text-red-400" : "text-emerald-400"
                }`}
              >
                {isPingError ? "Gián đoạn" : "Đang chạy"}
              </span>
            </div>

            <div className="flex items-center justify-between rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
              <div className="flex items-center gap-3">
                <span className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-primary">
                  <BookOpen className="h-5 w-5" />
                </span>
                <div>
                  <p className="text-sm font-semibold text-primary-foreground">Kho truyện</p>
                  <p className="text-xs text-surface-foreground/60">
                    {overview.totalComics > 0
                      ? `${formatNumber(overview.totalComics)} truyện đang theo dõi`
                      : "Chưa có dữ liệu truyện"}
                  </p>
                </div>
              </div>
              <span className="text-xs font-semibold uppercase tracking-wide text-primary/70">
                {overview.totalComics > 0 ? "Đầy đủ" : "Trống"}
              </span>
            </div>

            <div className="flex items-center justify-between rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
              <div className="flex items-center gap-3">
                <span className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-primary">
                  <Users className="h-5 w-5" />
                </span>
                <div>
                  <p className="text-sm font-semibold text-primary-foreground">Quản trị viên</p>
                  <p className="text-xs text-surface-foreground/60">
                    {overview.totalUsers > 0
                      ? `${formatNumber(overview.totalUsers)} tài khoản có quyền truy cập`
                      : "Chưa có ai được phân quyền"}
                  </p>
                </div>
              </div>
              <span className="text-xs font-semibold uppercase tracking-wide text-primary/70">
                {overview.totalUsers > 0 ? "Hoạt động" : "Đang chờ"}
              </span>
            </div>

            {isPingError && (
              <div className="flex items-start gap-3 rounded-xl border border-red-500/50 bg-red-500/10 px-4 py-3 text-sm text-red-400">
                <AlertCircle className="mt-0.5 h-4 w-4 flex-shrink-0" />
                Không thể kết nối tới API. Vui lòng kiểm tra lại dịch vụ backend hoặc thử làm mới.
              </div>
            )}
          </div>
        </article>
      </section>
    </div>
  );
};

export default AdminDashboardPage;
