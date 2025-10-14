"use client";

import { useState } from "react";
import { Loader2, ShieldCheck } from "lucide-react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { AxiosError } from "axios";

import { useToast } from "@components/providers/ToastProvider";
import ProfileHeader from "@components/user/profile/ProfileHeader";
import ProfileStatsGrid from "@components/user/profile/ProfileStatsGrid";
import EmailVerificationCard from "@components/user/profile/EmailVerificationCard";
import ChangePasswordForm from "@components/user/profile/ChangePasswordForm";
import { useUserProfileQuery, verifyEmail } from "@services/user/profile.service";
import { formatNumber } from "@helpers/format";

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

const UserProfilePage = () => {
  const queryClient = useQueryClient();
  const { data: profile, isLoading, isError } = useUserProfileQuery();
  const [verifyError, setVerifyError] = useState<string | null>(null);
  const { pushToast } = useToast();

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

  return (
    <main className="mx-auto flex w-full max-w-5xl flex-1 flex-col gap-8 px-6 py-10">
      <ProfileHeader profile={profile} />

      <ProfileStatsGrid profile={profile} />

      <section className="grid gap-4 lg:grid-cols-2">
        <ActivityCard label="Tổng lượt tương tác" value={Number(profile.read_comic_count) + Number(profile.bookmark_count)} />
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
        {verifyError && (
          <p className="lg:col-span-2 text-xs font-medium text-rose-400">{verifyError}</p>
        )}
      </section>
    </main>
  );
};

export default UserProfilePage;
