"use client";

import { FormEvent, useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Pencil, PlusCircle, RefreshCcw, Trash2 } from "lucide-react";

import { createComic, deleteComic, fetchComics, updateComic } from "@services/admin";
import { ComicStatus } from "../../../const/enum/comic-status";
import type { ComicResponse } from "../../../types/comic";

const DEFAULT_LIMIT = 12;

const initialFormState = {
  name: "",
  slug: "",
  author: "",
  description: "",
  embedded_from: "",
  embedded_from_url: "",
  chap_count: 0,
  rate: 0,
  status: ComicStatus.Continuing
};

type ComicFormState = typeof initialFormState;

const AdminComicsPage = () => {
  const queryClient = useQueryClient();
  const [offset, setOffset] = useState(0);
  const [formState, setFormState] = useState<ComicFormState>(initialFormState);
  const [editingComicId, setEditingComicId] = useState<string | null>(null);

  const comicsQuery = useQuery({
    queryKey: ["admin-comics", offset],
    queryFn: () => fetchComics({ offset, limit: DEFAULT_LIMIT })
  });

  const createMutation = useMutation({
    mutationFn: () =>
      createComic({
        name: formState.name,
        slug: formState.slug,
        description: formState.description,
        author: formState.author,
        embedded_from: formState.embedded_from || null,
        embedded_from_url: formState.embedded_from_url || null,
        chap_count: formState.chap_count,
        rate: formState.rate,
        status: formState.status
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin-comics"] });
      resetForm();
    }
  });

  const updateMutation = useMutation({
    mutationFn: () =>
      updateComic({
  id: editingComicId!,
        name: formState.name,
        slug: formState.slug,
        description: formState.description,
        author: formState.author,
        embedded_from: formState.embedded_from || null,
        embedded_from_url: formState.embedded_from_url || null,
        chap_count: formState.chap_count,
        rate: formState.rate,
        status: formState.status
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin-comics"] });
      resetForm();
    }
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteComic(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin-comics"] });
    }
  });

  const isSubmitting = createMutation.isPending || updateMutation.isPending;

  const statusOptions = useMemo(
    () =>
      Object.entries(ComicStatus)
        .filter(([, value]) => typeof value === "number")
        .map(([label, value]) => ({ label, value: value as number })),
    []
  );

  const resetForm = () => {
    setFormState(initialFormState);
    setEditingComicId(null);
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (editingComicId !== null) {
      updateMutation.mutate();
      return;
    }

    createMutation.mutate();
  };

  const handleEdit = (comic: ComicResponse) => {
    setEditingComicId(comic.id);
    setFormState({
      name: comic.name,
      slug: comic.slug,
      author: comic.author,
      description: comic.description,
      embedded_from: comic.embedded_from ?? "",
      embedded_from_url: comic.embedded_from_url ?? "",
      chap_count: comic.chap_count,
      rate: comic.rate,
      status: comic.status
    });
  };

  return (
    <div className="space-y-10">
      <section className="space-y-4">
        <header className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-xs uppercase tracking-[0.4em] text-primary/70">Quản lý truyện</p>
            <h2 className="text-2xl font-semibold text-primary-foreground">
              {editingComicId ? "Cập nhật truyện" : "Thêm truyện mới"}
            </h2>
          </div>
          <div className="flex gap-2">
            <button
              type="button"
              onClick={() => queryClient.invalidateQueries({ queryKey: ["admin-comics", offset] })}
              className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
            >
              <RefreshCcw className="h-4 w-4" />
              Làm mới danh sách
            </button>
            <button
              type="button"
              onClick={resetForm}
              className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary/10 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/20"
            >
              <PlusCircle className="h-4 w-4" />
              Tạo mới
            </button>
          </div>
        </header>
        <form onSubmit={handleSubmit} className="grid gap-4 rounded-2xl border border-surface-muted bg-surface/70 p-6">
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Tên truyện</span>
              <input
                required
                value={formState.name}
                onChange={(event) => setFormState((prev) => ({ ...prev, name: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              />
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Slug</span>
              <input
                required
                value={formState.slug}
                onChange={(event) => setFormState((prev) => ({ ...prev, slug: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              />
            </label>
          </div>
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Tác giả</span>
              <input
                required
                value={formState.author}
                onChange={(event) => setFormState((prev) => ({ ...prev, author: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              />
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Trạng thái</span>
              <select
                value={formState.status}
                onChange={(event) =>
                  setFormState((prev) => ({ ...prev, status: Number(event.target.value) as ComicStatus }))
                }
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              >
                {statusOptions.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </label>
          </div>
          <label className="space-y-2 text-sm">
            <span className="font-medium text-primary-foreground">Mô tả</span>
            <textarea
              required
              value={formState.description}
              onChange={(event) => setFormState((prev) => ({ ...prev, description: event.target.value }))}
              rows={4}
              className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
            />
          </label>
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Nguồn nhúng</span>
              <input
                value={formState.embedded_from}
                onChange={(event) => setFormState((prev) => ({ ...prev, embedded_from: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
                placeholder="Tên nguồn nếu có"
              />
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">URL nhúng</span>
              <input
                value={formState.embedded_from_url}
                onChange={(event) =>
                  setFormState((prev) => ({ ...prev, embedded_from_url: event.target.value }))
                }
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
                placeholder="https://..."
              />
            </label>
          </div>
          <div className="grid gap-4 md:grid-cols-3">
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Số chương</span>
              <input
                type="number"
                min={0}
                value={formState.chap_count}
                onChange={(event) => setFormState((prev) => ({ ...prev, chap_count: Number(event.target.value) }))}
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              />
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Đánh giá</span>
              <input
                type="number"
                min={0}
                max={5}
                step={0.1}
                value={formState.rate}
                onChange={(event) => setFormState((prev) => ({ ...prev, rate: Number(event.target.value) }))}
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              />
            </label>
          </div>
          <div className="flex justify-end gap-3">
            {editingComicId && (
              <button
                type="button"
                onClick={resetForm}
                className="rounded-full border border-surface-muted px-5 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
              >
                Hủy chỉnh sửa
              </button>
            )}
            <button
              type="submit"
              disabled={isSubmitting}
              className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
            >
              {isSubmitting && (
                <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
              )}
              {editingComicId ? "Cập nhật" : "Tạo mới"}
            </button>
          </div>
        </form>
      </section>

      <section className="space-y-4">
        <header className="flex items-center justify-between">
          <h3 className="text-lg font-semibold text-primary-foreground">Danh sách truyện</h3>
          <div className="flex items-center gap-3 text-xs text-surface-foreground/60">
            <button
              type="button"
              onClick={() => setOffset((prev) => Math.max(prev - DEFAULT_LIMIT, 0))}
              className="rounded-full border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground"
              disabled={offset === 0}
            >
              Trang trước
            </button>
            <span>
              {offset + 1} - {offset + DEFAULT_LIMIT}
            </span>
            <button
              type="button"
              onClick={() => setOffset((prev) => prev + DEFAULT_LIMIT)}
              className="rounded-full border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground"
            >
              Trang tiếp
            </button>
          </div>
        </header>
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {comicsQuery.isLoading && <p className="text-sm text-surface-foreground/60">Đang tải dữ liệu...</p>}
          {comicsQuery.isError && (
            <p className="text-sm text-red-300">Không thể tải danh sách truyện. Vui lòng thử lại.</p>
          )}
          {comicsQuery.data?.map((comic) => (
            <article key={comic.id} className="flex flex-col gap-3 rounded-2xl border border-surface-muted bg-surface/70 p-5">
              <div className="flex items-start justify-between gap-3">
                <div>
                  <h4 className="text-lg font-semibold text-primary-foreground">{comic.name}</h4>
                  <p className="text-xs uppercase tracking-wide text-surface-foreground/60">{comic.slug}</p>
                </div>
                <div className="flex gap-2">
                  <button
                    type="button"
                    onClick={() => handleEdit(comic)}
                    className="inline-flex items-center gap-1 rounded-full border border-primary/40 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/10"
                  >
                    <Pencil className="h-3.5 w-3.5" />
                    Sửa
                  </button>
                  <button
                    type="button"
                    onClick={() => deleteMutation.mutate(comic.id)}
                    className="inline-flex items-center gap-1 rounded-full border border-red-500/50 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-red-200 transition hover:bg-red-500/10"
                  >
                    {deleteMutation.isPending ? (
                      <RefreshCcw className="h-3.5 w-3.5 animate-spin" />
                    ) : (
                      <Trash2 className="h-3.5 w-3.5" />
                    )}
                    Xóa
                  </button>
                </div>
              </div>
              <p className="text-sm text-surface-foreground/70">{comic.description}</p>
              <div className="flex flex-wrap items-center gap-3 text-xs text-surface-foreground/60">
                <span>
                  Tác giả: <span className="text-primary-foreground">{comic.author}</span>
                </span>
                <span>Chương: {comic.chap_count}</span>
                <span>Đánh giá: {comic.rate.toFixed(1)}</span>
                <span>Trạng thái: {ComicStatus[comic.status]}</span>
              </div>
            </article>
          ))}
        </div>
      </section>
    </div>
  );
};

export default AdminComicsPage;
