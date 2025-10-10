"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useState } from "react";
import type { Route } from "next";
import { useMutation } from "@tanstack/react-query";
import { AlertCircle, CheckCircle2, KeyRound, Lock, Mail } from "lucide-react";

import { confirmPasswordReset, requestPasswordReset } from "@services/auth.service";

const OTP_LENGTH = 6;

const normalizeEmail = (value: string) => value.trim().toLowerCase();

export const UserResetPasswordContent = () => {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [otp, setOtp] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [step, setStep] = useState<"request" | "confirm">("request");

  const requestMutation = useMutation({
    mutationFn: (payload: RequestPasswordResetRequest) => requestPasswordReset(payload),
    onSuccess: (response) => {
      setError(null);
      setMessage(response.message ?? "Nếu email tồn tại, mã OTP đã được gửi");
      setStep("confirm");
    },
    onError: (mutationError) => {
      const fallback = "Không thể gửi mã OTP. Vui lòng thử lại sau.";
      if (typeof mutationError === "object" && mutationError !== null && "response" in mutationError) {
        const apiError = mutationError as { response?: { data?: { message?: string } } };
        setError(apiError.response?.data?.message ?? fallback);
      } else {
        setError(fallback);
      }
      setMessage(null);
    }
  });

  const confirmMutation = useMutation({
    mutationFn: (payload: ConfirmPasswordResetRequest) => confirmPasswordReset(payload),
    onSuccess: (response) => {
      setError(null);
      setMessage(response.message ?? "Đặt lại mật khẩu thành công");
      setTimeout(() => {
        router.replace("/user/auth/login?reset=1" as Route);
      }, 1600);
    },
    onError: (mutationError) => {
      const fallback = "OTP không hợp lệ hoặc đã hết hạn.";
      if (typeof mutationError === "object" && mutationError !== null && "response" in mutationError) {
        const apiError = mutationError as { response?: { data?: { message?: string } } };
        setError(apiError.response?.data?.message ?? fallback);
      } else {
        setError(fallback);
      }
    }
  });

  const handleRequest = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);
    setMessage(null);

    const normalizedEmail = normalizeEmail(email);
    if (!normalizedEmail) {
      setError("Vui lòng nhập email đã đăng ký");
      return;
    }

    requestMutation.mutate({ email: normalizedEmail });
  };

  const handleConfirm = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);

    const normalizedEmail = normalizeEmail(email);
    if (!normalizedEmail) {
      setError("Vui lòng nhập email đã đăng ký");
      return;
    }

    if (!otp || otp.length < 4) {
      setError("Vui lòng nhập đầy đủ mã OTP");
      return;
    }

    if (!password || password.length < 8) {
      setError("Mật khẩu mới cần ít nhất 8 ký tự");
      return;
    }

    if (password !== confirmPassword) {
      setError("Mật khẩu xác nhận không khớp");
      return;
    }

    confirmMutation.mutate({ email: normalizedEmail, otp, new_password: password });
  };

  const renderRequestStep = () => (
    <form className="space-y-5" onSubmit={handleRequest}>
      <div className="space-y-2">
        <label htmlFor="email" className="text-sm font-medium text-primary-foreground">
          Email đã đăng ký
        </label>
        <div className="relative">
          <Mail className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
          <input
            id="email"
            type="email"
            className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
            placeholder="ban@example.com"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
          />
        </div>
      </div>

      <button
        type="submit"
        disabled={requestMutation.isPending}
        className="inline-flex w-full items-center justify-center gap-2 rounded-full border border-primary/50 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
      >
        {requestMutation.isPending && (
          <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
        )}
        Gửi mã OTP
      </button>
    </form>
  );

  const renderConfirmStep = () => (
    <form className="space-y-5" onSubmit={handleConfirm}>
      <div className="space-y-2">
        <label htmlFor="otp" className="text-sm font-medium text-primary-foreground">
          Nhập mã OTP
        </label>
        <div className="relative">
          <KeyRound className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
          <input
            id="otp"
            type="text"
            inputMode="numeric"
            maxLength={OTP_LENGTH}
            className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
            placeholder="Nhập mã gồm 6 chữ số"
            value={otp}
            onChange={(event) => setOtp(event.target.value.replace(/[^0-9]/g, ""))}
            required
          />
        </div>
      </div>

      <div className="space-y-2">
        <label htmlFor="new-password" className="text-sm font-medium text-primary-foreground">
          Mật khẩu mới
        </label>
        <div className="relative">
          <Lock className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
          <input
            id="new-password"
            type="password"
            className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
            placeholder="••••••••"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
          />
        </div>
      </div>

      <div className="space-y-2">
        <label htmlFor="confirm-password" className="text-sm font-medium text-primary-foreground">
          Xác nhận mật khẩu
        </label>
        <div className="relative">
          <Lock className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
          <input
            id="confirm-password"
            type="password"
            className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
            placeholder="••••••••"
            value={confirmPassword}
            onChange={(event) => setConfirmPassword(event.target.value)}
            required
          />
        </div>
      </div>

      <button
        type="submit"
        disabled={confirmMutation.isPending}
        className="inline-flex w-full items-center justify-center gap-2 rounded-full border border-primary/50 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
      >
        {confirmMutation.isPending && (
          <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
        )}
        Đặt lại mật khẩu
      </button>
    </form>
  );

  return (
    <div className="flex min-h-screen items-center justify-center bg-surface text-surface-foreground">
      <div className="relative w-full max-w-xl overflow-hidden rounded-3xl border border-surface-muted bg-surface/70 shadow-glow backdrop-blur-xl">
        <div className="absolute -top-40 -left-32 h-64 w-64 rounded-full bg-primary/30 blur-3xl" />
        <div className="absolute -bottom-32 -right-24 h-72 w-72 rounded-full bg-primary/20 blur-3xl" />

        <div className="relative z-10 flex flex-col gap-6 px-10 py-12">
          <div className="flex items-center gap-3">
            <span className="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10 text-primary">
              <KeyRound className="h-6 w-6" />
            </span>
            <div>
              <p className="text-xs uppercase tracking-[0.4em] text-primary/80">TruyenCV</p>
              <h1 className="text-2xl font-semibold text-primary-foreground">Đặt lại mật khẩu</h1>
            </div>
          </div>

          {message && (
            <div className="flex items-start gap-3 rounded-2xl border border-emerald-500/40 bg-emerald-500/10 px-4 py-3 text-sm text-emerald-200">
              <CheckCircle2 className="mt-0.5 h-4 w-4 flex-none" />
              <p>{message}</p>
            </div>
          )}

          {error && (
            <div className="flex items-start gap-3 rounded-2xl border border-red-500/40 bg-red-500/10 px-4 py-3 text-sm text-red-200">
              <AlertCircle className="mt-0.5 h-4 w-4 flex-none" />
              <p>{error}</p>
            </div>
          )}

          {step === "request" ? renderRequestStep() : renderConfirmStep()}

          <div className="space-y-1 text-center text-xs text-surface-foreground/60">
            <p>
              Đã nhớ mật khẩu?{" "}
              <Link href={"/user/auth/login" as Route} className="underline">
                Quay lại đăng nhập
              </Link>
            </p>
            <p>
              Chưa có tài khoản?{" "}
              <Link href={"/user/auth/register" as Route} className="underline">
                Đăng ký ngay
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default UserResetPasswordContent;
