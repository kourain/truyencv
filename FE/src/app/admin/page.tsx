import { Activity, AlertTriangle, ArrowUpRight, BookOpen, CalendarClock, MessageCircle, ShieldCheck, Sparkles, TrendingDown, TrendingUp, Users } from "lucide-react";

const metrics = [
  {
    label: "Tổng truyện",
    value: "2.435",
    trend: 12.4,
    trendLabel: "trong 7 ngày",
    icon: BookOpen
  },
  {
    label: "Người dùng hoạt động",
    value: "18.920",
    trend: 6.1,
    trendLabel: "so với tuần trước",
    icon: Users
  },
  {
    label: "Đánh giá mới",
    value: "642",
    trend: 4.8,
    trendLabel: "tỷ lệ 5★",
    icon: MessageCircle
  },
  {
    label: "Thời gian phản hồi API",
    value: "98 ms",
    trend: -3.4,
    trendLabel: "đã nhanh hơn",
    icon: Activity
  }
];

const topComics = [
  {
    title: "Đại Lục Pháp Sư",
    author: "Hàn Vũ",
    category: "Huyền huyễn",
    views: "182K",
    completion: 78
  },
  {
    title: "Tái Sinh Thành Thần",
    author: "Mặc Thiên",
    category: "Tiên hiệp",
    views: "156K",
    completion: 54
  },
  {
    title: "Học Viện Anh Hùng",
    author: "Kuro",
    category: "Hành động",
    views: "144K",
    completion: 92
  }
];

const activities = [
  {
    id: "ACT-243",
    message: "Người dùng @phamtrong nâng cấp vai trò Moderator",
    time: "5 phút trước",
    status: "positive"
  },
  {
    id: "ACT-242",
    message: "Đăng tải chương mới cho 'Đại Lục Pháp Sư'",
    time: "16 phút trước",
    status: "neutral"
  },
  {
    id: "ACT-240",
    message: "Cảnh báo: Tăng đột biến lượt truy cập từ CDN",
    time: "1 giờ trước",
    status: "warning"
  }
];

const maintenance = [
  {
    id: "MT-12",
    title: "Tối ưu hóa truy vấn tìm kiếm nâng cao",
    due: "Hạn: 11/10",
    owner: "@minhquang"
  },
  {
    id: "MT-11",
    title: "Đồng bộ cache chương truyện dài",
    due: "Hạn: 12/10",
    owner: "@vuthao"
  },
  {
    id: "MT-10",
    title: "Rà soát chính sách bình luận độc hại",
    due: "Hạn: 13/10",
    owner: "@ngocanh"
  }
];

const systemHealth = [
  {
    label: "API",
    status: "Ổn định",
    detail: "Tỷ lệ thành công 99.2%",
    icon: ShieldCheck
  },
  {
    label: "Redis Cache",
    status: "Cần chú ý",
    detail: "Tỉ lệ hit 83%",
    icon: AlertTriangle
  },
  {
    label: "CDN",
    status: "Tốt",
    detail: "Latency trung bình 60 ms",
    icon: Sparkles
  }
];

