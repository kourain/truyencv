"use client";

import Link from "next/link";
import { BookOpenText, MessageSquare, ShieldAlert, Sparkles, Ticket, Coins } from "lucide-react";
import { useAuth } from "@hooks/useAuth";
import { formatNumber } from "@helpers/format";
import { useUserProfileQuery } from "@services/user/profile.service";

const actionCards = [
  {
    title: "Quản lý truyện",
    description: "Theo dõi trạng thái kiểm duyệt và chỉnh sửa nội dung truyện bạn đã đăng.",
    href: "/converter/comics",
    icon: BookOpenText,
  },
  {
    title: "Chương & bình luận",
    description: "Tạo chương mới, cập nhật nội dung và xử lý các bình luận vi phạm.",
    href: "/converter/chapters",
    icon: MessageSquare,
  },
  {
    title: "Báo cáo",
    description: "Xem các báo cáo liên quan tới truyện của bạn và phản hồi kịp thời.",
    href: "/converter/reports",
    icon: ShieldAlert,
  },
];

const ConverterDashboard = () => {
  const auth = useAuth();
  const { data: profile } = useUserProfileQuery({ enabled: auth.isAuthenticated });

  return (
    <div className="space-y-8">
      <section className="rounded-3xl border border-surface-muted/60 bg-gradient-to-r from-primary/15 via-primary/5 to-emerald-500/10 p-6 shadow-lg">
        <div className="flex flex-col gap-6 lg:flex-row lg:items-center lg:justify-between">
          <div className="space-y-3">
            <p className="inline-flex items-center gap-2 rounded-full border border-primary/40 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-primary">
              <Sparkles className="h-3.5 w-3.5" />
              Converter workspace
            </p>
            <h1 className="text-2xl font-semibold text-primary-foreground">
              Chào {auth.userProfile.name || "bạn"}, quản lý nội dung dễ dàng hơn
            </h1>
            <p className="max-w-2xl text-sm text-surface-foreground/80">
              Theo dõi truyện đã đăng, cập nhật chương và xử lý báo cáo của độc giả tại một nơi duy nhất. Chúng tôi đã chuẩn bị sẵn các mục quan trọng để bạn truy cập nhanh.
            </p>
          </div>
          <div className="grid grid-cols-2 gap-3 rounded-2xl border border-primary/20 bg-surface/70 p-4 text-sm text-surface-foreground/80">
            <div className="flex flex-col items-start gap-1">
              <span className="inline-flex items-center gap-2 text-xs uppercase tracking-wide text-surface-foreground/60">
                <Coins className="h-4 w-4 text-primary" />
                Xu khả dụng
              </span>
              <span className="text-xl font-semibold text-primary-foreground">
                {formatNumber(Number(profile?.coin ?? 0))}
              </span>
            </div>
            <div className="flex flex-col items-start gap-1">
              <span className="inline-flex items-center gap-2 text-xs uppercase tracking-wide text-surface-foreground/60">
                <Ticket className="h-4 w-4 text-primary" />
                Vé khả dụng
              </span>
              <span className="text-xl font-semibold text-primary-foreground">
                {formatNumber(Number(profile?.key ?? 0))}
              </span>
            </div>
          </div>
        </div>
      </section>

      <section className="grid gap-5 md:grid-cols-3">
        {actionCards.map((card) => (
          <Link
            key={card.href}
            href={card.href}
            className="group flex h-full flex-col gap-3 rounded-2xl border border-surface-muted/60 bg-surface p-5 shadow-sm transition hover:-translate-y-0.5 hover:border-primary/50 hover:shadow-xl"
          >
            <span className="inline-flex h-12 w-12 items-center justify-center rounded-xl bg-primary/10 text-primary">
              <card.icon className="h-6 w-6" />
            </span>
            <div className="space-y-2">
              <h3 className="text-base font-semibold text-primary-foreground">{card.title}</h3>
              <p className="text-sm text-surface-foreground/70">{card.description}</p>
            </div>
            <span className="mt-auto inline-flex items-center gap-2 text-sm font-medium text-primary">
              Đi tới
              <svg viewBox="0 0 20 20" fill="none" className="h-4 w-4 transition group-hover:translate-x-1">
                <path d="M7.5 5L12.5 10L7.5 15" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
              </svg>
            </span>
          </Link>
        ))}
      </section>
    </div>
  );
};

export default ConverterDashboard;
