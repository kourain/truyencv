"use client";
import { Route } from "next";
import FirebaseLoginButton from "./Firebase";
import Link from "next/link";
import { AlertCircle, Check, Lock, LucideIcon, Mail, Shield, User } from "lucide-react";
import { FormEvent, useEffect, useState } from "react";
import { clearAuthTokens } from "@helpers/authTokens";
import { login } from "@services/auth.service";
import { useRouter, useSearchParams } from "next/dist/client/components/navigation";
import { useAuth } from "@hooks/useAuth";
import { useToast } from "@components/providers/ToastProvider";
import { UserRole } from "@const/enum/role";
import { useMutation } from "@tanstack/react-query";

type LoginVariant = "user" | "admin";

type VariantConfig = {
  role: UserRole;
  defaultFallback: Route;
  icon: LucideIcon;
  headingTitle: string;
  successToast: { title: string; description: string };
  firebaseSuccessToast: { title: string; description: string };
  firebaseNotConfiguredDescription: string;
  submitLabel: string;
  emailLabel: string;
  emailPlaceholder: string;
};

const LOGIN_VARIANT_CONFIG: Record<LoginVariant, VariantConfig> = {
  user: {
    role: UserRole.User,
    defaultFallback: "/user" as Route,
    icon: User,
    headingTitle: "Đăng nhập người dùng",
    successToast: {
      title: "Đăng nhập thành công",
      description: "Chào mừng bạn trở lại TruyenCV."
    },
    firebaseSuccessToast: {
      title: "Đăng nhập thành công",
      description: "Chào mừng bạn trở lại TruyenCV."
    },
    firebaseNotConfiguredDescription: "Liên hệ quản trị viên để kích hoạt đăng nhập Firebase.",
    submitLabel: "Đăng nhập",
    emailLabel: "Email",
    emailPlaceholder: "ban@example.com"
  },
  admin: {
    role: UserRole.Admin,
    defaultFallback: "/admin" as Route,
    icon: Shield,
    headingTitle: "Đăng nhập quản trị",
    successToast: {
      title: "Đăng nhập quản trị thành công",
      description: "Chào mừng quay lại bảng điều khiển."
    },
    firebaseSuccessToast: {
      title: "Đăng nhập quản trị thành công",
      description: "Đã xác thực thông qua Firebase."
    },
    firebaseNotConfiguredDescription: "Liên hệ quản trị viên hệ thống để bật đăng nhập Firebase.",
    submitLabel: "Đăng nhập quản trị",
    emailLabel: "Email quản trị",
    emailPlaceholder: "admin@truyencv.com"
  }
};

