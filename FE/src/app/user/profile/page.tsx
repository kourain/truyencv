"use client";

import { useEffect, useState } from "react";
import { Loader2, ShieldCheck } from "lucide-react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { AxiosError } from "axios";
import { useRouter, useSearchParams } from "next/navigation";

import { useToast } from "@components/providers/ToastProvider";
import ProfileHeader from "@components/user/profile/ProfileHeader";
import ProfileStatsGrid from "@components/user/profile/ProfileStatsGrid";
import EmailVerificationCard from "@components/user/profile/EmailVerificationCard";
import ChangePasswordForm from "@components/user/profile/ChangePasswordForm";
import {
  useUserProfileQuery,
  verifyEmail,
  useUserCoinHistoryQuery,
  useUserPaymentHistoryQuery,
  useUserKeyHistoryQuery,
  useUserReadHistoryQuery,
  useUserCommentHistoryQuery,
} from "@services/user/profile.service";
import { formatNumber, formatRelativeTime } from "@helpers/format";

const ActivityCard = ({ label, value }: { label: string; value: number }) => {
  return (
    <div className="flex flex-col gap-2 rounded-3xl border border-surface-muted/70 bg-surface/70 p-6 shadow-lg">
      <span className="text-sm font-semibold uppercase tracking-wide text-surface-foreground/60">{label}</span>
      <span className="text-3xl font-semibold text-primary-foreground">{formatNumber(value)}</span>
    </div>
  );
};

const LoadingState = () => (
  <div className="flex w-full justify-center py-12">
    <Loader2 className="h-8 w-8 animate-spin text-primary" />
  </div>
);

const ErrorState = () => (
  <div className="rounded-3xl border border-red-500/40 bg-red-500/10 p-6 text-center text-sm text-red-200">
    Không thể tải thông tin tài khoản. Vui lòng thử lại sau.
  </div>
);

const InlineError = ({ message }: { message: string }) => (
  <div className="rounded-3xl border border-red-500/40 bg-red-500/10 p-4 text-sm text-red-200">
    {message}
  </div>
);

const EmptyState = ({ message }: { message: string }) => (
  <div className="rounded-3xl border border-surface-muted/60 bg-surface/70 p-6 text-center text-sm text-surface-foreground/60">
    {message}
  </div>
);

type MainTabKey = "info" | "payment" | "read" | "comment";
type PaymentTabKey = "coin" | "deposit" | "ticket";

const mainTabs: Array<{ key: MainTabKey; label: string }> = [
  { key: "info", label: "Thông tin" },
  { key: "payment", label: "Lịch sử thanh toán" },
  { key: "read", label: "Lịch sử đọc" },
  { key: "comment", label: "Lịch sử bình luận" },
];

const paymentTabs: Array<{ key: PaymentTabKey; label: string }> = [
  { key: "coin", label: "Coin" },
  { key: "deposit", label: "Nạp" },
  { key: "ticket", label: "Vé" },
];

const paymentTabPaths: Record<PaymentTabKey, string> = {
  coin: "/user/profile/history_coin",
  deposit: "/user/profile/history_deposit",
  ticket: "/user/profile/history_ticket",
};

const mainTabDefaultPaths: Record<MainTabKey, string> = {
  info: "/user/profile",
  payment: paymentTabPaths.coin,
  read: "/user/profile/history_read",
  comment: "/user/profile/history_comment",
};

const historyStatusStyles: Record<HistoryStatus, { label: string; className: string }> = {
  [HistoryStatus.Add]: {
    label: "Nhận",
    className: "bg-emerald-500/15 text-emerald-300",
  },
  [HistoryStatus.Use]: {
    label: "Sử dụng",
    className: "bg-rose-500/15 text-rose-300",
  },
};

const defaultHistoryStatusStyle = {
  label: "Khác",
  className: "bg-surface-muted/70 text-surface-foreground/70",
};

const resolveTabsFromSection = (section: string | null): { main: MainTabKey; payment?: PaymentTabKey } => {
  switch (section) {
    case "info":
      return { main: "info", payment: "coin" };
    case "history_coin":
      return { main: "payment", payment: "coin" };
    case "history_deposit":
      return { main: "payment", payment: "deposit" };
    case "history_ticket":
      return { main: "payment", payment: "ticket" };
    case "history_read":
      return { main: "read" };
    case "history_comment":
      return { main: "comment" };
    default:
      return { main: "info", payment: "coin" };
  }
};

