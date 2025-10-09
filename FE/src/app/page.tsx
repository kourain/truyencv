"use client";

import { usePingQuery } from "@hooks/usePingQuery";
import { appEnv } from "@const/env";
import { resolveCdnUrl } from "@helpers/httpClient";

const HomePage = () => {
  const { data, isLoading, isError, refetch, isFetching } = usePingQuery();

  return (
    <main className="flex flex-col items-center justify-center gap-10 bg-gradient-to-b from-surface via-surface-muted to-surface px-6 py-24">
      <section className="max-w-2xl rounded-3xl border border-surface-muted bg-surface/80 p-10 text-center shadow-glow backdrop-blur">
        <span className="rounded-full bg-primary/15 px-4 py-1 text-sm font-semibold uppercase tracking-[0.3em] text-primary">
          TruyenCV Frontend
        </span>

        <h1 className="mt-6 text-4xl font-bold text-primary-foreground sm:text-5xl">Cấu hình Next.js đã sẵn sàng</h1>
        <p className="mt-4 text-base text-surface-foreground/80">
          Dự án sử dụng <b>Next.js 14, TailwindCSS, TanStack Query</b> và <b>Axios</b>. Các biến môi trường trong
          <code className="mx-1 rounded bg-surface-muted px-2 py-1 text-xs">.env</code> đã được ánh xạ vào ứng dụng.
        </p>

        <div className="mt-8 grid grid-cols-1 gap-4 sm:grid-cols-3">
          <EnvironmentCard label="Backend" value={appEnv.backendUrl || "Chưa cấu hình"} />
          <EnvironmentCard label="CDN" value={appEnv.cdnUrl || "Chưa cấu hình"} />
          <EnvironmentCard label="Cổng FE" value={String(appEnv.fePort)} />
        </div>

        <button
          type="button"
          className="mt-8 inline-flex items-center justify-center rounded-full bg-primary px-6 py-3 font-semibold text-primary-foreground transition hover:shadow-glow disabled:cursor-progress disabled:bg-primary/60"
          onClick={() => refetch()}
          disabled={isFetching}
        >
          Làm mới trạng thái Backend
        </button>

        <div className="mt-6 rounded-2xl border border-surface-muted bg-surface-muted/40 p-6 text-left">
          <h2 className="text-lg font-semibold text-primary-foreground">Trạng thái backend</h2>
          {isLoading && <p className="mt-3 text-sm text-surface-foreground/70">Đang kiểm tra...</p>}
          {isError && (
            <p className="mt-3 text-sm text-red-400">Không thể kết nối tới backend. Kiểm tra lại biến BACKEND_URL.</p>
          )}
          {data && !isError && (
            <div className="mt-3 space-y-1 text-sm text-surface-foreground/80">
              <p>
                <span className="font-semibold text-primary">GET /ping</span> → {data.message}
              </p>
              <p>
                Base URL: <code className="rounded bg-surface px-2 py-1">{appEnv.backendUrl}</code>
              </p>
            </div>
          )}
        </div>

        {appEnv.cdnUrl && (
          <div className="mt-4 rounded-2xl border border-surface-muted bg-surface-muted/30 p-6 text-left text-sm text-surface-foreground/80">
            <h3 className="font-semibold text-primary-foreground">Ví dụ đường dẫn CDN</h3>
            <p className="mt-2">
              <code className="rounded bg-surface px-2 py-1">
                {resolveCdnUrl("example/thumbnail.jpg")}
              </code>
            </p>
            <p className="mt-2 text-xs text-surface-foreground/60">
              Thay <code>example/thumbnail.jpg</code> bằng đường dẫn thực tế của nội dung truyện để tải từ CDN.
            </p>
          </div>
        )}
      </section>

      <footer className="text-sm text-surface-foreground/60">
        Các tài nguyên frontend sẽ được phục vụ từ cổng <b>{appEnv.fePort}</b>
      </footer>
    </main>
  );
};

interface EnvironmentCardProps {
  label: string;
  value: string;
}

const EnvironmentCard = ({ label, value }: EnvironmentCardProps) => (
  <article className="rounded-2xl border border-surface-muted bg-surface-muted/50 p-4 text-left">
    <p className="text-xs uppercase tracking-[0.3em] text-surface-foreground/60">{label}</p>
    <p className="mt-2 text-sm font-semibold text-primary-foreground break-words">{value || "Không rõ"}</p>
  </article>
);

export default HomePage;
