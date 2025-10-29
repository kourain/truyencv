"use client";

import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { Activity, BookOpen, MessageCircle, ShieldCheck, Sparkles, Users } from "lucide-react";

import DashboardHeaderSection from "./dashboard-header-section";
import MetricCardsSection from "./metric-cards-section";
import TopComicsSection from "./top-comics-section";
import InsightsSidebar from "./insights-sidebar";
import CategorySection from "./category-section";
import ServiceStatusSection from "./service-status-section";
import type { MetricCardConfig, OverviewStats } from "@components/admin/types";

import { formatNumber } from "@helpers/format";
import { fetchAdminDashboardOverview } from "@services/admin/dashboard.service";
import type { AdminDashboardOverview } from "../../../types/admin-dashboard";
import { usePingQuery } from "@hooks/usePingQuery";

const AdminDashboardPage = () => {
  const { data, isLoading, isFetching, isError, error, refetch } = useQuery<AdminDashboardOverview>({
    queryKey: ["admin-dashboard", "overview"],
    queryFn: fetchAdminDashboardOverview,
    staleTime: 60_000,
    retry: 1
  });

  const { data: pingData, isLoading: isPingLoading, isError: isPingError, refetch: refetchPing } = usePingQuery();

  const parseMetricCount = (value: string | number | null | undefined) => {
    if (value === undefined || value === null) {
      return 0;
    }

    if (typeof value === "number") {
      return value;
    }

    const parsed = Number(value);
    return Number.isNaN(parsed) ? 0 : parsed;
  };

  const overview = useMemo<OverviewStats>(
    () => {
      const metrics = data?.metrics ?? {
        total_comics: 0,
        continuing_comics: 0,
        completed_comics: 0,
        total_users: 0,
        new_users_7_days: 0,
        categories_count: 0,
        total_chapters: "0",
        total_comments: "0",
        total_bookmarks: "0",
        active_admins: 0
      };
      return {
        totalComics: metrics.total_comics,
        continuing: metrics.continuing_comics,
        completed: metrics.completed_comics,
        totalUsers: metrics.total_users,
        newUsers: metrics.new_users_7_days,
        categoriesCount: metrics.categories_count,
        totalChapters: parseMetricCount(metrics.total_chapters),
        totalComments: parseMetricCount(metrics.total_comments),
        totalBookmarks: parseMetricCount(metrics.total_bookmarks),
        activeAdmins: metrics.active_admins
      };
    }, [data]
  );

  const topComics = useMemo(() => {
    const comics = data?.top_comics ?? [];

    return [...comics]
      .sort((a, b) => {
        const rateDiff = (b.rate ?? 0) - (a.rate ?? 0);
        if (Math.abs(rateDiff) > 0.01) {
          return rateDiff;
        }

        return new Date(b.updated_at).getTime() - new Date(a.updated_at).getTime();
      })
      .slice(0, 5);
  }, [data?.top_comics]);

  const recentUsers = useMemo(() => {
    const users = data?.recent_users ?? [];
    return [...users]
      .sort((a, b) => new Date(b.created_at).getTime() - new Date(a.created_at).getTime())
      .slice(0, 6);
  }, [data?.recent_users]);

  const categories = useMemo(() => data?.category_highlights ?? [], [data?.category_highlights]);
  const categoryShowcase = useMemo(() => categories.slice(0, 8), [categories]);

  const metricCards = useMemo<MetricCardConfig[]>(
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
      },
      {
        label: "Bình luận hệ thống",
        value: formatNumber(overview.totalComments),
        description: overview.totalComments > 0 ? "Tổng số phản hồi được ghi nhận" : "Chưa phát sinh bình luận",
        icon: MessageCircle
      },
      {
        label: "Quản trị viên hoạt động",
        value: formatNumber(overview.activeAdmins),
        description: overview.activeAdmins > 0 ? "Số tài khoản giữ vai trò quản trị" : "Chưa có quản trị viên",
        icon: ShieldCheck
      }
    ],
    [overview]
  );

  const handleRefetch = () => {
    refetch();
    refetchPing();
  };

  const showLoading = isLoading && !data;
  const errorMessage = isError ? (error instanceof Error ? error.message : "Không thể tải dữ liệu tổng quan.") : null;

  return (
    <div className="space-y-10">
      {errorMessage && (
        <div className="rounded-xl border border-red-500/40 bg-red-500/10 px-4 py-3 text-sm text-red-200">
          {errorMessage}
        </div>
      )}

      <DashboardHeaderSection
        isFetching={isFetching}
        onRefetch={handleRefetch}
        isMock={Boolean(data?.is_mock)}
      />

      <MetricCardsSection metricCards={metricCards} showLoading={showLoading} />

      <section className="grid gap-6 xl:grid-cols-[1.15fr_0.85fr]">
        <TopComicsSection topComics={topComics} showLoading={showLoading} />
        <InsightsSidebar overview={overview} recentUsers={recentUsers} showLoading={showLoading} />
      </section>

      <section className="grid gap-6 lg:grid-cols-[1.05fr_0.95fr]">
        <CategorySection
          categories={categories}
          categoryShowcase={categoryShowcase}
          showLoading={showLoading}
        />
        <ServiceStatusSection
          overview={overview}
          pingData={pingData}
          isPingLoading={isPingLoading}
          isPingError={isPingError}
        />
      </section>
    </div>
  );
};

export default AdminDashboardPage;
