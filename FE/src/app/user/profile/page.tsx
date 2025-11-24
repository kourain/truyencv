"use client";

import { useEffect, useState } from "react";
import { AlertTriangle, Link2Off, Loader2, LogOut, MailCheck, ShieldCheck } from "lucide-react";
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
  changeEmail,
  unlinkFirebaseAccount,
  useUserActiveSessionsQuery,
  revokeUserSession,
  revokeAllUserSessions,
} from "@services/user/profile.service";
import { formatNumber, formatRelativeTime } from "@helpers/format";
import { HistoryStatus } from "@const/enum/history-status";
import { useAuth } from "@hooks/useAuth";

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

type MainTabKey = "info" | "payment" | "read" | "comment" | "security";
type PaymentTabKey = "coin" | "deposit" | "ticket";
type SecurityTabKey = "email" | "password" | "sessions";

const mainTabs: Array<{ key: MainTabKey; label: string }> = [
  { key: "info", label: "Thông tin" },
  { key: "payment", label: "Lịch sử thanh toán" },
  { key: "read", label: "Lịch sử đọc" },
  { key: "comment", label: "Lịch sử bình luận" },
  { key: "security", label: "Bảo mật" },
];

const paymentTabs: Array<{ key: PaymentTabKey; label: string }> = [
  { key: "coin", label: "Coin" },
  { key: "deposit", label: "Nạp" },
  { key: "ticket", label: "Vé" },
];

const securityTabs: Array<{ key: SecurityTabKey; label: string }> = [
  { key: "email", label: "Email" },
  { key: "password", label: "Mật khẩu" },
  { key: "sessions", label: "Lịch sử đăng nhập" },
];

const paymentTabPaths: Record<PaymentTabKey, string> = {
  coin: "/user/profile/history_coin",
  deposit: "/user/profile/history_deposit",
  ticket: "/user/profile/history_ticket",
};

const securityTabPaths: Record<SecurityTabKey, string> = {
  email: "/user/profile/security_email",
  password: "/user/profile/security_password",
  sessions: "/user/profile/security_sessions",
};

