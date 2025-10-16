"use client";

import { useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { PlusCircle, RefreshCcw, Tag, Trash2 } from "lucide-react";

import {
  addComicToCategory,
  createComicCategory,
  deleteComicCategory,
  fetchComicCategories,
  fetchComicsOfCategory,
  removeComicFromCategory,
  updateComicCategory
} from "@services/admin";

const DEFAULT_LIMIT = 20;

type CategoryFormState = {
  name: string;
};

type MappingFormState = {
  categoryId: string | null;
  comicId: string | null;
};

const initialCategoryForm: CategoryFormState = {
  name: ""
};

const initialMappingForm: MappingFormState = {
  categoryId: null,
  comicId: null
};

const AdminCategoriesPage = () => {
  const queryClient = useQueryClient();
  const [offset, setOffset] = useState(0);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [categoryForm, setCategoryForm] = useState<CategoryFormState>(initialCategoryForm);
  const [mappingForm, setMappingForm] = useState<MappingFormState>(initialMappingForm);

  const categoriesQuery = useQuery({
    queryKey: ["admin-categories", offset],
    queryFn: () => fetchComicCategories({ offset, limit: DEFAULT_LIMIT })
  });

  const selectedCategoryQuery = useQuery({
    queryKey: ["admin-category-comics", mappingForm.categoryId],
    queryFn: () => fetchComicsOfCategory(mappingForm.categoryId!),
    enabled: mappingForm.categoryId !== null
  });

  const invalidateCategories = () => {
    queryClient.invalidateQueries({ queryKey: ["admin-categories"] });
    queryClient.invalidateQueries({ queryKey: ["admin-category-comics"] });
  };

  const createMutation = useMutation({
    mutationFn: () => createComicCategory({ name: categoryForm.name } as CreateComicCategoryRequest),
    onSuccess: () => {
      invalidateCategories();
      resetCategoryForm();
    }
  });

  const updateMutation = useMutation({
    mutationFn: () => updateComicCategory({ id: editingId!, name: categoryForm.name } as UpdateComicCategoryRequest),
    onSuccess: () => {
      invalidateCategories();
      resetCategoryForm();
    }
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteComicCategory(id),
    onSuccess: () => {
      invalidateCategories();
    }
  });

  const addMappingMutation = useMutation({
    mutationFn: () =>
      addComicToCategory({
        comic_id: mappingForm.comicId!,
        comic_category_id: mappingForm.categoryId!
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin-category-comics", mappingForm.categoryId] });
      setMappingForm((prev) => ({ ...prev, comicId: null }));
    }
  });

  const removeMappingMutation = useMutation({
    mutationFn: ({ comicId, categoryId }: { comicId: string; categoryId: string }) =>
      removeComicFromCategory(comicId, categoryId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["admin-category-comics", variables.categoryId] });
    }
  });

  const isSubmitting = createMutation.isPending || updateMutation.isPending;

  const resetCategoryForm = () => {
    setCategoryForm(initialCategoryForm);
    setEditingId(null);
  };

  const handleEdit = (category: ComicCategoryResponse) => {
    setEditingId(category.id);
    setCategoryForm({ name: category.name });
  };

  const isMappingReady = useMemo(
    () => mappingForm.categoryId !== null && mappingForm.comicId !== null,
    [mappingForm]
  );

  return (
    <div className="space-y-10">
      <section className="space-y-4">
        <header className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-xs uppercase tracking-[0.4em] text-primary/70">Quản lý thể loại</p>
            <h2 className="text-2xl font-semibold text-primary-foreground">
              {editingId ? "Cập nhật thể loại" : "Thêm thể loại mới"}
            </h2>
          </div>
          <div className="flex gap-2">
            <button
              type="button"
              onClick={() => queryClient.invalidateQueries({ queryKey: ["admin-categories", offset] })}
              className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
            >
              <RefreshCcw className="h-4 w-4" />
              Làm mới
            </button>
            <button
              type="button"
              onClick={resetCategoryForm}
              className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary/10 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/20"
            >
              <PlusCircle className="h-4 w-4" />
              Tạo mới
            </button>
          </div>
        </header>
        <form
          onSubmit={(event) => {
            event.preventDefault();
            if (!categoryForm.name.trim()) {
              return;
            }
            if (editingId) {
              updateMutation.mutate();
            } else {
              createMutation.mutate();
            }
          }}
          className="grid gap-4 rounded-2xl border border-surface-muted bg-surface/70 p-6"
        >
          <label className="space-y-2 text-sm">
            <span className="font-medium text-primary-foreground">Tên thể loại</span>
            <input
              required
              value={categoryForm.name}
              onChange={(event) => setCategoryForm({ name: event.target.value })}
              className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
            />
          </label>
          <div className="flex justify-end gap-3">
            {editingId && (
              <button
                type="button"
                onClick={resetCategoryForm}
                className="rounded-full border border-surface-muted px-5 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
              >
                Hủy
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
              {editingId ? "Cập nhật" : "Tạo mới"}
            </button>
          </div>
        </form>
      </section>

      <section className="space-y-4">
        <header className="flex items-center justify-between">
          <h3 className="text-lg font-semibold text-primary-foreground">Danh sách thể loại</h3>
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
          {categoriesQuery.isLoading && <p className="text-sm text-surface-foreground/60">Đang tải dữ liệu...</p>}
          {categoriesQuery.isError && (
            <p className="text-sm text-red-300">Không thể tải danh sách thể loại. Vui lòng thử lại.</p>
          )}
          {categoriesQuery.data?.map((category) => (
            <article key={category.id} className="flex flex-col gap-3 rounded-2xl border border-surface-muted bg-surface/70 p-5">
              <div className="flex items-start justify-between gap-3">
                <div>
                  <h4 className="text-lg font-semibold text-primary-foreground">{category.name}</h4>
                  <p className="text-xs uppercase tracking-wide text-surface-foreground/60">ID: {category.id}</p>
                </div>
                <div className="flex gap-2">
                  <button
                    type="button"
                    onClick={() => handleEdit(category)}
                    className="inline-flex items-center gap-1 rounded-full border border-primary/40 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/10"
                  >
                    <Tag className="h-3.5 w-3.5" />
                    Sửa
                  </button>
                  <button
                    type="button"
                    onClick={() => deleteMutation.mutate(category.id)}
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
            </article>
          ))}
        </div>
      </section>

      <section className="space-y-4">
        <header className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-xs uppercase tracking-[0.3em] text-primary/70">Gán truyện cho thể loại</p>
            <h3 className="text-lg font-semibold text-primary-foreground">Quản lý phân loại nội dung</h3>
          </div>
        </header>
        <div className="grid gap-4 rounded-2xl border border-surface-muted bg-surface/70 p-6 md:grid-cols-[1fr_1fr]">
          <form
            className="space-y-4"
            onSubmit={(event) => {
              event.preventDefault();
              if (!isMappingReady) {
                return;
              }
              addMappingMutation.mutate();
            }}
          >
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">ID thể loại</span>
              <input
                type="text"
                inputMode="numeric"
                pattern="[0-9]*"
                value={mappingForm.categoryId ?? ""}
                onChange={(event) =>
                  setMappingForm((prev) => ({ ...prev, categoryId: event.target.value.trim() ? event.target.value.trim() : null }))
                }
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
                placeholder="Nhập ID thể loại"
              />
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">ID truyện</span>
              <input
                type="text"
                inputMode="numeric"
                pattern="[0-9]*"
                value={mappingForm.comicId ?? ""}
                onChange={(event) =>
                  setMappingForm((prev) => ({ ...prev, comicId: event.target.value.trim() ? event.target.value.trim() : null }))
                }
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
                placeholder="Nhập ID truyện"
              />
            </label>
            <button
              type="submit"
              disabled={!isMappingReady || addMappingMutation.isPending}
              className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
            >
              {addMappingMutation.isPending && (
                <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
              )}
              Thêm vào thể loại
            </button>
          </form>
          <div className="space-y-3">
            <div className="rounded-2xl border border-surface-muted bg-surface/80 p-5">
              <h4 className="text-sm font-semibold text-primary-foreground">Danh sách truyện thuộc thể loại</h4>
              {mappingForm.categoryId === null ? (
                <p className="mt-2 text-xs text-surface-foreground/60">Nhập ID thể loại để xem danh sách.</p>
              ) : selectedCategoryQuery.isLoading ? (
                <p className="mt-2 text-xs text-surface-foreground/60">Đang tải dữ liệu...</p>
              ) : selectedCategoryQuery.isError ? (
                <p className="mt-2 text-xs text-red-300">Không thể tải danh sách truyện.</p>
              ) : selectedCategoryQuery.data && selectedCategoryQuery.data.length > 0 ? (
                <ul className="mt-3 space-y-2 text-sm text-surface-foreground/70">
                  {selectedCategoryQuery.data.map((comic) => (
                    <li
                      key={comic.id}
                      className="flex items-center justify-between rounded-xl border border-surface-muted/60 bg-surface px-4 py-2"
                    >
                      <span>{comic.name}</span>
                      <button
                        type="button"
                        onClick={() =>
                          removeMappingMutation.mutate({ comicId: comic.id, categoryId: mappingForm.categoryId! })
                        }
                        className="text-xs font-semibold uppercase tracking-wide text-red-200 transition hover:text-red-100"
                      >
                        Gỡ
                      </button>
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="mt-2 text-xs text-surface-foreground/60">Chưa có truyện nào thuộc thể loại này.</p>
              )}
            </div>
          </div>
        </div>
      </section>
    </div>
  );
};

export default AdminCategoriesPage;