const formatDateTime = (isoString: string) => {
  return new Date(isoString).toLocaleString("vi-VN", {
    hour: "2-digit",
    minute: "2-digit",
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
};

const formatCurrency = (value: number) => {
  return formatNumber(value, { style: "currency", currency: "VND" });
};

const UserProfilePage = () => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const queryClient = useQueryClient();
  const { data: profile, isLoading, isError } = useUserProfileQuery();
  const [verifyError, setVerifyError] = useState<string | null>(null);
  const [activeMainTab, setActiveMainTab] = useState<MainTabKey>("info");
  const [activePaymentTab, setActivePaymentTab] = useState<PaymentTabKey>("coin");
  const { pushToast } = useToast();
  const coinHistoryQuery = useUserCoinHistoryQuery();
  const paymentHistoryQuery = useUserPaymentHistoryQuery();
  const keyHistoryQuery = useUserKeyHistoryQuery();
  const readHistoryQuery = useUserReadHistoryQuery(50);
  const commentHistoryQuery = useUserCommentHistoryQuery();

  const verifyEmailMutation = useMutation({
    mutationFn: verifyEmail,
    onSuccess: (data) => {
      queryClient.setQueryData(["user-profile"], data);
      setVerifyError(null);
      pushToast({
        title: "Email đã được xác thực",
        description: "Cảm ơn bạn đã xác nhận địa chỉ email.",
        variant: "success",
      });
    },
    onError: (error: unknown) => {
      if (typeof error === "object" && error !== null && "response" in error) {
        const apiError = error as AxiosError<{ message?: string }>;
        const message = apiError.response?.data?.message;
        const fallbackMessage = "Không thể gửi yêu cầu xác thực email. Vui lòng thử lại sau.";
        setVerifyError(message ?? fallbackMessage);
        pushToast({
          title: "Xác thực email thất bại",
          description: message ?? fallbackMessage,
          variant: "error",
        });
      } else {
        const fallbackMessage = "Không thể gửi yêu cầu xác thực email. Vui lòng thử lại sau.";
        setVerifyError(fallbackMessage);
        pushToast({
          title: "Xác thực email thất bại",
          description: fallbackMessage,
          variant: "error",
        });
      }
    },
  });

  const sectionParam = searchParams.get("section");

  useEffect(() => {
    const resolved = resolveTabsFromSection(sectionParam);
    setActiveMainTab(resolved.main);
    if (resolved.main === "payment") {
      setActivePaymentTab(resolved.payment ?? "coin");
    } else {
      setActivePaymentTab("coin");
    }
  }, [sectionParam]);

  const handleMainTabChange = (tab: MainTabKey) => {
    if (tab === "payment") {
      const nextPaymentTab = activePaymentTab ?? "coin";
      setActiveMainTab("payment");
      setActivePaymentTab(nextPaymentTab);
      router.push(paymentTabPaths[nextPaymentTab]);
      return;
    }

    setActiveMainTab(tab);
    setActivePaymentTab("coin");
    router.push(mainTabDefaultPaths[tab]);
  };

  const handlePaymentTabChange = (tab: PaymentTabKey) => {
    setActiveMainTab("payment");
    setActivePaymentTab(tab);
    router.push(paymentTabPaths[tab]);
  };

  const renderCoinHistory = () => {
    if (coinHistoryQuery.isLoading) {
      return <LoadingState />;
    }

    if (coinHistoryQuery.isError) {
      return <InlineError message="Không thể tải lịch sử coin." />;
    }

    const entries = coinHistoryQuery.data ?? [];
    if (entries.length === 0) {
      return <EmptyState message="Bạn chưa có giao dịch coin nào." />;
    }

    return (
      <ul className="space-y-3">
        {entries.map((item) => {
          const meta = historyStatusStyles[item.status as HistoryStatus] ?? defaultHistoryStatusStyle;
          return (
            <li key={item.id} className="flex flex-col gap-2 rounded-2xl border border-surface-muted/60 bg-surface/80 p-4">
              <div className="flex items-center justify-between gap-3">
                <span className="text-base font-semibold text-primary-foreground">{formatNumber(item.coin)} coin</span>
                <span className={`rounded-full px-3 py-1 text-xs font-semibold ${meta.className}`}>{meta.label}</span>
              </div>
              {item.reason && <p className="text-sm text-surface-foreground/70">{item.reason}</p>}
              <p className="text-xs text-surface-foreground/50">
                {formatRelativeTime(item.created_at)} · {formatDateTime(item.created_at)}
              </p>
            </li>
          );
        })}
      </ul>
    );
  };

  const renderKeyHistory = () => {
    if (keyHistoryQuery.isLoading) {
      return <LoadingState />;
    }

    if (keyHistoryQuery.isError) {
      return <InlineError message="Không thể tải lịch sử vé." />;
    }

    const entries = keyHistoryQuery.data ?? [];
    if (entries.length === 0) {
      return <EmptyState message="Bạn chưa sử dụng vé nào." />;
    }

    return (
      <ul className="space-y-3">
        {entries.map((item) => {
          const meta = historyStatusStyles[item.status as HistoryStatus] ?? defaultHistoryStatusStyle;
          return (
            <li key={item.id} className="flex flex-col gap-2 rounded-2xl border border-surface-muted/60 bg-surface/80 p-4">
              <div className="flex items-center justify-between gap-3">
                <span className="text-base font-semibold text-primary-foreground">{formatNumber(item.key)} vé</span>
                <span className={`rounded-full px-3 py-1 text-xs font-semibold ${meta.className}`}>{meta.label}</span>
              </div>
              {item.note && <p className="text-sm text-surface-foreground/70">{item.note}</p>}
              <p className="text-xs text-surface-foreground/50">
                {formatRelativeTime(item.created_at)} · {formatDateTime(item.created_at)}
              </p>
            </li>
          );
        })}
      </ul>
    );
  };

  const renderPaymentHistory = () => {
    if (paymentHistoryQuery.isLoading) {
      return <LoadingState />;
    }

    if (paymentHistoryQuery.isError) {
      return <InlineError message="Không thể tải lịch sử nạp." />;
    }

    const entries = paymentHistoryQuery.data ?? [];
    if (entries.length === 0) {
      return <EmptyState message="Bạn chưa có giao dịch nạp nào." />;
    }

    return (
      <ul className="space-y-3">
        {entries.map((item) => (
          <li key={item.id} className="flex flex-col gap-2 rounded-2xl border border-surface-muted/60 bg-surface/80 p-4">
            <div className="flex flex-wrap items-center justify-between gap-3">
              <span className="text-base font-semibold text-primary-foreground">+{formatNumber(item.amount_coin)} coin</span>
              <span className="text-sm font-medium text-emerald-300">{formatCurrency(item.amount_money)}</span>
            </div>
            <div className="text-sm text-surface-foreground/70">
              Phương thức: {item.payment_method || "Không xác định"}
            </div>
            {item.note && <p className="text-sm text-surface-foreground/60">Ghi chú: {item.note}</p>}
            <p className="text-xs text-surface-foreground/50">
              {formatRelativeTime(item.created_at)} · {formatDateTime(item.created_at)}
            </p>
          </li>
        ))}
      </ul>
    );
  };

  const renderReadHistory = () => {
    if (readHistoryQuery.isLoading) {
      return <LoadingState />;
    }

    if (readHistoryQuery.isError) {
      return <InlineError message="Không thể tải lịch sử đọc." />;
    }

    const entries = readHistoryQuery.data ?? [];
    if (entries.length === 0) {
      return <EmptyState message="Bạn chưa đọc truyện nào gần đây." />;
    }

    return (
      <ul className="space-y-3">
        {entries.map((item) => (
          <li key={item.id} className="flex flex-col gap-2 rounded-2xl border border-surface-muted/60 bg-surface/80 p-4">
            <div className="flex flex-wrap items-center gap-2 text-sm text-surface-foreground/70">
              <span className="font-semibold text-primary-foreground">Truyện #{item.comic_id}</span>
              <span className="text-surface-foreground/50">·</span>
              <span>Chương #{item.chapter_id}</span>
            </div>
            <p className="text-xs text-surface-foreground/50">
              {formatRelativeTime(item.read_at)} · {formatDateTime(item.read_at)}
            </p>
          </li>
        ))}
      </ul>
    );
  };

  const renderCommentHistory = () => {
    if (commentHistoryQuery.isLoading) {
      return <LoadingState />;
    }

    if (commentHistoryQuery.isError) {
      return <InlineError message="Không thể tải lịch sử bình luận." />;
    }

    const entries = commentHistoryQuery.data ?? [];
    if (entries.length === 0) {
      return <EmptyState message="Bạn chưa có bình luận nào." />;
    }

    return (
      <ul className="space-y-3">
        {entries.map((item) => (
          <li key={item.id} className="flex flex-col gap-2 rounded-2xl border border-surface-muted/60 bg-surface/80 p-4">
            <div className="flex flex-wrap items-center justify-between gap-2 text-sm text-surface-foreground/70">
              <span className="font-semibold text-primary-foreground">Truyện #{item.comic_id}</span>
              {item.comic_chapter_id && <span className="text-xs text-surface-foreground/50">Chương #{item.comic_chapter_id}</span>}
            </div>
            <p className="text-sm text-surface-foreground/80">{item.comment}</p>
            {item.is_rate && item.rate_star !== null && (
              <p className="text-xs font-medium text-amber-300">Đã đánh giá {item.rate_star}/5 sao</p>
            )}
            <p className="text-xs text-surface-foreground/50">
              {formatRelativeTime(item.created_at)} · {formatDateTime(item.created_at)}
            </p>
          </li>
        ))}
      </ul>
    );
  };

  if (isLoading) {
    return (
      <main className="mx-auto flex w-full max-w-5xl flex-1 flex-col gap-8 px-6 py-10">
        <LoadingState />
      </main>
    );
  }

  if (isError || !profile) {
    return (
      <main className="mx-auto flex w-full max-w-5xl flex-1 flex-col gap-8 px-6 py-10">
        <ErrorState />
      </main>
    );
  }

  const totalInteraction = Number(profile.read_comic_count) + Number(profile.bookmark_count);

  return (
    <main className="mx-auto flex w-full max-w-5xl flex-1 flex-col gap-8 px-6 py-10">
      <ProfileHeader profile={profile} />

      <nav className="flex flex-wrap gap-3 rounded-3xl border border-surface-muted/60 bg-surface/80 p-2 shadow-lg">
        {mainTabs.map((tab) => {
          const isActive = tab.key === activeMainTab;
          return (
            <button
              key={tab.key}
              type="button"
              className={`rounded-full px-5 py-2 text-sm font-semibold transition ${
                isActive
                  ? "bg-primary text-primary-foreground shadow-lg"
                  : "border border-transparent text-surface-foreground/70 hover:border-primary/40 hover:text-primary"
              }`}
              onClick={() => handleMainTabChange(tab.key)}
            >
              {tab.label}
            </button>
          );
        })}
      </nav>

      {activeMainTab === "info" && (
        <>
          <ProfileStatsGrid profile={profile} />

          <section className="grid gap-4 lg:grid-cols-2">
            <ActivityCard label="Tổng lượt tương tác" value={totalInteraction} />
            <div className="flex flex-col gap-3 rounded-3xl border border-surface-muted/70 bg-surface/70 p-6 shadow-lg">
              <div className="flex items-center gap-2 text-primary-foreground">
                <ShieldCheck className="h-5 w-5 text-primary" />
                <h2 className="text-sm font-semibold uppercase tracking-wide">Bảo mật tài khoản</h2>
              </div>
              <p className="text-sm text-surface-foreground/70">
                Email {profile.email_verified_at ? "đã" : "chưa"} được xác thực. Hãy đảm bảo thông tin liên hệ chính xác để nhận thông báo quan trọng từ TruyenCV.
              </p>
            </div>
          </section>

          <section className="grid gap-4 lg:grid-cols-2">
            <EmailVerificationCard
              isVerified={Boolean(profile.email_verified_at)}
              isVerifying={verifyEmailMutation.isPending}
              onVerify={() => {
                setVerifyError(null);
                verifyEmailMutation.mutate();
              }}
              lastVerifiedAt={profile.email_verified_at}
            />
            <div className="lg:col-span-1">
              <ChangePasswordForm
                username={profile.name}
                onSuccess={() => queryClient.invalidateQueries({ queryKey: ["user-profile"] })}
              />
            </div>
            {verifyError && <p className="lg:col-span-2 text-xs font-medium text-rose-400">{verifyError}</p>}
          </section>
        </>
      )}

      {activeMainTab === "payment" && (
        <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
          <nav className="mb-6 flex flex-wrap justify-center gap-3">
            {paymentTabs.map((tab) => {
              const isActive = tab.key === activePaymentTab;
              return (
                <button
                  key={tab.key}
                  type="button"
                  className={`rounded-full px-5 py-2 text-sm font-semibold transition ${
                    isActive
                      ? "bg-primary text-primary-foreground shadow-lg"
                      : "border border-surface-muted/60 text-surface-foreground/70 hover:border-primary/40 hover:text-primary"
                  }`}
                  onClick={() => handlePaymentTabChange(tab.key)}
                >
                  {tab.label}
                </button>
              );
            })}
          </nav>
          <div>
            {activePaymentTab === "coin" && renderCoinHistory()}
            {activePaymentTab === "deposit" && renderPaymentHistory()}
            {activePaymentTab === "ticket" && renderKeyHistory()}
          </div>
        </section>
      )}

      {activeMainTab === "read" && (
        <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
          {renderReadHistory()}
        </section>
      )}

      {activeMainTab === "comment" && (
        <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
          {renderCommentHistory()}
        </section>
      )}
    </main>
  );
};

export default UserProfilePage;
