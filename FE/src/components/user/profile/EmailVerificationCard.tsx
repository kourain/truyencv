"use client";

import { MailCheck, MailWarning } from "lucide-react";

interface EmailVerificationCardProps {
  isVerified: boolean;
  isVerifying: boolean;
  onVerify: () => void;
  lastVerifiedAt?: string | null;
}

const EmailVerificationCard = ({ isVerified, isVerifying, onVerify, lastVerifiedAt }: EmailVerificationCardProps) => {
  if (isVerified) {
    return (
      <section className="rounded-3xl border border-emerald-500/40 bg-emerald-500/10 p-6 shadow-lg">
        <div className="flex items-center gap-3 text-emerald-200">
          <MailCheck className="h-5 w-5" />
          <div>
            <p className="text-sm font-semibold uppercase tracking-wide">Email đã xác thực</p>
            {lastVerifiedAt && (
              <p className="text-xs text-emerald-100/80">Xác thực lần cuối: {new Date(lastVerifiedAt).toLocaleString("vi-VN")}</p>
            )}
          </div>
        </div>
      </section>
    );
  }

  return (
    <section className="rounded-3xl border border-amber-500/50 bg-amber-500/10 p-6 shadow-lg">
      <div className="flex flex-col gap-4">
        <div className="flex items-center gap-3 text-amber-200">
          <MailWarning className="h-5 w-5" />
          <div>
            <p className="text-sm font-semibold uppercase tracking-wide">Email chưa xác thực</p>
            <p className="text-xs text-amber-100/80">
              Vui lòng xác nhận địa chỉ email để bảo vệ tài khoản và nhận thông báo quan trọng từ TruyenCV.
            </p>
          </div>
        </div>
        <button
          type="button"
          onClick={onVerify}
          disabled={isVerifying}
          className="inline-flex w-full items-center justify-center gap-2 rounded-full border border-primary/40 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:opacity-70"
        >
          {isVerifying && <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />}
          Xác nhận email ngay
        </button>
      </div>
    </section>
  );
};

export default EmailVerificationCard;