const mainTabDefaultPaths: Record<MainTabKey, string> = {
  info: "/user/profile",
  payment: paymentTabPaths.coin,
  read: "/user/profile/history_read",
  comment: "/user/profile/history_comment",
  security: securityTabPaths.email,
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

const resolveTabsFromSection = (section: string | null): { main: MainTabKey; payment?: PaymentTabKey; security?: SecurityTabKey } => {
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
    case "security_password":
      return { main: "security", security: "password" };
    case "security_email":
      return { main: "security", security: "email" };
    case "security_sessions":
      return { main: "security", security: "sessions" };
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

const maskToken = (token: string) => {
  if (token.length <= 10) {
    return token;
  }
  return `${token.slice(0, 4)}...${token.slice(-4)}`;
};

const UserProfilePage = () => {
  const auth = useAuth();
  const router = useRouter();
  const searchParams = useSearchParams();
  const queryClient = useQueryClient();
  const { data: profile, isLoading, isError } = useUserProfileQuery();
  const [verifyError, setVerifyError] = useState<string | null>(null);
  const [activeMainTab, setActiveMainTab] = useState<MainTabKey>("info");
  const [activePaymentTab, setActivePaymentTab] = useState<PaymentTabKey>("coin");
  const [activeSecurityTab, setActiveSecurityTab] = useState<SecurityTabKey>("email");
  const [newEmail, setNewEmail] = useState<string>("");
  const [confirmEmail, setConfirmEmail] = useState<string>("");
  const [currentPassword, setCurrentPassword] = useState<string>("");
  const [emailFormError, setEmailFormError] = useState<string | null>(null);
  const [revokingSessionId, setRevokingSessionId] = useState<string | null>(null);
  const { pushToast } = useToast();
  const coinHistoryQuery = useUserCoinHistoryQuery();
  const paymentHistoryQuery = useUserPaymentHistoryQuery();
  const keyHistoryQuery = useUserKeyHistoryQuery();
  const readHistoryQuery = useUserReadHistoryQuery(50);
  const commentHistoryQuery = useUserCommentHistoryQuery();
  const isSessionsTabActive = activeMainTab === "security" && activeSecurityTab === "sessions";
  const activeSessionsQuery = useUserActiveSessionsQuery({ enabled: isSessionsTabActive });

  const updateProfileCache = (data: UserProfileResponse) => {
    queryClient.setQueryData(["user-profile"], (prev: (UserProfileResponse & AuthTokensResponse) | undefined) => {
      if (!prev) {
        return data as UserProfileResponse & AuthTokensResponse;
      }

      return {
        ...prev,
        ...data,
      };
    });
  };

  const getErrorMessage = (error: unknown, fallbackMessage: string) => {
    if (typeof error === "object" && error !== null && "response" in error) {
      const apiError = error as AxiosError<{ message?: string }>;
      return apiError.response?.data?.message ?? fallbackMessage;
    }

    return fallbackMessage;
  };

  const verifyEmailMutation = useMutation({
    mutationFn: verifyEmail,
    onSuccess: (data) => {
      updateProfileCache(data);
      setVerifyError(null);
      pushToast({
        title: "Email đã được xác thực",
        description: "Cảm ơn bạn đã xác nhận địa chỉ email.",
        variant: "success",
      });
    },
    onError: (error: unknown) => {
      const fallbackMessage = "Không thể gửi yêu cầu xác thực email. Vui lòng thử lại sau.";
      const message = getErrorMessage(error, fallbackMessage);
      setVerifyError(message);
      pushToast({
        title: "Xác thực email thất bại",
        description: message,
        variant: "error",
      });
    },
  });

  const changeEmailMutation = useMutation({
    mutationFn: changeEmail,
    onSuccess: (data) => {
      updateProfileCache(data);
      setEmailFormError(null);
      setNewEmail("");
      setConfirmEmail("");
      setCurrentPassword("");
      pushToast({
        title: "Cập nhật email thành công",
        description: "Vui lòng xác thực lại email mới để hoàn tất.",
        variant: "success",
      });
    },
    onError: (error: unknown) => {
      const fallbackMessage = "Không thể cập nhật email. Vui lòng thử lại sau.";
      const message = getErrorMessage(error, fallbackMessage);
      setEmailFormError(message);
      pushToast({
        title: "Đổi email thất bại",
        description: message,
        variant: "error",
      });
    },
  });

  const unlinkFirebaseMutation = useMutation({
    mutationFn: unlinkFirebaseAccount,
    onSuccess: (data) => {
      updateProfileCache(data);
      pushToast({
        title: "Đã hủy liên kết Firebase",
        description: "Email cần được xác thực trước khi đăng nhập lại bằng mật khẩu.",
        variant: "success",
      });
    },
    onError: (error: unknown) => {
      const fallbackMessage = "Không thể hủy liên kết Firebase. Vui lòng thử lại sau.";
      const message = getErrorMessage(error, fallbackMessage);
      pushToast({
        title: "Hủy liên kết thất bại",
        description: message,
        variant: "error",
      });
    },
  });

  const revokeSessionMutation = useMutation({
    mutationFn: (sessionId: string) => revokeUserSession(sessionId),
    onMutate: (sessionId: string) => {
      setRevokingSessionId(sessionId);
    },
    onSuccess: () => {
      pushToast({
        title: "Đã hủy phiên đăng nhập",
        description: "Phiên đã được vô hiệu hóa ngay lập tức.",
        variant: "success",
      });
      activeSessionsQuery.refetch();
    },
    onError: (error: unknown) => {
      const fallbackMessage = "Không thể hủy phiên đăng nhập. Vui lòng thử lại sau.";
      const message = getErrorMessage(error, fallbackMessage);
      pushToast({
        title: "Hủy phiên thất bại",
        description: message,
        variant: "error",
      });
    },
    onSettled: () => {
      setRevokingSessionId(null);
    },
  });

  const revokeAllSessionsMutation = useMutation({
    mutationFn: revokeAllUserSessions,
    onSuccess: () => {
      pushToast({
        title: "Đã hủy tất cả phiên",
        description: "Mọi phiên đăng nhập đang hoạt động đã bị vô hiệu hóa.",
        variant: "success",
      });
      activeSessionsQuery.refetch();
    },
    onError: (error: unknown) => {
      const fallbackMessage = "Không thể hủy tất cả phiên. Vui lòng thử lại sau.";
      const message = getErrorMessage(error, fallbackMessage);
      pushToast({
        title: "Hủy phiên thất bại",
        description: message,
        variant: "error",
      });
    },
  });

  const sectionParam = searchParams.get("section");

  useEffect(() => {
    const resolved = resolveTabsFromSection(sectionParam);
    setActiveMainTab(resolved.main);
    if (resolved.main === "payment") {
      setActivePaymentTab(resolved.payment ?? "coin");
      setActiveSecurityTab("email");
    } else if (resolved.main === "security") {
      setActiveSecurityTab(resolved.security ?? "email");
      setActivePaymentTab("coin");
    } else {
      setActivePaymentTab("coin");
      setActiveSecurityTab("email");
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

    if (tab === "security") {
      const nextSecurityTab = activeSecurityTab ?? "email";
      setActiveMainTab("security");
      setActiveSecurityTab(nextSecurityTab);
      router.push(securityTabPaths[nextSecurityTab]);
      return;
    }

    setActiveMainTab(tab);
    setActivePaymentTab("coin");
    setActiveSecurityTab("email");
    router.push(mainTabDefaultPaths[tab]);
  };

  const handlePaymentTabChange = (tab: PaymentTabKey) => {
    setActiveMainTab("payment");
    setActivePaymentTab(tab);
    router.push(paymentTabPaths[tab]);
  };

  const handleSecurityTabChange = (tab: SecurityTabKey) => {
    setActiveMainTab("security");
    setActiveSecurityTab(tab);
    router.push(securityTabPaths[tab]);
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
          const unlockDetails = [
            item.comic_name,
            item.chapter_number ? `Chương ${item.chapter_number}` : null,
          ]
            .filter((value): value is string => Boolean(value))
            .join(" · ");
          return (
            <li key={item.id} className="flex flex-col gap-2 rounded-2xl border border-surface-muted/60 bg-surface/80 p-4">
              <div className="flex items-center justify-between gap-3">
                <span className="text-base font-semibold text-primary-foreground">{formatNumber(item.key)} vé</span>
                <span className={`rounded-full px-3 py-1 text-xs font-semibold ${meta.className}`}>{meta.label}</span>
              </div>
              {unlockDetails && <p className="text-sm text-surface-foreground/70">{unlockDetails}</p>}
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
              <span className="font-semibold text-primary-foreground">{item.comic_name ?? `Truyện #${item.comic_id}`}</span>
              <span className="text-surface-foreground/50">·</span>
              <span>
                {item.chapter_number !== null && item.chapter_number !== undefined
                  ? `Chương ${item.chapter_number}`
                  : `Chương #${item.chapter_id}`}
              </span>
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
        {entries.map((item) => {
          const commentChapterLabel =
            item.chapter_number !== null && item.chapter_number !== undefined
              ? `Chương ${item.chapter_number}`
              : item.comic_chapter_id
                ? `Chương #${item.comic_chapter_id}`
                : null;
          return (
            <li key={item.id} className="flex flex-col gap-2 rounded-2xl border border-surface-muted/60 bg-surface/80 p-4">
              <div className="flex flex-wrap items-center justify-between gap-2 text-sm text-surface-foreground/70">
                <span className="font-semibold text-primary-foreground">{item.comic_name ?? `Truyện #${item.comic_id}`}</span>
                {commentChapterLabel && <span className="text-xs text-surface-foreground/50">{commentChapterLabel}</span>}
              </div>
              <p className="text-sm text-surface-foreground/80">{item.comment}</p>
              {item.is_rate && item.rate_star !== null && (
                <p className="text-xs font-medium text-amber-300">Đã đánh giá {item.rate_star}/5 sao</p>
              )}
              <p className="text-xs text-surface-foreground/50">
                {formatRelativeTime(item.created_at)} · {formatDateTime(item.created_at)}
              </p>
            </li>
          );
        })}
      </ul>
    );
  };

  const renderActiveSessions = () => {
    if (!isSessionsTabActive) {
      return null;
    }

    if (activeSessionsQuery.isLoading) {
      return <LoadingState />;
    }

    if (activeSessionsQuery.isError) {
      return <InlineError message="Không thể tải danh sách phiên đăng nhập." />;
    }

    const sessions = activeSessionsQuery.data ?? [];
    if (sessions.length === 0) {
      return <EmptyState message="Không có phiên đăng nhập nào đang hoạt động." />;
    }

    return (
      <ul className="space-y-3">
        {sessions.map((session) => {
          const currentRefreshToken = auth.refresh_token ?? "";
          const isCurrentSession = Boolean(currentRefreshToken) && session.token === currentRefreshToken;
          const isRevoking = (!isCurrentSession && revokingSessionId === session.id) || revokeAllSessionsMutation.isPending;
          return (
            <li key={session.id} className="flex flex-col gap-3 rounded-2xl border border-surface-muted/60 bg-surface/80 p-4">
              <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                  <div className="flex items-center gap-2">
                    <p className="text-sm font-semibold text-primary-foreground">Phiên #{session.id}</p>
                    {isCurrentSession && <span className="rounded-full bg-emerald-500/15 px-3 py-0.5 text-xs font-semibold text-emerald-200">Phiên hiện tại</span>}
                  </div>
                  <p className="text-xs text-surface-foreground/60">Token: {maskToken(session.token)}</p>
                </div>
                <button
                  type="button"
                  disabled={isRevoking || isCurrentSession}
                  className={`inline-flex items-center gap-2 rounded-full px-4 py-2 text-xs font-semibold transition disabled:cursor-not-allowed disabled:opacity-70 ${
                    isCurrentSession ? "border border-surface-muted/60 text-surface-foreground/60" : "border border-rose-500/60 text-rose-200 hover:bg-rose-500/10"
                  }`}
                  onClick={() => {
                    if (isCurrentSession) {
                      return;
                    }
                    revokeSessionMutation.mutate(session.id);
                  }}
                >
                  {isRevoking && <span className="h-3 w-3 animate-spin rounded-full border border-rose-200 border-t-transparent" />}
                  {isCurrentSession ? "Đang sử dụng" : "Hủy phiên"}
                </button>
              </div>
              <div className="grid gap-2 text-xs text-surface-foreground/70 md:grid-cols-2">
                <p>Khởi tạo: {formatDateTime(session.created_at)}</p>
                <p>Hết hạn: {formatDateTime(session.expires_at)}</p>
                <p className="text-surface-foreground/50">{formatRelativeTime(session.created_at)} tạo</p>
                <p className="text-surface-foreground/50">{formatRelativeTime(session.expires_at)} hết hạn</p>
              </div>
            </li>
          );
        })}
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
  const requiresPasswordForEmailChange = !profile.has_firebase_linked;
  const emailInputsDisabled = changeEmailMutation.isPending;
  const unlinkFirebaseDisabled = unlinkFirebaseMutation.isPending || !profile.has_firebase_linked;

  const handleEmailChangeSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const trimmedNewEmail = newEmail.trim();
    const trimmedConfirmEmail = confirmEmail.trim();

    if (!trimmedNewEmail) {
      setEmailFormError("Vui lòng nhập email mới.");
      return;
    }

    if (trimmedNewEmail !== trimmedConfirmEmail) {
      setEmailFormError("Email xác nhận không khớp.");
      return;
    }

    const payload: ChangeEmailPayload = { new_email: trimmedNewEmail };

    if (requiresPasswordForEmailChange) {
      const normalizedPassword = currentPassword.trim();
      if (!normalizedPassword) {
        setEmailFormError("Vui lòng nhập mật khẩu hiện tại.");
        return;
      }

      payload.current_password = normalizedPassword;
    }

    setEmailFormError(null);
    changeEmailMutation.mutate(payload);
  };

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
              <p className="text-xs text-surface-foreground/60">
                Quản lý email, mật khẩu và liên kết Firebase nhanh chóng tại tab <span className="font-semibold text-primary">Bảo mật</span>.
              </p>
            </div>
          </section>

          <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
            <h3 className="text-base font-semibold text-primary-foreground">Lối tắt quản lý bảo mật</h3>
            <p className="mt-2 text-sm text-surface-foreground/70">
              Chuyển sang tab <span className="font-semibold text-primary">Bảo mật</span> để xác thực email, cập nhật địa chỉ email, đổi mật khẩu
              hoặc hủy liên kết Firebase theo nhu cầu của bạn.
            </p>
          </section>
        </>
      )}

      {activeMainTab === "security" && (
        <section className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg">
          <nav className="mb-6 flex flex-wrap justify-center gap-3">
            {securityTabs.map((tab) => {
              const isActive = tab.key === activeSecurityTab;
              return (
                <button
                  key={tab.key}
                  type="button"
                  className={`rounded-full px-5 py-2 text-sm font-semibold transition ${
                    isActive
                      ? "bg-primary text-primary-foreground shadow-lg"
                      : "border border-surface-muted/60 text-surface-foreground/70 hover:border-primary/40 hover:text-primary"
                  }`}
                  onClick={() => handleSecurityTabChange(tab.key)}
                >
                  {tab.label}
                </button>
              );
            })}
          </nav>

          <div>
            {activeSecurityTab === "email" && (
              <div className="space-y-6">
                <div className="grid gap-4 lg:grid-cols-2">
                  <EmailVerificationCard
                    isVerified={Boolean(profile.email_verified_at)}
                    isVerifying={verifyEmailMutation.isPending}
                    onVerify={() => {
                      setVerifyError(null);
                      verifyEmailMutation.mutate();
                    }}
                    lastVerifiedAt={profile.email_verified_at}
                  />

                  <form className="flex flex-col gap-4 rounded-3xl border border-slate-700/70 bg-slate-900/70 p-6 shadow-xl" onSubmit={handleEmailChangeSubmit}>
                    <div className="flex items-center gap-2 text-slate-100">
                      <MailCheck className="h-5 w-5 text-primary" />
                      <h3 className="text-lg font-semibold">Đổi email đăng nhập</h3>
                    </div>

                    <label className="flex flex-col gap-2 text-xs text-slate-300">
                      <span>Email mới</span>
                      <input
                        type="email"
                        value={newEmail}
                        onChange={(event) => setNewEmail(event.target.value)}
                        disabled={emailInputsDisabled}
                        className="w-full rounded-xl border border-slate-700 bg-slate-950/40 px-4 py-3 text-sm text-slate-100 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/40 disabled:opacity-70"
                        placeholder="name@example.com"
                        required
                      />
                    </label>

                    <label className="flex flex-col gap-2 text-xs text-slate-300">
                      <span>Xác nhận email mới</span>
                      <input
                        type="email"
                        value={confirmEmail}
                        onChange={(event) => setConfirmEmail(event.target.value)}
                        disabled={emailInputsDisabled}
                        className="w-full rounded-xl border border-slate-700 bg-slate-950/40 px-4 py-3 text-sm text-slate-100 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/40 disabled:opacity-70"
                        placeholder="Nhập lại email"
                        required
                      />
                    </label>

                    {requiresPasswordForEmailChange && (
                      <label className="flex flex-col gap-2 text-xs text-slate-300">
                        <span>Mật khẩu hiện tại</span>
                        <input
                          type="password"
                          value={currentPassword}
                          onChange={(event) => setCurrentPassword(event.target.value)}
                          disabled={emailInputsDisabled}
                          className="w-full rounded-xl border border-slate-700 bg-slate-950/40 px-4 py-3 text-sm text-slate-100 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/40 disabled:opacity-70"
                          placeholder="Nhập mật khẩu để xác nhận"
                          required
                        />
                      </label>
                    )}

                    {emailFormError && <p className="text-xs font-medium text-rose-400">{emailFormError}</p>}

                    <button
                      type="submit"
                      disabled={emailInputsDisabled}
                      className="mt-2 inline-flex items-center justify-center gap-2 rounded-full bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:opacity-70"
                    >
                      {changeEmailMutation.isPending && (
                        <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
                      )}
                      Cập nhật email
                    </button>
                  </form>
                </div>

                {verifyError && <p className="text-xs font-medium text-rose-400">{verifyError}</p>}

                <div className="flex flex-col gap-4 rounded-3xl border border-surface-muted/70 bg-surface/80 p-6 shadow-lg">
                  <div className="flex items-center gap-3 text-primary-foreground">
                    <Link2Off className="h-5 w-5 text-primary" />
                    <div>
                      <h3 className="text-base font-semibold">Liên kết Firebase</h3>
                      <p className="text-xs text-surface-foreground/60">
                        Trạng thái: {profile.has_firebase_linked ? "Đang liên kết" : "Không liên kết"}
                      </p>
                    </div>
                  </div>
                  <p className="text-sm text-surface-foreground/70">
                    Khi hủy liên kết, tài khoản sẽ sử dụng đăng nhập bằng email và mật khẩu. Email cần được xác thực lại sau khi hủy liên kết.
                  </p>
                  <div className="flex items-start gap-3 rounded-2xl bg-amber-500/10 p-4 text-xs text-amber-200">
                    <AlertTriangle className="h-4 w-4 flex-shrink-0" />
                    <p>Hành động này không thể hoàn tác ngay. Vui lòng chắc chắn rằng bạn đã nhớ mật khẩu hiện tại trước khi hủy liên kết.</p>
                  </div>
                  <button
                    type="button"
                    disabled={unlinkFirebaseDisabled}
                    className={`inline-flex items-center justify-center gap-2 rounded-full px-6 py-3 text-sm font-semibold transition ${
                      profile.has_firebase_linked
                        ? "bg-rose-600 text-white hover:bg-rose-500"
                        : "bg-slate-700 text-slate-300"
                    } disabled:cursor-not-allowed disabled:opacity-70`}
                    onClick={() => {
                      if (!profile.has_firebase_linked || unlinkFirebaseDisabled) {
                        return;
                      }
                      unlinkFirebaseMutation.mutate();
                    }}
                  >
                    {unlinkFirebaseMutation.isPending && (
                      <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
                    )}
                    {profile.has_firebase_linked ? "Hủy liên kết Firebase" : "Không có liên kết Firebase"}
                  </button>
                </div>
              </div>
            )}

            {activeSecurityTab === "password" && (
              <div className="mx-auto w-full lg:w-3/4">
                <ChangePasswordForm
                  username={profile.name}
                  onSuccess={() => queryClient.invalidateQueries({ queryKey: ["user-profile"] })}
                />
              </div>
            )}

            {activeSecurityTab === "sessions" && (
              <div className="space-y-6">
                <div className="flex flex-col gap-4 rounded-3xl border border-surface-muted/70 bg-surface/80 p-6 shadow-lg md:flex-row md:items-center md:justify-between">
                  <div className="flex items-center gap-3 text-primary-foreground">
                    <LogOut className="h-5 w-5 text-primary" />
                    <div>
                      <p className="text-sm font-semibold">Phiên đăng nhập đang hoạt động</p>
                      <p className="text-xs text-surface-foreground/60">
                        {activeSessionsQuery.data?.length ?? 0} phiên hợp lệ. Hãy hủy các thiết bị không còn sử dụng.
                      </p>
                    </div>
                  </div>
                  <div className="flex flex-wrap gap-3">
                    <button
                      type="button"
                      className="rounded-full border border-surface-muted/60 px-4 py-2 text-xs font-semibold text-surface-foreground/70 transition hover:border-primary/40 hover:text-primary"
                      onClick={() => activeSessionsQuery.refetch()}
                      disabled={activeSessionsQuery.isFetching}
                    >
                      {activeSessionsQuery.isFetching && (
                        <span className="mr-2 inline-block h-3 w-3 animate-spin rounded-full border border-primary border-t-transparent" />
                      )}
                      Tải lại
                    </button>
                    <button
                      type="button"
                      className="rounded-full bg-rose-600 px-4 py-2 text-xs font-semibold text-white transition hover:bg-rose-500 disabled:cursor-not-allowed disabled:opacity-70"
                      disabled={(activeSessionsQuery.data?.length ?? 0) === 0 || revokeAllSessionsMutation.isPending}
                      onClick={() => revokeAllSessionsMutation.mutate()}
                    >
                      {revokeAllSessionsMutation.isPending && (
                        <span className="mr-2 inline-block h-3 w-3 animate-spin rounded-full border border-white border-t-transparent" />
                      )}
                      Hủy tất cả phiên
                    </button>
                  </div>
                </div>

                {renderActiveSessions()}
              </div>
            )}
          </div>
        </section>
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
