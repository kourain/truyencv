"use client";

import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import type { AxiosError } from "axios";

import { useToast } from "@components/providers/ToastProvider";
import { changePassword } from "@services/user/profile.service";

interface ChangePasswordFormProps {
  username: string;
  onSuccess: () => void;
}

const ChangePasswordForm = ({ username, onSuccess }: ChangePasswordFormProps) => {
  const { pushToast } = useToast();
  const [form, setForm] = useState<ChangePasswordPayload>({
    current_password: "",
    new_password: "",
  });
  const [confirmPassword, setConfirmPassword] = useState<string>("");
  const [error, setError] = useState<string | null>(null);

  const mutation = useMutation({
    mutationFn: changePassword,
    onSuccess: () => {
      setError(null);
      setForm({ current_password: "", new_password: "" });
      setConfirmPassword("");
      pushToast({
        title: "Đổi mật khẩu thành công",
        description: "Bạn có thể sử dụng mật khẩu mới ngay lập tức.",
        variant: "success",
      });
      onSuccess();
    },
    onError: (error: unknown) => {
      if (typeof error === "object" && error !== null && "response" in error) {
        const apiError = error as AxiosError<{ message?: string }>;
        const message = apiError.response?.data?.message;
        const fallbackMessage = "Không thể đổi mật khẩu. Vui lòng thử lại sau.";
        setError(message ?? fallbackMessage);
        pushToast({
          title: "Đổi mật khẩu thất bại",
          description: message ?? fallbackMessage,
          variant: "error",
        });
      } else {
        const fallbackMessage = "Không thể đổi mật khẩu. Vui lòng thử lại sau.";
        setError(fallbackMessage);
        pushToast({
          title: "Đổi mật khẩu thất bại",
          description: fallbackMessage,
          variant: "error",
        });
      }
    },
  });

  const passwordsMismatch = Boolean(form.new_password) && form.new_password !== confirmPassword;
  const inputDisabled = mutation.isPending;

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (passwordsMismatch) {
      setError("Mật khẩu mới và xác nhận mật khẩu không khớp");
      return;
    }

    setError(null);
    mutation.mutate({ ...form });
  };

  const updateField = (key: keyof ChangePasswordPayload) => (value: string) => {
    setForm((prev: ChangePasswordPayload) => ({ ...prev, [key]: value }));
  };

  return (
    <section className="rounded-3xl border border-slate-700/70 bg-slate-900/70 p-6 shadow-xl">
      <header className="mb-6">
        <h3 className="text-lg font-semibold text-slate-100">Đổi mật khẩu</h3>
        <p className="text-xs text-slate-400">Đặt lại mật khẩu thường xuyên để đảm bảo tài khoản <code>{username}</code> luôn an toàn.</p>
      </header>

      <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
        <label className="flex flex-col gap-2 text-xs text-slate-300">
          <span>Mật khẩu hiện tại</span>
          <input
            type="password"
            value={form.current_password}
            onChange={(event) => updateField("current_password")(event.target.value)}
            disabled={inputDisabled}
            className="w-full rounded-xl border border-slate-700 bg-slate-950/40 px-4 py-3 text-sm text-slate-100 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/40 disabled:opacity-70"
            placeholder="Nhập mật khẩu đang dùng"
            required
          />
        </label>

        <label className="flex flex-col gap-2 text-xs text-slate-300">
          <span>Mật khẩu mới</span>
          <input
            type="password"
            value={form.new_password}
            onChange={(event) => updateField("new_password")(event.target.value)}
            disabled={inputDisabled}
            className="w-full rounded-xl border border-slate-700 bg-slate-950/40 px-4 py-3 text-sm text-slate-100 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/40 disabled:opacity-70"
            placeholder="Tối thiểu 8 ký tự, bao gồm số và chữ cái"
            required
          />
        </label>

        <label className="flex flex-col gap-2 text-xs text-slate-300">
          <span>Xác nhận mật khẩu mới</span>
          <input
            type="password"
            value={confirmPassword}
            onChange={(event) => setConfirmPassword(event.target.value)}
            disabled={inputDisabled}
            className={`w-full rounded-xl border bg-slate-950/40 px-4 py-3 text-sm text-slate-100 outline-none transition focus:ring-2 disabled:opacity-70 ${
              passwordsMismatch
                ? "border-rose-500/60 focus:border-rose-500 focus:ring-rose-500/30"
                : "border-slate-700 focus:border-primary focus:ring-primary/40"
            }`}
            placeholder="Nhập lại mật khẩu mới"
            required
          />
        </label>

  {error && <p className="text-xs font-medium text-rose-400">{error}</p>}

        <button
          type="submit"
          disabled={inputDisabled}
          className="mt-2 inline-flex items-center justify-center gap-2 rounded-full bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:opacity-70"
        >
          {mutation.isPending && <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />}
          Cập nhật mật khẩu
        </button>
      </form>
    </section>
  );
};

export default ChangePasswordForm;
