import Link from "next/link";

const primaryLinks = [
  { label: "Điều khoản", href: "/user/terms-of-service" },
  { label: "Bảo mật", href: "/user/privacy-policy" },
  { label: "Hỗ trợ", href: "mailto:ht.kourain@gmail.com" }
];

export default function Footer() {
  return (
    <footer className="border-t border-surface-muted/60 bg-surface">
      <div className="mx-auto flex max-w-6xl flex-col gap-8 px-6 py-10 md:flex-row md:items-start md:justify-between">
        <div className="space-y-3">
          <p className="text-lg font-semibold text-primary-foreground">TruyenCV</p>
          <p className="text-sm text-surface-foreground/70">
            Không gian đọc truyện hiện đại với trải nghiệm thống nhất trên mọi thiết bị.
          </p>
          <p className="text-sm text-surface-foreground/70">
            Chú ý: 
            Đây chỉ là một đề tài đồ án tốt nghiệp của sinh viên chuyên ngành CNTT, dữ liệu trang web chỉ mang mục đích trình diễn, không phải là trang web mang tính thương mại.
            Bất kỳ vấn đề gì vui lòng liên hệ qua email ht.kourain@gmail.com
          </p>
        </div>
        <div className="flex flex-wrap gap-6 text-sm text-surface-foreground/70">
          {primaryLinks.map((link) => (
            <Link
              key={link.href}
              href={link.href}
              className="transition hover:text-primary"
            >
              {link.label}
            </Link>
          ))}
        </div>
      </div>
      <div className="border-t border-surface-muted/50">
        <div className="mx-auto max-w-6xl px-6 py-4 text-xs text-surface-foreground/60">
          © {new Date().getFullYear()} TruyenCV. All rights reserved.
        </div>
      </div>
    </footer>
  );
}
