import type { MetricCardConfig } from "@components/admin/types";

type MetricCardsSectionProps = {
  metricCards: MetricCardConfig[];
  showLoading: boolean;
};

const MetricCardsSection = ({ metricCards, showLoading }: MetricCardsSectionProps) => (
  <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
    {metricCards.map(({ label, value, description, icon: Icon }) => (
      <article key={label} className="rounded-2xl border border-surface-muted bg-surface/60 p-5 shadow-glow">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-xs uppercase tracking-wide text-surface-foreground/60">{label}</p>
            <p className="mt-3 text-3xl font-semibold text-primary-foreground">{showLoading ? "--" : value}</p>
          </div>
          <span className="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10 text-primary">
            <Icon className="h-6 w-6" />
          </span>
        </div>
        <p className="mt-4 text-xs text-surface-foreground/70">{showLoading ? "Đang tải dữ liệu..." : description}</p>
      </article>
    ))}
  </section>
);

export default MetricCardsSection;
