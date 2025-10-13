import { useMemo } from "react";
import { ArrowRight } from "lucide-react";

import { formatNumber, formatRelativeTime } from "@helpers/format";

type CategorySectionProps = {
  categories: ComicCategoryResponse[];
  categoryShowcase: ComicCategoryResponse[];
  showLoading: boolean;
};

const CategorySection = ({ categories, categoryShowcase, showLoading }: CategorySectionProps) => {
  const placeholderItems = useMemo(() => Array.from({ length: 5 }), []);

  return (
    <article className="rounded-2xl border border-surface-muted bg-surface/60 p-6">
      <header className="flex items-center justify-between">
        <div>
          <p className="text-xs uppercase tracking-[0.3em] text-primary/70">Danh mục truyện</p>
          <h3 className="text-xl font-semibold text-primary-foreground">Phân bổ theo thể loại</h3>
        </div>
        <div className="text-xs text-surface-foreground/60">Tổng cộng {formatNumber(categories.length)} thể loại</div>
      </header>
      <div className="mt-6 space-y-3">
        {showLoading && (
          <div className="space-y-3">
            {placeholderItems.map((_, index) => (
              <div key={index} className="h-14 animate-pulse rounded-xl bg-surface-muted/40" />
            ))}
          </div>
        )}

        {!showLoading && categoryShowcase.length === 0 && (
          <div className="rounded-xl border border-dashed border-surface-muted/60 bg-surface px-4 py-6 text-center text-sm text-surface-foreground/60">
            Chưa có danh mục nào được tạo.
          </div>
        )}

        {!showLoading &&
          categoryShowcase.map((category) => (
            <div
              key={category.id}
              className="flex items-center justify-between rounded-xl border border-surface-muted/70 bg-surface px-4 py-3"
            >
              <div>
                <p className="text-sm font-semibold text-primary-foreground">{category.name}</p>
                <p className="text-xs text-surface-foreground/60">Tạo {formatRelativeTime(category.created_at)}</p>
              </div>
              <ArrowRight className="h-4 w-4 text-surface-foreground/40" />
            </div>
          ))}
      </div>
    </article>
  );
};

export default CategorySection;
