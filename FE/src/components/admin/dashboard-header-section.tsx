import { CalendarClock, LayoutDashboard, Loader2 } from "lucide-react";

type DashboardHeaderSectionProps = {
  isFetching: boolean;
  onRefetch: () => void;
  isMock: boolean;
};

const DashboardHeaderSection = ({ isFetching, onRefetch, isMock }: DashboardHeaderSectionProps) => (
  <section className="space-y-4">
    <header className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
      <div>
        <p className="flex items-center gap-2 text-xs uppercase tracking-[0.4em] text-primary/80">
          <LayoutDashboard className="h-3.5 w-3.5" />
          Bảng điều khiển tổng quan
        </p>
        <h2 className="mt-2 text-2xl font-semibold text-primary-foreground">Tình trạng hệ thống và nội dung</h2>
        <p className="mt-1 text-sm text-surface-foreground/60">
          Theo dõi nhanh kho truyện, danh mục và hoạt động người dùng với dữ liệu trực tiếp từ API quản trị.
        </p>
      </div>
      <button
        type="button"
        onClick={onRefetch}
        className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary/15 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/25"
        disabled={isFetching}
      >
        {isFetching ? <Loader2 className="h-3.5 w-3.5 animate-spin" /> : <CalendarClock className="h-4 w-4" />}
        {isFetching ? "Đang làm mới" : "Làm mới số liệu"}
      </button>
    </header>

    {isMock && (
      <div className="rounded-xl border border-amber-400/50 bg-amber-100/10 px-4 py-3 text-sm text-amber-500">
        Hiện đang hiển thị dữ liệu minh họa vì API trả về lỗi. Hãy kiểm tra lại cấu hình backend nếu cần.
      </div>
    )}
  </section>
);

export default DashboardHeaderSection;
