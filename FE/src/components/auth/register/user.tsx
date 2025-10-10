"use client";

import { useState, FormEvent, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import type { Route } from "next";
import { useMutation } from "@tanstack/react-query";

import { register } from "@services/auth.service";
import { AlertCircle, AtSign, IdCard, Lock, Mail, Phone, UserPlus } from "lucide-react";

export const UserRegisterContent = () => {
  const router = useRouter();
  const [name, setName] = useState("");
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [phone, setPhone] = useState("");
  const [error, setError] = useState<string | null>(null);

  const registerMutation = useMutation({
    mutationFn: (payload: RegisterRequest) => register(payload),
    onSuccess: () => {
      setError(null);
      router.replace("/user" as Route);
    },
    onError: (mutationError: unknown) => {
      let message = "Không thể đăng ký. Vui lòng thử lại sau.";
      if (typeof mutationError === "object" && mutationError !== null && "response" in mutationError) {
        const apiError = mutationError as { response?: { data?: { message?: string } } };
        message = apiError.response?.data?.message ?? message;
      }
      setError(message);
    }
  });

  useEffect(() => {
    setError(null);
  }, [name, userName, email, password, confirmPassword, phone]);

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!name || !userName || !email || !password || !confirmPassword || !phone) {
      setError("Vui lòng điền đầy đủ thông tin bắt buộc.");
      return;
    }

    if (password !== confirmPassword) {
      setError("Mật khẩu xác nhận không khớp.");
      return;
    }

    if (password.length < 8) {
      setError("Mật khẩu cần ít nhất 8 ký tự.");
      return;
    }

    const normalizedPhone = phone.trim();
    if (!/^[0-9]{9,15}$/.test(normalizedPhone)) {
      setError("Số điện thoại phải gồm 9-15 chữ số.");
      return;
    }

    registerMutation.mutate({
      name,
      user_name: userName,
      email,
      password,
      phone: normalizedPhone
    });
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-surface text-surface-foreground">
      <div className="relative w-full max-w-xl overflow-hidden rounded-3xl border border-surface-muted bg-surface/70 shadow-glow backdrop-blur-xl">
        <div className="absolute -top-40 -left-32 h-64 w-64 rounded-full bg-primary/30 blur-3xl" />
        <div className="absolute -bottom-32 -right-24 h-72 w-72 rounded-full bg-primary/20 blur-3xl" />
        <div className="relative z-10 flex flex-col gap-6 px-10 py-12">
          <div className="flex items-center gap-3">
            <span className="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10 text-primary">
              <UserPlus className="h-6 w-6" />
            </span>
            <div>
              <p className="text-xs uppercase tracking-[0.4em] text-primary/80">TruyenCV</p>
              <h1 className="text-2xl font-semibold text-primary-foreground">Tạo tài khoản mới</h1>
            </div>
          </div>

          {error && (
            <div className="flex items-start gap-3 rounded-2xl border border-red-500/40 bg-red-500/10 px-4 py-3 text-sm text-red-200">
              <AlertCircle className="mt-0.5 h-4 w-4 flex-none" />
              <p>{error}</p>
            </div>
          )}

          <form className="space-y-5" onSubmit={handleSubmit}>
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <label htmlFor="name" className="text-sm font-medium text-primary-foreground">
                  Họ và tên
                </label>
                <div className="relative">
                  <IdCard className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
                  <input
                    id="name"
                    type="text"
                    autoComplete="name"
                    className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
                    placeholder="Nguyễn Văn A"
                    value={name}
                    onChange={(event) => setName(event.target.value)}
                    required
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label htmlFor="userName" className="text-sm font-medium text-primary-foreground">
                  Tên hiển thị
                </label>
                <div className="relative">
                  <AtSign className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
                  <input
                    id="userName"
                    type="text"
                    autoComplete="username"
                    className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
                    placeholder="truyencv_reader"
                    value={userName}
                    onChange={(event) => setUserName(event.target.value)}
                    required
                  />
                </div>
              </div>
            </div>

            <div className="space-y-2">
              <label htmlFor="email" className="text-sm font-medium text-primary-foreground">
                Email
              </label>
              <div className="relative">
                <Mail className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
                <input
                  id="email"
                  type="email"
                  autoComplete="email"
                  className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
                  placeholder="ban@example.com"
                  value={email}
                  onChange={(event) => setEmail(event.target.value)}
                  required
                />
              </div>
            </div>

            <div className="space-y-2">
              <label htmlFor="phone" className="text-sm font-medium text-primary-foreground">
                Số điện thoại
              </label>
              <div className="relative">
                <Phone className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
                <input
                  id="phone"
                  type="tel"
                  autoComplete="tel"
                  className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
                  placeholder="0912345678"
                  value={phone}
                  onChange={(event) => setPhone(event.target.value)}
                  required
                />
              </div>
            </div>

            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <label htmlFor="password" className="text-sm font-medium text-primary-foreground">
                  Mật khẩu
                </label>
                <div className="relative">
                  <Lock className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
                  <input
                    id="password"
                    type="password"
                    autoComplete="new-password"
                    className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
                    placeholder="••••••••"
                    value={password}
                    onChange={(event) => setPassword(event.target.value)}
                    required
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label htmlFor="confirmPassword" className="text-sm font-medium text-primary-foreground">
                  Xác nhận mật khẩu
                </label>
                <div className="relative">
                  <Lock className="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-primary" />
                  <input
                    id="confirmPassword"
                    type="password"
                    autoComplete="new-password"
                    className="w-full rounded-full border border-surface-muted bg-surface px-11 py-3 text-sm text-surface-foreground/90 placeholder:text-surface-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary/60"
                    placeholder="••••••••"
                    value={confirmPassword}
                    onChange={(event) => setConfirmPassword(event.target.value)}
                    required
                  />
                </div>
              </div>
            </div>

            <button
              type="submit"
              disabled={registerMutation.isPending}
              className="inline-flex w-full items-center justify-center gap-2 rounded-full border border-primary/50 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
            >
              {registerMutation.isPending && (
                <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
              )}
              Đăng ký tài khoản
            </button>
          </form>

          <p className="text-center text-xs text-surface-foreground/60">
            Đã có tài khoản? <Link href={"/user/auth/login" as Route} className="underline">Đăng nhập ngay</Link>.
          </p>
        </div>
      </div>
    </div>
  );
};

export default UserRegisterContent;
