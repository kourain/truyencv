"use client";

import { FormEvent, useEffect, useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Pencil, PlusCircle, RefreshCcw, Trash2 } from "lucide-react";

import { createComic, deleteComic, fetchComics, updateComic } from "@services/admin";
import { fetchAllComicCategories } from "@services/admin/comic-category.service";
import { fetchCategoriesOfComic } from "@services/admin/comic-have-category.service";
import { ComicStatus, ComicStatusLabel } from "../../../const/enum/comic-status";
import type { ComicResponse } from "../../../types/comic";

const DEFAULT_LIMIT = 12;

const initialFormState = {
  name: "",
  author: "",
  description: "",
  embedded_from: "",
  embedded_from_url: "",
  cover_url: "",
  main_category_id: 1001, // Default to "Tiên Hiệp" (Genre)
  main_character_id: null as number | null, // MainCharacter (ID 2001-2010)
  world_theme_id: null as number | null, // WorldTheme (ID 3001-3052)
  class_id: null as number | null, // Class (ID 4001-4038)
  view_id: null as number | null, // View (ID 5001-5002)
  chap_count: 0,
  rate: 0,
  status: ComicStatus.Continuing
};

type ComicFormState = typeof initialFormState;

const initialFilters = {
  keyword: "",
  status: "",
  author: "",
  embedded_from: ""
};

