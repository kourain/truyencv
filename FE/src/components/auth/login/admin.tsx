"use client";
import { FormEvent, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import type { Route } from "next";
import Link from "next/link";
import { AlertCircle, Lock, Mail, Shield } from "lucide-react";
import { useMutation } from "@tanstack/react-query";
import { login } from "@services/auth.service";
import { clearAuthTokens } from "@helpers/authTokens";
import { useAuth } from "@hooks/useAuth";
import { UserRole } from "@const/role";
import { useToast } from "@components/providers/ToastProvider";

export const AdminLoginContent = () => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const { pushToast } = useToast();

  const redirectParam = searchParams?.get("redirect") as Route | null;
  const fallback = redirectParam ?? ("/admin" as Route);
  const auth = useAuth();

  if (auth?.isAuthenticated && auth?.roles.includes(UserRole.Admin)) {
    router.replace(fallback);
  }
  const loginMutation = useMutation({
    mutationFn: (payload: LoginRequest) => login(payload),
    onSuccess: async (response) => {
      setError(null);
      pushToast({
        title: "Đăng nhập quản trị thành công",
        description: "Chào mừng quay lại bảng điều khiển.",
        variant: "success",
      });
      await auth.updateAuthStateFromAccessToken(response.access_token);
      router.replace(fallback);
    },
    onError: async (mutationError: unknown) => {
      await clearAuthTokens();
      const fallbackMessage = "Không thể đăng nhập. Vui lòng kiểm tra lại thông tin hoặc thử lại sau.";
      if (typeof mutationError === "object" && mutationError !== null && "response" in mutationError) {
        const apiError = mutationError as {
          response?: { data?: { message?: string } };
        };

        const message = apiError.response?.data?.message;
        setError(message ?? fallbackMessage);
        pushToast({
          title: "Đăng nhập thất bại",
          description: message ?? fallbackMessage,
          variant: "error",
        });
      } else {
        setError(fallbackMessage);
        pushToast({
          title: "Đăng nhập thất bại",
          description: fallbackMessage,
          variant: "error",
        });
      }
    }
  });

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);

    if (!email || !password) {
      setError("Vui lòng nhập đầy đủ email và mật khẩu");
      pushToast({
        title: "Thiếu thông tin",
        description: "Vui lòng nhập đầy đủ email và mật khẩu để tiếp tục.",
        variant: "warning",
      });
      return;
    }

    loginMutation.mutate({ email, password });
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-surface text-surface-foreground">
      <div className="relative w-full max-w-lg overflow-hidden rounded-3xl border border-surface-muted bg-surface/70 shadow-glow backdrop-blur-xl">
        <div className="absolute -top-40 -left-32 h-64 w-64 rounded-full bg-primary/30 blur-3xl" />
        <div className="absolute -bottom-32 -right-24 h-72 w-72 rounded-full bg-primary/20 blur-3xl" />
        <div className="relative z-10 flex flex-col gap-6 px-10 py-12">
          <div className="flex items-center gap-3">
            <span className="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10 text-primary">
              <Shield className="h-6 w-6" />
            </span>
            <div>
              <p className="text-xs uppercase tracking-[0.4em] text-primary/80">TruyenCV</p>
              <h1 className="text-2xl font-semibold text-primary-foreground">Đăng nhập quản trị</h1>
            </div>
          </div>

          {error && (
            <div className="flex items-start gap-3 rounded-2xl border border-red-500/40 bg-red-500/10 px-4 py-3 text-sm text-red-200">
              <AlertCircle className="mt-0.5 h-4 w-4 flex-none" />
              <p>{error}</p>
            </div>
          )}

          <form className="space-y-5" onSubmit={handleSubmit}>
            <div className="space-y-2">
              <label htmlFor="email" className="text-sm font-medium text-primary-foreground">
                Email quản trị
              </label>
              <div className="relative">
                <Mail className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
                <input
                  id="email"
                  type="email"
                  autoComplete="email"
                  className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
                  placeholder="admin@truyencv.com"
                  value={email}
                  onChange={(event) => setEmail(event.target.value)}
                  required
                />
              </div>
            </div>

            <div className="space-y-2">
              <label htmlFor="password" className="text-sm font-medium text-primary-foreground">
                Mật khẩu
              </label>
              <div className="relative">
                <Lock className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
                <input
                  id="password"
                  type="password"
                  autoComplete="current-password"
                  className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
                  placeholder="••••••••"
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                  required
                />
              </div>
            </div>

            <button
              type="submit"
              disabled={loginMutation.isPending}
              className="inline-flex w-full items-center justify-center gap-2 rounded-full border border-primary/50 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
            >
              {loginMutation.isPending && (
                <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
              )}
              Đăng nhập quản trị
            </button>
          </form>

          <p className="text-center text-xs text-surface-foreground/60">
            Quên mật khẩu? Liên hệ quản trị viên hệ thống để được hỗ trợ cấp lại quyền truy cập.
          </p>

          <p className="text-center text-xs text-surface-foreground/60">
            Bạn là người đọc? <Link href={"/user/auth/login" as Route} className="underline">Đăng nhập dành cho người dùng</Link>.
          </p>
        </div>
      </div>
    </div>
  );
};