const MISSING_FIELDS_MESSAGE = "Vui lòng nhập đầy đủ email và mật khẩu";
const GENERIC_LOGIN_ERROR = "Không thể đăng nhập. Vui lòng kiểm tra lại thông tin hoặc thử lại sau.";
const GENERIC_FIREBASE_ERROR = "Không thể đăng nhập bằng Firebase. Vui lòng thử lại sau.";
export const LoginContent = ({ variant }: { variant: LoginVariant }) => {
  const config = LOGIN_VARIANT_CONFIG[variant];
  const [isSuccessed, setIsSuccessed] = useState(false);
  const router = useRouter();
  const searchParams = useSearchParams();
  const auth = useAuth();
  const { pushToast } = useToast();

  const redirectParam = (searchParams?.get("redirect") as Route | null) ?? null;
  const fallback = (redirectParam ?? config.defaultFallback) as Route;

  const registered = variant === "user" && searchParams?.get("registered") === "1";
  const resetCompleted = variant === "user" && searchParams?.get("reset") === "1";

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [isFirebasePending, setIsFirebasePending] = useState(false);

  useEffect(() => {
    if (auth?.isAuthenticated && auth?.roles.includes(config.role)) {
      router.replace(fallback);
    }
  }, [auth, config.role, fallback, router]);

  useEffect(() => {
    if (!registered && !resetCompleted) {
      return;
    }

    if (variant !== "user") {
      return;
    }

    if (registered) {
      pushToast({
        title: "Đăng ký thành công",
        description: "Vui lòng đăng nhập để tiếp tục trải nghiệm.",
        variant: "success"
      });
      return;
    }

    pushToast({
      title: "Đặt lại mật khẩu thành công",
      description: "Đăng nhập bằng mật khẩu mới của bạn để tiếp tục.",
      variant: "success"
    });
  }, [registered, resetCompleted, variant, pushToast]);

  const loginMutation = useMutation({
    mutationFn: (payload: LoginRequest) => login(payload),
    onSuccess: async (response) => {
      setError(null);
      setIsSuccessed(true);
      pushToast({
        title: config.successToast.title,
        description: config.successToast.description,
        variant: "success"
      });
      await auth.updateAuthStateFromAccessToken(response.access_token);
      router.replace(fallback);
    },
    onError: async (mutationError: unknown) => {
      await clearAuthTokens();
      const fallbackMessage = GENERIC_LOGIN_ERROR;
      if (typeof mutationError === "object" && mutationError !== null && "response" in mutationError) {
        const apiError = mutationError as {
          response?: { data?: { message?: string } };
        };

        const message = apiError.response?.data?.message;
        setError(message ?? fallbackMessage);
        pushToast({
          title: "Đăng nhập thất bại",
          description: message ?? fallbackMessage,
          variant: "error"
        });
      } else {
        setError(fallbackMessage);
        pushToast({
          title: "Đăng nhập thất bại",
          description: fallbackMessage,
          variant: "error"
        });
      }
    }
  });

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);

    if (!email || !password) {
      setError(MISSING_FIELDS_MESSAGE);
      pushToast({
        title: "Thiếu thông tin",
        description: MISSING_FIELDS_MESSAGE,
        variant: "warning"
      });
      return;
    }

    loginMutation.mutate({ email, password });
  };

  const Icon = config.icon;

  return (
    <div className="flex min-h-screen items-center justify-center bg-surface text-surface-foreground">
      <div className="relative w-full max-w-lg overflow-hidden rounded-xl border border-surface-muted bg-surface/80 shadow-xl">
        <div className="absolute -top-40 -left-32 h-64 w-64 rounded-full bg-primary/30 blur-3xl" />
        <div className="absolute -bottom-32 -right-24 h-72 w-72 rounded-full bg-primary/20 blur-3xl" />
        <div className="relative z-10 flex flex-col gap-6 px-8 py-10">
          <div className="flex items-center gap-3">
            <span className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10 text-primary">
              <Icon className="h-6 w-6" />
            </span>
            <div>
              <p className="text-xs uppercase tracking-[0.4em] text-primary/80">TruyenCV</p>
              <h1 className="text-2xl font-semibold text-primary-foreground">{config.headingTitle}</h1>
            </div>
          </div>

          {error && (
            <div className="flex items-start gap-3 rounded-lg border border-red-500/40 bg-red-500/10 px-4 py-3 text-sm text-red-200">
              <AlertCircle className="mt-0.5 h-4 w-4 flex-none" />
              <p>{error}</p>
            </div>
          )}
          {isSuccessed ? (
            <>
              <div className="flex flex-col items-center gap-3 rounded-lg border border-green-500/40 bg-green-500/10 px-6 py-8 text-center text-sm text-green-200">
                <span className="flex h-10 w-10 items-center justify-center rounded-full bg-green-500/20 text-green-400">
                  <Check className="h-5 w-5" />
                </span>
                <p>Đăng nhập thành công! Chuyển hướng...</p>
              </div>
            </>
          ) : (
            <>
              <form className="space-y-5" onSubmit={handleSubmit}>
                <div className="space-y-2">
                  <label htmlFor="email" className="text-sm font-medium text-primary-foreground">
                    {config.emailLabel}
                  </label>
                  <div className="relative">
                    <Mail className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
                    <input
                      id="email"
                      type="email"
                      autoComplete="email"
                      className="w-full rounded-lg border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
                      placeholder={config.emailPlaceholder}
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
                      className="w-full rounded-lg border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
                      placeholder="••••••••"
                      value={password}
                      onChange={(event) => setPassword(event.target.value)}
                      required
                    />
                  </div>
                </div>

                {variant === "user" && (
                  <p className="text-right text-xs text-surface-foreground/60">
                    <Link href={"/user/auth/reset-password" as Route} className="underline">
                      Quên mật khẩu?
                    </Link>
                  </p>
                )}

                <button
                  type="submit"
                  disabled={loginMutation.isPending || isFirebasePending}
                  className="inline-flex w-full items-center justify-center gap-2 rounded-lg border border-primary/50 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
                >
                  {loginMutation.isPending && (
                    <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
                  )}
                  {config.submitLabel}
                </button>
              </form>

              <div className="relative flex items-center justify-center">
                <span className="h-px w-full bg-surface-muted/60" />
                <span className="absolute bg-surface px-2 text-[11px] uppercase tracking-[0.3em] text-surface-foreground/40">
                  Hoặc
                </span>
              </div>

              <div className="flex flex-col gap-2">
                <FirebaseLoginButton
                  provider="google"
                  fallback={fallback}
                  successToast={config.firebaseSuccessToast}
                  notConfiguredDescription={config.firebaseNotConfiguredDescription}
                  genericErrorMessage={GENERIC_FIREBASE_ERROR}
                  setError={setError}
                  onPendingChange={setIsFirebasePending}
                  disabled={loginMutation.isPending}
                />
                <FirebaseLoginButton
                  provider="facebook"
                  fallback={fallback}
                  successToast={config.firebaseSuccessToast}
                  notConfiguredDescription={config.firebaseNotConfiguredDescription}
                  genericErrorMessage={GENERIC_FIREBASE_ERROR}
                  setError={setError}
                  onPendingChange={setIsFirebasePending}
                  disabled={loginMutation.isPending}
                />
              </div>

              {variant === "user" ? (
                <>
                  <p className="text-center text-xs text-surface-foreground/60">
                    Gặp vấn đề khi đăng nhập?{" "}
                    <Link href={"/user/auth/reset-password" as Route} className="underline">
                      Đặt lại mật khẩu
                    </Link>{" "}
                    hoặc liên hệ đội hỗ trợ của TruyenCV để được trợ giúp.
                  </p>
                  <p className="text-center text-xs text-surface-foreground/60">
                    Chưa có tài khoản?{" "}
                    <Link href={"/user/auth/register" as Route} className="underline">
                      Đăng ký ngay
                    </Link>
                    .
                  </p>
                </>
              ) : (
                <>
                  <p className="text-center text-xs text-surface-foreground/60">
                    Quên mật khẩu? Liên hệ quản trị viên hệ thống để được hỗ trợ cấp lại quyền truy cập.
                  </p>
                  <p className="text-center text-xs text-surface-foreground/60">
                    Bạn là người đọc?{" "}
                    <Link href={"/user/auth/login" as Route} className="underline">
                      Đăng nhập dành cho người dùng
                    </Link>
                    .
                  </p>
                </>
              )}
            </>)}
        </div>
      </div>
    </div>
  );
};