const AdminComicsPage = () => {
  const queryClient = useQueryClient();
  const [offset, setOffset] = useState(0);
  const [formState, setFormState] = useState<ComicFormState>(initialFormState);
  const [editingComicId, setEditingComicId] = useState<string | null>(null);
  const [filters, setFilters] = useState(initialFilters);

  const queryFilters = useMemo(
    () => ({
      keyword: filters.keyword.trim() || undefined,
      status: filters.status ? Number(filters.status) : undefined,
      author: filters.author.trim() || undefined,
      embedded_from: filters.embedded_from.trim() || undefined
    }),
    [filters]
  );

  useEffect(() => {
    setOffset(0);
  }, [queryFilters]);

  const comicsQuery = useQuery({
    queryKey: ["admin-comics", offset, queryFilters],
    queryFn: () => fetchComics({ offset, limit: DEFAULT_LIMIT, ...queryFilters })
  });

  // Fetch categories for dropdown
  const categoriesQuery = useQuery({
    queryKey: ["admin-categories-all"],
    queryFn: () => fetchAllComicCategories()
  });

  const genreCategories = useMemo(() => {
    if (!categoriesQuery.data) return [];
    // Filter only Genre categories (ID 1001-1012) for main_category
    return categoriesQuery.data
      .filter((cat) => Number(cat.id) >= 1001 && Number(cat.id) <= 1012)
      .sort((a, b) => Number(a.id) - Number(b.id));
  }, [categoriesQuery.data]);

  // Group categories by type for separate dropdowns
  const mainCharacterCategories = useMemo(() => {
    if (!categoriesQuery.data) return [];
    return categoriesQuery.data
      .filter((cat) => Number(cat.id) >= 2001 && Number(cat.id) <= 2010)
      .sort((a, b) => Number(a.id) - Number(b.id));
  }, [categoriesQuery.data]);

  const worldThemeCategories = useMemo(() => {
    if (!categoriesQuery.data) return [];
    return categoriesQuery.data
      .filter((cat) => Number(cat.id) >= 3001 && Number(cat.id) <= 3052)
      .sort((a, b) => Number(a.id) - Number(b.id));
  }, [categoriesQuery.data]);

  const classCategories = useMemo(() => {
    if (!categoriesQuery.data) return [];
    return categoriesQuery.data
      .filter((cat) => Number(cat.id) >= 4001 && Number(cat.id) <= 4038)
      .sort((a, b) => Number(a.id) - Number(b.id));
  }, [categoriesQuery.data]);

  const viewCategories = useMemo(() => {
    if (!categoriesQuery.data) return [];
    return categoriesQuery.data
      .filter((cat) => Number(cat.id) >= 5001 && Number(cat.id) <= 5002)
      .sort((a, b) => Number(a.id) - Number(b.id));
  }, [categoriesQuery.data]);

  const createMutation = useMutation({
    mutationFn: () => {
      // Collect all selected category IDs (excluding null values)
      const categoryIds: number[] = [];
      if (formState.main_character_id) categoryIds.push(formState.main_character_id);
      if (formState.world_theme_id) categoryIds.push(formState.world_theme_id);
      if (formState.class_id) categoryIds.push(formState.class_id);
      if (formState.view_id) categoryIds.push(formState.view_id);

      const payload = {
        name: formState.name.trim(),
        description: formState.description.trim(),
        author: formState.author.trim(),
        embedded_from: formState.embedded_from.trim() || null,
        embedded_from_url: formState.embedded_from_url.trim() || null,
        cover_url: formState.cover_url.trim() || null,
        main_category_id: formState.main_category_id,
        category_ids: categoryIds.length > 0 ? categoryIds : [],
        status: formState.status
        // Note: chap_count, rate, slug are NOT sent - backend auto-generates/calculates them
      };

      return createComic(payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin-comics"] });
      resetForm();
    }
  });

  const updateMutation = useMutation({
    mutationFn: () => {
      // Collect all selected category IDs (excluding null values)
      const categoryIds: number[] = [];
      if (formState.main_character_id) categoryIds.push(formState.main_character_id);
      if (formState.world_theme_id) categoryIds.push(formState.world_theme_id);
      if (formState.class_id) categoryIds.push(formState.class_id);
      if (formState.view_id) categoryIds.push(formState.view_id);

      return updateComic({
        id: editingComicId!,
        name: formState.name.trim(),
        description: formState.description.trim(),
        author: formState.author.trim(),
        embedded_from: formState.embedded_from.trim() || null,
        embedded_from_url: formState.embedded_from_url.trim() || null,
        cover_url: formState.cover_url.trim() || null,
        main_category_id: formState.main_category_id,
        category_ids: categoryIds.length > 0 ? categoryIds : [],
        chap_count: formState.chap_count,
        rate: formState.rate,
        status: formState.status
      })},
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
        .map(([, value]) => {
          const numericValue = value as number;

          return {
            label: ComicStatusLabel[numericValue as ComicStatus],
            value: numericValue
          };
        }),
    []
  );

  const resetForm = () => {
    setFormState({ ...initialFormState });
    setEditingComicId(null);
  };

  const resetFilters = () => {
    setFilters({ ...initialFilters });
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (editingComicId !== null) {
      updateMutation.mutate();
      return;
    }

    createMutation.mutate();
  };

  // Fetch categories for editing comic
  const comicCategoriesQuery = useQuery({
    queryKey: ["admin-comic-categories", editingComicId],
    queryFn: () => fetchCategoriesOfComic(editingComicId!),
    enabled: editingComicId !== null
  });

  const handleEdit = async (comic: ComicResponse) => {
    setEditingComicId(comic.id);
    // Find category ID from category name
    const categoryIdStr = genreCategories.find((cat) => cat.name === comic.main_category)?.id;
    const categoryId = categoryIdStr ? Number(categoryIdStr) : null;
    
    // Fetch existing categories for this comic
    try {
      const existingCategories = await fetchCategoriesOfComic(comic.id);
      
      // Separate categories by type (each type can only have 1 category)
      let mainCharacterId: number | null = null;
      let worldThemeId: number | null = null;
      let classId: number | null = null;
      let viewId: number | null = null;
      
      existingCategories.forEach((cat) => {
        const catId = Number(cat.id);
        // Exclude main category
        if (categoryId !== null && catId === categoryId) return;
        
        // Categorize by ID range
        if (catId >= 2001 && catId <= 2010) {
          mainCharacterId = catId;
        } else if (catId >= 3001 && catId <= 3052) {
          worldThemeId = catId;
        } else if (catId >= 4001 && catId <= 4038) {
          classId = catId;
        } else if (catId >= 5001 && catId <= 5002) {
          viewId = catId;
        }
      });
      
      setFormState({
        name: comic.name,
        author: comic.author,
        description: comic.description,
        embedded_from: comic.embedded_from ?? "",
        embedded_from_url: comic.embedded_from_url ?? "",
        cover_url: comic.cover_url ?? "",
        main_category_id: categoryId ? Number(categoryId) : 1001, // Default if not found
        main_character_id: mainCharacterId,
        world_theme_id: worldThemeId,
        class_id: classId,
        view_id: viewId,
        chap_count: comic.chap_count,
        rate: comic.rate,
        status: comic.status
      });
    } catch (error) {
      // If fetch fails, just set basic info
      setFormState({
        name: comic.name,
        author: comic.author,
        description: comic.description,
        embedded_from: comic.embedded_from ?? "",
        embedded_from_url: comic.embedded_from_url ?? "",
        cover_url: comic.cover_url ?? "",
        main_category_id: categoryId ? Number(categoryId) : 1001,
        main_character_id: null,
        world_theme_id: null,
        class_id: null,
        view_id: null,
        chap_count: comic.chap_count,
        rate: comic.rate,
        status: comic.status
      });
    }
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
              <span className="font-medium text-primary-foreground">Thể loại chính</span>
              <select
                value={formState.main_category_id}
                onChange={(event) =>
                  setFormState((prev) => ({ ...prev, main_category_id: Number(event.target.value) }))
                }
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
                disabled={categoriesQuery.isLoading}
              >
                {categoriesQuery.isLoading ? (
                  <option>Đang tải...</option>
                ) : (
                  genreCategories.map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.name}
                    </option>
                  ))
                )}
              </select>
            </label>
          </div>
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Nhân vật chính</span>
              <select
                value={formState.main_character_id ?? ""}
                onChange={(event) =>
                  setFormState((prev) => ({
                    ...prev,
                    main_character_id: event.target.value ? Number(event.target.value) : null
                  }))
                }
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
                disabled={categoriesQuery.isLoading}
              >
                <option value="">-- Chọn nhân vật chính --</option>
                {categoriesQuery.isLoading ? (
                  <option>Đang tải...</option>
                ) : (
                  mainCharacterCategories.map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.name}
                    </option>
                  ))
                )}
              </select>
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Bối cảnh thế giới</span>
              <select
                value={formState.world_theme_id ?? ""}
                onChange={(event) =>
                  setFormState((prev) => ({
                    ...prev,
                    world_theme_id: event.target.value ? Number(event.target.value) : null
                  }))
                }
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
                disabled={categoriesQuery.isLoading}
              >
                <option value="">-- Chọn bối cảnh thế giới --</option>
                {categoriesQuery.isLoading ? (
                  <option>Đang tải...</option>
                ) : (
                  worldThemeCategories.map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.name}
                    </option>
                  ))
                )}
              </select>
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Trường phái</span>
              <select
                value={formState.class_id ?? ""}
                onChange={(event) =>
                  setFormState((prev) => ({
                    ...prev,
                    class_id: event.target.value ? Number(event.target.value) : null
                  }))
                }
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
                disabled={categoriesQuery.isLoading}
              >
                <option value="">-- Chọn trường phái --</option>
                {categoriesQuery.isLoading ? (
                  <option>Đang tải...</option>
                ) : (
                  classCategories.map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.name}
                    </option>
                  ))
                )}
              </select>
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Góc nhìn</span>
              <select
                value={formState.view_id ?? ""}
                onChange={(event) =>
                  setFormState((prev) => ({
                    ...prev,
                    view_id: event.target.value ? Number(event.target.value) : null
                  }))
                }
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
                disabled={categoriesQuery.isLoading}
              >
                <option value="">-- Chọn góc nhìn --</option>
                {categoriesQuery.isLoading ? (
                  <option>Đang tải...</option>
                ) : (
                  viewCategories.map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.name}
                    </option>
                  ))
                )}
              </select>
            </label>
          </div>
          <div className="grid gap-4 md:grid-cols-2">
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
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Ảnh bìa (cover_url)</span>
              <input
                value={formState.cover_url}
                onChange={(event) => setFormState((prev) => ({ ...prev, cover_url: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
                placeholder="https://cdn.example.com/cover.jpg"
              />
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
          {formState.cover_url && (
            <div className="overflow-hidden rounded-xl border border-surface-muted/70">
              <img
                src={formState.cover_url}
                alt={`Ảnh bìa truyện ${formState.name || ""}`}
                className="h-48 w-full object-cover"
              />
            </div>
          )}
          <div className="grid gap-4 md:grid-cols-3">
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Số chương</span>
              <input
                type="number"
                min={0}
                value={formState.chap_count}
                onChange={(event) => setFormState((prev) => ({ ...prev, chap_count: Number(event.target.value) }))}
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
                disabled={!editingComicId}
              />
              {!editingComicId && (
                <p className="text-xs text-surface-foreground/60">Tự động cập nhật sau khi nhập chương.</p>
              )}
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
                disabled={!editingComicId}
              />
              {!editingComicId && (
                <p className="text-xs text-surface-foreground/60">Hệ thống tính toán dựa trên đánh giá độc giả.</p>
              )}
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
              className="rounded-md border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground disabled:opacity-60"
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
              className="rounded-md border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground"
            >
              Trang tiếp
            </button>
          </div>
        </header>
        <div className="space-y-3 rounded-2xl border border-surface-muted/70 bg-surface/60 p-4">
          <div className="grid gap-3 md:grid-cols-2 lg:grid-cols-4">
            <label className="space-y-1 text-xs">
              <span className="font-semibold uppercase tracking-wide text-surface-foreground/70">Từ khóa</span>
              <input
                value={filters.keyword}
                onChange={(event) => setFilters((prev) => ({ ...prev, keyword: event.target.value }))}
                placeholder="Tên truyện hoặc slug"
                className="w-full rounded-xl border border-surface-muted bg-surface px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary/50"
              />
            </label>
            <label className="space-y-1 text-xs">
              <span className="font-semibold uppercase tracking-wide text-surface-foreground/70">Tác giả</span>
              <input
                value={filters.author}
                onChange={(event) => setFilters((prev) => ({ ...prev, author: event.target.value }))}
                placeholder="Tên tác giả"
                className="w-full rounded-xl border border-surface-muted bg-surface px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary/50"
              />
            </label>
            <label className="space-y-1 text-xs">
              <span className="font-semibold uppercase tracking-wide text-surface-foreground/70">Nguồn nhúng</span>
              <input
                value={filters.embedded_from}
                onChange={(event) => setFilters((prev) => ({ ...prev, embedded_from: event.target.value }))}
                placeholder="Tên nguồn"
                className="w-full rounded-xl border border-surface-muted bg-surface px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary/50"
              />
            </label>
            <label className="space-y-1 text-xs">
              <span className="font-semibold uppercase tracking-wide text-surface-foreground/70">Trạng thái</span>
              <select
                value={filters.status}
                onChange={(event) => setFilters((prev) => ({ ...prev, status: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted bg-surface px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary/50"
              >
                <option value="">Tất cả</option>
                {statusOptions.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </label>
          </div>
          <div className="flex justify-end">
            <button
              type="button"
              onClick={resetFilters}
              className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
            >
              Xóa bộ lọc
            </button>
          </div>
        </div>
        <div className="overflow-hidden rounded-lg border border-surface-muted/60 bg-surface/80 shadow">
          <table className="min-w-full text-sm">
            <thead className="bg-surface-muted/40 text-xs uppercase tracking-wide text-surface-foreground/60">
              <tr>
                <th scope="col" className="px-4 py-3 text-left font-semibold">#</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">ID</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Truyện</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Tác giả</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Số chương</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Đánh giá</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Trạng thái</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Hành động</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-surface-muted/40 text-surface-foreground/80">
              {comicsQuery.isLoading && (
                <tr>
                  <td colSpan={8} className="px-4 py-6 text-center text-xs text-surface-foreground/60">
                    Đang tải danh sách truyện...
                  </td>
                </tr>
              )}
              {comicsQuery.isError && (
                <tr>
                  <td colSpan={8} className="px-4 py-6 text-center text-xs text-red-300">
                    Không thể tải danh sách truyện. Vui lòng thử lại.
                  </td>
                </tr>
              )}
              {!comicsQuery.isLoading && !comicsQuery.isError && comicsQuery.data && comicsQuery.data.length === 0 && (
                <tr>
                  <td colSpan={8} className="px-4 py-6 text-center text-xs text-surface-foreground/60">
                    Chưa có truyện nào.
                  </td>
                </tr>
              )}
              {comicsQuery.data?.map((comic, index) => {
                const isEditing = editingComicId === comic.id;
                const isDeleting = deleteMutation.isPending && deleteMutation.variables === comic.id;
                const statusLabel = ComicStatusLabel[comic.status];

                return (
                  <tr
                    key={comic.id}
                    className={`transition ${
                      isEditing ? "bg-primary/15 text-primary-foreground" : "hover:bg-surface-muted/40"
                    }`}
                  >
                    <td className="px-4 py-3 text-xs text-surface-foreground/60">{offset + index + 1}</td>
                    <td className="px-4 py-3 text-xs text-surface-foreground/60 font-mono">{comic.id}</td>
                    <td className="px-4 py-3">
                      <div className="flex gap-3">
                        <div className="hidden h-16 w-12 overflow-hidden rounded-md border border-surface-muted/70 bg-surface-muted/40 sm:block">
                          {comic.cover_url ? (
                            <img src={comic.cover_url} alt={`Ảnh bìa ${comic.name}`} className="h-full w-full object-cover" />
                          ) : (
                            <div className="flex h-full w-full items-center justify-center text-[10px] text-surface-foreground/40">
                              No Cover
                            </div>
                          )}
                        </div>
                        <div className="flex flex-col gap-1">
                          <span className="font-semibold">{comic.name}</span>
                          <span className="text-xs text-surface-foreground/60">{comic.slug}</span>
                          <span className="text-xs text-surface-foreground/60 line-clamp-2" title={comic.description}>
                            {comic.description}
                          </span>
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-xs text-surface-foreground/70">{comic.author}</td>
                    <td className="px-4 py-3 text-xs text-surface-foreground/70">{comic.chap_count}</td>
                    <td className="px-4 py-3 text-xs text-surface-foreground/70">{comic.rate.toFixed(1)}</td>
                    <td className="px-4 py-3 text-xs text-surface-foreground/70">{statusLabel}</td>
                    <td className="px-4 py-3">
                      <div className="flex flex-wrap items-center gap-2">
                        <button
                          type="button"
                          onClick={() => handleEdit(comic)}
                          className="inline-flex items-center gap-2 rounded-md border border-primary/40 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/10"
                        >
                          <Pencil className="h-3.5 w-3.5" />
                          Sửa
                        </button>
                        <button
                          type="button"
                          onClick={() => deleteMutation.mutate(comic.id)}
                          className="inline-flex items-center gap-2 rounded-md border border-red-500/50 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-red-200 transition hover:bg-red-500/10 disabled:opacity-60"
                          disabled={isDeleting}
                        >
                          {isDeleting ? (
                            <RefreshCcw className="h-3.5 w-3.5 animate-spin" />
                          ) : (
                            <Trash2 className="h-3.5 w-3.5" />
                          )}
                          Xóa
                        </button>
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
};

export default AdminComicsPage;