const AdminDashboardPage = () => {
  return (
    <div className="space-y-10">
      <section id="tong-quan" className="space-y-6">
        <header className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
          <div>
            <p className="text-xs uppercase tracking-[0.4em] text-primary/80">Báo cáo tức thì</p>
            <h2 className="mt-2 text-2xl font-semibold text-primary-foreground">Tổng quan hệ sinh thái truyện</h2>
          </div>
          <button
            type="button"
            className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary/15 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/25"
          >
            <CalendarClock className="h-4 w-4" />
            Xuất báo cáo tuần
          </button>
        </header>
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          {metrics.map(({ label, value, trend, trendLabel, icon: Icon }) => (
            <article key={label} className="rounded-2xl border border-surface-muted bg-surface/60 p-5 shadow-glow">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-xs uppercase tracking-wide text-surface-foreground/60">{label}</p>
                  <p className="mt-3 text-3xl font-semibold text-primary-foreground">{value}</p>
                </div>
                <span className="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10 text-primary">
                  <Icon className="h-6 w-6" />
                </span>
              </div>
              <p className="mt-4 flex items-center gap-1 text-xs text-surface-foreground/70">
                {trend >= 0 ? (
                  <ArrowUpRight className="h-3.5 w-3.5 text-emerald-400" />
                ) : (
                  <TrendingDown className="h-3.5 w-3.5 text-red-400" />
                )}
                <span className={trend >= 0 ? "text-emerald-400" : "text-red-400"}>{trend}%</span>
                <span>{trendLabel}</span>
              </p>
            </article>
          ))}
        </div>
      </section>

      <section id="quan-ly-truyen" className="grid gap-6 lg:grid-cols-[1.1fr_0.9fr]">
        <div className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
          <header className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <p className="text-xs uppercase tracking-[0.3em] text-primary/70">Đầu bảng</p>
              <h3 className="text-xl font-semibold text-primary-foreground">Top truyện theo lượt đọc</h3>
            </div>
            <button
              type="button"
              className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-3 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
            >
              Xem toàn bộ
            </button>
          </header>
          <div className="mt-6 space-y-4">
            {topComics.map(({ title, author, category, views, completion }) => (
              <article
                key={title}
                className="rounded-2xl border border-surface-muted/60 bg-surface px-4 py-4 transition hover:border-primary/60"
              >
                <div className="flex flex-wrap items-center justify-between gap-4">
                  <div>
                    <h4 className="text-lg font-semibold text-primary-foreground">{title}</h4>
                    <p className="text-sm text-surface-foreground/70">
                      {author} • {category}
                    </p>
                  </div>
                  <div className="text-right">
                    <p className="text-xs uppercase tracking-wide text-surface-foreground/60">Lượt đọc</p>
                    <p className="text-base font-semibold text-primary-foreground">{views}</p>
                  </div>
                </div>
                <div className="mt-4 h-2 w-full overflow-hidden rounded-full bg-surface-muted/60">
                  <div className="h-full rounded-full bg-primary" style={ { width: `${completion}%` } } />
                </div>
                <p className="mt-2 text-xs text-surface-foreground/60">Hoàn thiện nội dung {completion}%</p>
              </article>
            ))}
          </div>
        </div>
        <div className="space-y-6">
          <article className="rounded-2xl border border-primary/40 bg-primary/10 p-6">
            <header className="flex items-center gap-3">
              <Sparkles className="h-5 w-5 text-primary" />
              <div>
                <h3 className="text-lg font-semibold text-primary-foreground">Gợi ý tối ưu hóa</h3>
                <p className="text-xs text-primary/80">Thật tuyệt! Nội dung dài hoạt động tốt với người dùng mới.</p>
              </div>
            </header>
            <ul className="mt-4 space-y-3 text-sm text-surface-foreground/80">
              <li>• Ưu tiên làm mới chương cho truyện huyền huyễn.</li>
              <li>• Khuyến khích tác giả cập nhật ảnh bìa chất lượng cao.</li>
              <li>• Tạo chiến dịch gợi ý truyện cho người dùng quay lại.</li>
            </ul>
          </article>

          <article className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
            <header className="flex items-center gap-2 text-primary-foreground">
              <TrendingUp className="h-4 w-4" />
              <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Dòng sự kiện</h3>
            </header>
            <div className="mt-4 space-y-4">
              {activities.map(({ id, message, time, status }) => (
                <div key={id} className="rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
                  <p className="text-sm font-medium text-primary-foreground">{message}</p>
                  <div className="mt-2 flex items-center justify-between text-xs text-surface-foreground/60">
                    <span>{time}</span>
                    <span
                      className={
                        status === "positive"
                          ? "text-emerald-400"
                          : status === "warning"
                            ? "text-amber-300"
                            : "text-surface-foreground/60"
                      }
                    >
                      {status === "positive" ? "Ổn định" : status === "warning" ? "Cảnh báo" : "Hoạt động"}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </article>
        </div>
      </section>

      <section id="nguoi-dung" className="grid gap-6 lg:grid-cols-[1.05fr_0.95fr]">
        <article className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
          <header className="flex items-center justify-between">
            <div>
              <p className="text-xs uppercase tracking-[0.3em] text-primary/70">Quản trị viên nổi bật</p>
              <h3 className="text-xl font-semibold text-primary-foreground">Bảng phân quyền</h3>
            </div>
            <button
              type="button"
              className="inline-flex items-center gap-2 rounded-full border border-primary/40 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/10"
            >
              Thêm quản trị viên
            </button>
          </header>
          <div className="mt-6 space-y-3">
            {[
              { name: "Trần Nhật Minh", role: "Super Admin", managed: 42 },
              { name: "Vũ Thùy Dung", role: "Content Manager", managed: 31 },
              { name: "Lê Phương", role: "Moderator", managed: 18 }
            ].map(({ name, role, managed }) => (
              <div
                key={name}
                className="flex items-center justify-between rounded-2xl border border-surface-muted/70 bg-surface px-4 py-3"
              >
                <div>
                  <p className="text-sm font-semibold text-primary-foreground">{name}</p>
                  <p className="text-xs text-surface-foreground/60">{role}</p>
                </div>
                <p className="text-xs uppercase tracking-wide text-surface-foreground/60">
                  Quản lý {managed} truyện
                </p>
              </div>
            ))}
          </div>
        </article>

        <article className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
          <header className="flex items-center gap-2 text-primary-foreground">
            <MessageCircle className="h-5 w-5" />
            <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Phản hồi mới nhất</h3>
          </header>
          <div className="mt-5 space-y-4 text-sm text-surface-foreground/80">
            <div className="rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
              <p className="font-medium text-primary-foreground">@lamngoc:</p>
              <p className="mt-1">"Rất cần bộ lọc chương nâng cao cho truyện dài"</p>
              <p className="mt-2 text-xs text-surface-foreground/60">2 giờ trước • Từ trang chi tiết truyện</p>
            </div>
            <div className="rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
              <p className="font-medium text-primary-foreground">@kietpham:</p>
              <p className="mt-1">"Đề xuất thêm chủ đề tranh biện để tăng tương tác"</p>
              <p className="mt-2 text-xs text-surface-foreground/60">4 giờ trước • Từ diễn đàn thảo luận</p>
            </div>
            <div className="rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
              <p className="font-medium text-primary-foreground">@thuylinh:</p>
              <p className="mt-1">"Chức năng đọc offline chạy mượt hơn sau bản cập nhật"</p>
              <p className="mt-2 text-xs text-surface-foreground/60">Hôm qua • Qua email hỗ trợ</p>
            </div>
          </div>
        </article>
      </section>

      <section id="he-thong" className="grid gap-6 lg:grid-cols-[1fr_1fr]">
        <article className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
          <header className="flex items-center gap-2 text-primary-foreground">
            <ShieldCheck className="h-5 w-5" />
            <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Sức khỏe hệ thống</h3>
          </header>
          <ul className="mt-5 space-y-3">
            {systemHealth.map(({ label, status, detail, icon: Icon }) => (
              <li key={label} className="flex items-center justify-between rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
                <div className="flex items-center gap-3">
                  <span className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-primary">
                    <Icon className="h-5 w-5" />
                  </span>
                  <div>
                    <p className="text-sm font-semibold text-primary-foreground">{label}</p>
                    <p className="text-xs text-surface-foreground/60">{detail}</p>
                  </div>
                </div>
                <span className="text-xs font-semibold uppercase tracking-wide text-primary/70">{status}</span>
              </li>
            ))}
          </ul>
        </article>
        <article className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
          <header className="flex items-center gap-2 text-primary-foreground">
            <CalendarClock className="h-5 w-5" />
            <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Lịch bảo trì</h3>
          </header>
          <div className="mt-5 space-y-3">
            {maintenance.map(({ id, title, due, owner }) => (
              <div key={id} className="flex items-center justify-between rounded-xl border border-surface-muted/70 bg-surface px-4 py-3">
                <div>
                  <p className="text-sm font-semibold text-primary-foreground">{title}</p>
                  <p className="text-xs text-surface-foreground/60">{id} • {owner}</p>
                </div>
                <span className="text-xs font-semibold uppercase tracking-wide text-primary/70">{due}</span>
              </div>
            ))}
          </div>
        </article>
      </section>
    </div>
  );
};

export default AdminDashboardPage;
