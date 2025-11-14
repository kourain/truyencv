"use client";
import {
  useConverterComicsQuery,
  useCreateConverterComicMutation,
  useUpdateConverterComicMutation,
  useDeleteConverterComicMutation,
  useConverterComicCategoriesQuery,
} from "@services/converter";

import { FormEvent, useMemo, useState } from "react";
import { Filter, Pencil, PlusCircle, RefreshCcw, Trash2 } from "lucide-react";

import type { ComicResponse } from "../../../types/comic";
import { CategoryType } from "@const/enum/category-type";
import { ComicStatus, ComicStatusLabel } from "@const/enum/comic-status";
import { formatRelativeTime } from "@helpers/format";

const DEFAULT_LIMIT = 20;
const DEFAULT_MAIN_CATEGORY_ID = 1001;

const initialFormState = {
  name: "",
  author: "",
  description: "",
  embedded_from: "",
  embedded_from_url: "",
  cover_url: "",
  main_category_id: DEFAULT_MAIN_CATEGORY_ID,
  status: ComicStatus.Continuing,
  chap_count: 0,
  rate: 0,
};

const initialFilters = {
  keyword: "",
  status: "",
  author: "",
  embedded_from: "",
};

const ConverterComicsPage = () => {
  const [limit, setLimit] = useState(DEFAULT_LIMIT);
  const [offset, setOffset] = useState(0);
  const [formState, setFormState] = useState(() => ({ ...initialFormState }));
  const [editingComicId, setEditingComicId] = useState<string | null>(null);
  const [filters, setFilters] = useState(() => ({ ...initialFilters }));

  const queryParams = useMemo(() => ({ limit, offset }), [limit, offset]);
  const { data: comics = [], isLoading, isFetching, refetch } = useConverterComicsQuery(queryParams);
  const createMutation = useCreateConverterComicMutation();
  const updateMutation = useUpdateConverterComicMutation();
  const deleteMutation = useDeleteConverterComicMutation();
  const { data: categories = [], isLoading: isCategoryLoading } = useConverterComicCategoriesQuery();

  const statusOptions = useMemo(
    () =>
      (Object.values(ComicStatus).filter((value) => typeof value === "number") as ComicStatus[]).sort(
        (a, b) => a - b,
      ),
    [],
  );

  const genreCategories = useMemo(
    () =>
      categories
        .filter((category) => category.category_type === CategoryType.Genre)
        .sort((a, b) => Number(a.id) - Number(b.id)),
    [categories],
  );

  const filteredComics = useMemo(() => {
    const keyword = filters.keyword.trim().toLowerCase();
    const authorFilter = filters.author.trim().toLowerCase();
    const embeddedFilter = filters.embedded_from.trim().toLowerCase();
    const statusFilter = filters.status ? Number(filters.status) : undefined;

    return comics.filter((comic) => {
      const matchesKeyword =
        !keyword ||
        comic.name.toLowerCase().includes(keyword) ||
        comic.slug.toLowerCase().includes(keyword) ||
        comic.description.toLowerCase().includes(keyword);
      const matchesAuthor = !authorFilter || comic.author.toLowerCase().includes(authorFilter);
      const matchesEmbedded = !embeddedFilter || (comic.embedded_from ?? "").toLowerCase().includes(embeddedFilter);
      const matchesStatus = !statusFilter || comic.status === statusFilter;
      return matchesKeyword && matchesAuthor && matchesEmbedded && matchesStatus;
    });
  }, [comics, filters]);

  const resetForm = () => {
    setFormState({ ...initialFormState });
    setEditingComicId(null);
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const fallbackMainCategoryId = genreCategories.length ? Number(genreCategories[0].id) : DEFAULT_MAIN_CATEGORY_ID;
    const mainCategoryId = formState.main_category_id ?? fallbackMainCategoryId;

    const payload = {
      name: formState.name.trim(),
      author: formState.author.trim(),
      description: formState.description.trim(),
      embedded_from: formState.embedded_from.trim() || null,
      embedded_from_url: formState.embedded_from_url.trim() || null,
      cover_url: formState.cover_url.trim() || null,
      main_category_id: mainCategoryId,
      status: formState.status,
    };

    if (editingComicId) {
      updateMutation.mutate(
        {
          id: editingComicId,
          ...payload,
          chap_count: formState.chap_count,
          rate: formState.rate,
        },
        {
          onSuccess: async () => {
            await refetch();
            resetForm();
          },
        },
      );
      return;
    }

    createMutation.mutate(payload, {
      onSuccess: async () => {
        await refetch();
        resetForm();
      },
    });
  };

  const handleEdit = (comic: ComicResponse) => {
    const fallbackMainCategoryId = genreCategories.length ? Number(genreCategories[0].id) : DEFAULT_MAIN_CATEGORY_ID;
    const matchedCategory = genreCategories.find((category) => category.name === comic.main_category);
    setEditingComicId(comic.id);
    setFormState({
      name: comic.name,
      author: comic.author,
      description: comic.description,
      embedded_from: comic.embedded_from ?? "",
      embedded_from_url: comic.embedded_from_url ?? "",
      cover_url: comic.cover_url ?? "",
      main_category_id: matchedCategory ? Number(matchedCategory.id) : fallbackMainCategoryId,
      status: comic.status,
      chap_count: comic.chap_count,
      rate: comic.rate,
    });
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleDelete = async (comic: ComicResponse) => {
    if (!window.confirm(`Xác nhận xóa truyện "${comic.name}"?`)) {
      return;
    }

    try {
      await deleteMutation.mutateAsync(comic.id);
      await refetch();
      if (editingComicId === comic.id) {
        resetForm();
      }
    } catch (error) {
      console.error("Delete comic failed", error);
    }
  };

  const handleFilterChange = (field: keyof typeof initialFilters, value: string) => {
    setFilters((prev) => ({ ...prev, [field]: value }));
  };

  const handlePrevPage = () => {
    setOffset((prev) => Math.max(0, prev - limit));
  };

  const handleNextPage = () => {
    setOffset((prev) => prev + limit);
  };

  const isSubmitting = createMutation.isPending || updateMutation.isPending;
  const isBusy = isLoading || isFetching;

  return (
    <section className="space-y-8">
      <header className="rounded-2xl border border-surface-muted/60 bg-surface/80 p-5 shadow-sm">
        <div className="flex flex-col gap-2 md:flex-row md:items-center md:justify-between">
          <div>
            <p className="text-xs uppercase tracking-[0.4em] text-primary/70">Quản lý truyện</p>
            <h1 className="text-2xl font-semibold text-primary-foreground">
              {editingComicId ? "Cập nhật truyện" : "Thêm truyện mới"}
            </h1>
            <p className="text-sm text-surface-foreground/70">
              Sao chép luồng thao tác của trang admin: bộ lọc nhanh, biểu mẫu đầy đủ và bảng theo dõi trạng thái.
            </p>
          </div>
          <div className="flex flex-wrap gap-2">
            <button
              type="button"
              className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
              onClick={() => refetch()}
              disabled={isBusy}
            >
              <RefreshCcw className={`h-4 w-4 ${isFetching ? "animate-spin" : ""}`} />
              Làm mới danh sách
            </button>
            <button
              type="button"
              className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary/10 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/20"
              onClick={resetForm}
              disabled={isSubmitting}
            >
              <PlusCircle className="h-4 w-4" />
              Tạo mới
            </button>
          </div>
        </div>
      </header>

      <section className="rounded-2xl border border-surface-muted/60 bg-surface p-5 shadow-sm">
        <header className="mb-4 flex items-center gap-2 text-sm font-semibold text-primary">
          <Filter className="h-4 w-4" />
          Bộ lọc nhanh
        </header>
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
          <label className="space-y-2 text-xs uppercase tracking-wide text-surface-foreground/60">
            <span>Tìm kiếm</span>
            <input
              value={filters.keyword}
              onChange={(event) => handleFilterChange("keyword", event.target.value)}
              className="w-full rounded-xl border border-surface-muted bg-surface px-3 py-2 text-sm"
              placeholder="Tên truyện, mô tả"
            />
          </label>
          <label className="space-y-2 text-xs uppercase tracking-wide text-surface-foreground/60">
            <span>Tác giả</span>
            <input
              value={filters.author}
              onChange={(event) => handleFilterChange("author", event.target.value)}
              className="w-full rounded-xl border border-surface-muted bg-surface px-3 py-2 text-sm"
              placeholder="Tên tác giả"
            />
          </label>
          <label className="space-y-2 text-xs uppercase tracking-wide text-surface-foreground/60">
            <span>Nguồn convert</span>
            <input
              value={filters.embedded_from}
              onChange={(event) => handleFilterChange("embedded_from", event.target.value)}
              className="w-full rounded-xl border border-surface-muted bg-surface px-3 py-2 text-sm"
              placeholder="Tên website/nhóm"
            />
          </label>
          <label className="space-y-2 text-xs uppercase tracking-wide text-surface-foreground/60">
            <span>Trạng thái</span>
            <select
              value={filters.status}
              onChange={(event) => handleFilterChange("status", event.target.value)}
              className="w-full rounded-xl border border-surface-muted bg-surface px-3 py-2 text-sm"
            >
              <option value="">Tất cả</option>
              {statusOptions.map((value) => (
                <option key={value} value={value}>
                  {ComicStatusLabel[value as ComicStatus]}
                </option>
              ))}
            </select>
          </label>
        </div>
      </section>

      <section className="space-y-4 rounded-2xl border border-surface-muted bg-surface p-6 shadow-sm">
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-2 text-sm">
              <span className="font-semibold text-primary-foreground">Tên truyện</span>
              <input
                required
                value={formState.name}
                onChange={(event) => setFormState((prev) => ({ ...prev, name: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted px-4 py-3"
              />
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-semibold text-primary-foreground">Tác giả</span>
              <input
                required
                value={formState.author}
                onChange={(event) => setFormState((prev) => ({ ...prev, author: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted px-4 py-3"
              />
            </label>
          </div>

          <label className="space-y-2 text-sm">
            <span className="font-semibold text-primary-foreground">Mô tả</span>
            <textarea
              required
              value={formState.description}
              onChange={(event) => setFormState((prev) => ({ ...prev, description: event.target.value }))}
              className="min-h-[140px] w-full rounded-xl border border-surface-muted px-4 py-3"
            />
          </label>

          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-2 text-sm">
              <span className="font-semibold text-primary-foreground">Nguồn chuyển thể</span>
              <input
                value={formState.embedded_from}
                onChange={(event) => setFormState((prev) => ({ ...prev, embedded_from: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted px-4 py-3"
                placeholder="Tên website"
              />
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-semibold text-primary-foreground">URL nguồn</span>
              <input
                value={formState.embedded_from_url}
                onChange={(event) => setFormState((prev) => ({ ...prev, embedded_from_url: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted px-4 py-3"
                placeholder="https://"
              />
            </label>
          </div>

          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-2 text-sm">
              <span className="font-semibold text-primary-foreground">Ảnh bìa</span>
              <input
                value={formState.cover_url}
                onChange={(event) => setFormState((prev) => ({ ...prev, cover_url: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted px-4 py-3"
                placeholder="URL ảnh"
              />
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-semibold text-primary-foreground">Thể loại chính</span>
              <select
                value={formState.main_category_id}
                onChange={(event) =>
                  setFormState((prev) => ({
                    ...prev,
                    main_category_id: Number(event.target.value),
                  }))
                }
                className="w-full rounded-xl border border-surface-muted px-4 py-3"
                disabled={isCategoryLoading}
              >
                {isCategoryLoading ? (
                  <option value="">Đang tải...</option>
                ) : (
                  genreCategories.map((category) => (
                    <option key={category.id} value={Number(category.id)}>
                      {category.name}
                    </option>
                  ))
                )}
              </select>
            </label>
          </div>

          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-2 text-sm">
              <span className="font-semibold text-primary-foreground">Trạng thái</span>
              <select
                value={formState.status}
                onChange={(event) => setFormState((prev) => ({ ...prev, status: Number(event.target.value) as ComicStatus }))}
                className="w-full rounded-xl border border-surface-muted px-4 py-3"
              >
                {statusOptions.map((value) => (
                  <option key={value} value={value}>
                    {ComicStatusLabel[value as ComicStatus]}
                  </option>
                ))}
              </select>
            </label>
            {editingComicId ? (
              <div className="grid grid-cols-2 gap-4">
                <label className="space-y-2 text-sm">
                  <span className="font-semibold text-primary-foreground">Tổng chương</span>
                  <input
                    value={formState.chap_count}
                    readOnly
                    className="w-full rounded-xl border border-dashed border-surface-muted px-4 py-3 text-surface-foreground/60"
                  />
                </label>
                <label className="space-y-2 text-sm">
                  <span className="font-semibold text-primary-foreground">Đánh giá</span>
                  <input
                    value={formState.rate}
                    readOnly
                    className="w-full rounded-xl border border-dashed border-surface-muted px-4 py-3 text-surface-foreground/60"
                  />
                </label>
              </div>
            ) : (
              <div />
            )}
          </div>

          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={resetForm}
              className="rounded-full border border-surface-muted/70 px-5 py-2 text-sm font-semibold text-surface-foreground/70 hover:border-primary/40 hover:text-primary"
              disabled={isSubmitting}
            >
              Hủy
            </button>
            <button
              type="submit"
              className="rounded-full bg-primary px-5 py-2 text-sm font-semibold text-white shadow hover:bg-primary/90"
              disabled={isSubmitting}
            >
              {isSubmitting ? "Đang lưu..." : editingComicId ? "Cập nhật" : "Tạo truyện"}
            </button>
          </div>
        </form>
      </section>

      <div className="rounded-2xl border border-surface-muted/60 bg-surface shadow-sm">
        <div className="flex flex-col gap-3 border-b border-surface-muted/60 px-4 py-3 text-sm text-surface-foreground/70 md:flex-row md:items-center md:justify-between">
          <div className="flex items-center gap-2">
            <span>Giới hạn mỗi lần:</span>
            <select
              className="rounded-lg border border-surface-muted/60 bg-transparent px-2 py-1"
              value={limit}
              onChange={(event) => {
                setLimit(Number(event.target.value));
                setOffset(0);
              }}
            >
              {[10, 20, 50].map((value) => (
                <option key={value} value={value}>
                  {value} truyện
                </option>
              ))}
            </select>
          </div>
          <span className="text-xs text-surface-foreground/60">
            {isFetching ? "Đang tải dữ liệu..." : `Hiển thị ${filteredComics.length} / ${comics.length} truyện`}
          </span>
        </div>

        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-surface-muted/60 text-sm">
            <thead className="bg-surface-muted/30 text-xs uppercase tracking-wide text-surface-foreground/60">
              <tr>
                <th className="px-4 py-3 text-left">Truyện</th>
                <th className="px-4 py-3 text-left">Chương</th>
                <th className="px-4 py-3 text-left">Trạng thái</th>
                <th className="px-4 py-3 text-left">Cập nhật</th>
                <th className="px-4 py-3 text-left">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-surface-muted/50">
              {isLoading && (
                <tr>
                  <td colSpan={5} className="px-4 py-6 text-center text-surface-foreground/60">
                    Đang tải dữ liệu truyện...
                  </td>
                </tr>
              )}

              {!isLoading && filteredComics.length === 0 && (
                <tr>
                  <td colSpan={5} className="px-4 py-8 text-center text-surface-foreground/60">
                    Không có truyện phù hợp với bộ lọc hiện tại.
                  </td>
                </tr>
              )}

              {filteredComics.map((comic) => (
                <tr key={comic.id} className="hover:bg-primary/5">
                  <td className="px-4 py-3">
                    <div className="font-semibold text-primary-foreground">{comic.name}</div>
                    <div className="text-xs text-surface-foreground/70">{comic.slug}</div>
                  </td>
                  <td className="px-4 py-3 text-surface-foreground/70">{comic.chap_count}</td>
                  <td className="px-4 py-3">
                    <span className="inline-flex items-center rounded-full bg-primary/10 px-3 py-1 text-xs font-semibold text-primary">
                      {ComicStatusLabel[comic.status]}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-surface-foreground/70">{formatRelativeTime(comic.updated_at)}</td>
                  <td className="px-4 py-3">
                    <div className="flex flex-wrap gap-2">
                      <button
                        type="button"
                        className="inline-flex items-center gap-2 rounded-full border border-surface-muted/80 px-3 py-1.5 text-xs font-semibold text-primary-foreground transition hover:border-primary hover:text-primary"
                        onClick={() => handleEdit(comic)}
                        disabled={isBusy}
                      >
                        <Pencil className="h-4 w-4" />
                        Sửa
                      </button>
                      <button
                        type="button"
                        className="inline-flex items-center gap-2 rounded-full border border-red-200 px-3 py-1.5 text-xs font-semibold text-red-500 transition hover:bg-red-50"
                        onClick={() => handleDelete(comic)}
                        disabled={deleteMutation.isPending}
                      >
                        <Trash2 className="h-4 w-4" />
                        Xóa
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="flex items-center justify-between border-t border-surface-muted/60 px-4 py-3 text-sm">
          <span className="text-surface-foreground/60">Offset hiện tại: {offset}</span>
          <div className="flex gap-2">
            <button
              type="button"
              className="rounded-full border border-surface-muted/70 px-4 py-2 text-xs font-semibold text-surface-foreground/70 hover:border-primary/40 hover:text-primary"
              onClick={handlePrevPage}
              disabled={offset === 0 || isBusy}
            >
              Trang trước
            </button>
            <button
              type="button"
              className="rounded-full border border-primary/50 px-4 py-2 text-xs font-semibold text-primary hover:bg-primary/10"
              onClick={handleNextPage}
              disabled={isBusy || comics.length < limit}
            >
              Trang tiếp
            </button>
          </div>
        </div>
      </div>
    </section>
  );
};

export default ConverterComicsPage;
