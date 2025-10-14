"use client";

import { FormEvent, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { BookOpenCheck, Pencil, PlusCircle, RefreshCcw, Trash2 } from "lucide-react";

import { createComicChapter, deleteComicChapter, fetchChaptersByComic, updateComicChapter } from "@services/admin";

const AdminChaptersPage = () => {
  const queryClient = useQueryClient();
  const [selectedComicId, setSelectedComicId] = useState<string | null>(null);
  const [editingChapterId, setEditingChapterId] = useState<string | null>(null);
  const [formState, setFormState] = useState<{
    comic_id: string | null;
    chapter: number;
    content: string;
  }>({
    comic_id: null,
    chapter: 1,
    content: ""
  });

  const chaptersQuery = useQuery({
    queryKey: ["admin-chapters", selectedComicId],
    queryFn: () => fetchChaptersByComic(selectedComicId!),
    enabled: selectedComicId !== null
  });

  const resetForm = () => {
    setEditingChapterId(null);
    setFormState({ comic_id: selectedComicId, chapter: 1, content: "" });
  };

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ["admin-chapters", selectedComicId] });
  };

  const createMutation = useMutation({
    mutationFn: () =>
      createComicChapter({
        comic_id: formState.comic_id!,
        chapter: formState.chapter,
        content: formState.content
      }),
    onSuccess: () => {
      invalidate();
      resetForm();
    }
  });

  const updateMutation = useMutation({
    mutationFn: () =>
      updateComicChapter({
        id: editingChapterId!,
        comic_id: formState.comic_id!,
        chapter: formState.chapter,
        content: formState.content
      }),
    onSuccess: () => {
      invalidate();
      resetForm();
    }
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteComicChapter(id),
    onSuccess: () => {
      invalidate();
    }
  });

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!formState.comic_id) {
      return;
    }
    if (editingChapterId) {
      updateMutation.mutate();
    } else {
      createMutation.mutate();
    }
  };

  const handleSelectComic = (comicIdValue: string | null) => {
    setSelectedComicId(comicIdValue);
    setEditingChapterId(null);
    setFormState({ comic_id: comicIdValue, chapter: 1, content: "" });
  };

  return (
    <div className="space-y-10">
      <section className="grid gap-6 rounded-2xl border border-surface-muted bg-surface/70 p-6 md:grid-cols-[0.45fr_1fr]">
        <div className="space-y-4">
          <header>
            <p className="text-xs uppercase tracking-[0.4em] text-primary/70">Chọn truyện</p>
            <h2 className="text-lg font-semibold text-primary-foreground">Nhập ID truyện để quản lý chương</h2>
          </header>
          <input
            type="text"
            inputMode="numeric"
            pattern="[0-9]*"
            placeholder="VD: 101"
            value={selectedComicId ?? ""}
            onChange={(event) => handleSelectComic(event.target.value.trim() ? event.target.value.trim() : null)}
            className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
          />
          <button
            type="button"
            onClick={() => {
              if (selectedComicId) {
                queryClient.invalidateQueries({ queryKey: ["admin-chapters", selectedComicId] });
              }
            }}
            className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
          >
            <RefreshCcw className="h-4 w-4" />
            Làm mới danh sách chương
          </button>
        </div>
        <form onSubmit={handleSubmit} className="space-y-4">
          <header className="flex items-center justify-between">
            <div>
              <p className="text-xs uppercase tracking-[0.35em] text-primary/70">Biên tập chương</p>
              <h3 className="text-xl font-semibold text-primary-foreground">
                {editingChapterId ? `Sửa chương #${formState.chapter}` : "Tạo chương mới"}
              </h3>
            </div>
            <button
              type="button"
              onClick={resetForm}
              className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary/10 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/20"
            >
              <PlusCircle className="h-4 w-4" />
              Khởi tạo
            </button>
          </header>
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">ID truyện</span>
              <input
                type="text"
                inputMode="numeric"
                pattern="[0-9]*"
                required
                value={formState.comic_id ?? ""}
                onChange={(event) =>
                  setFormState((prev) => ({ ...prev, comic_id: event.target.value.trim() ? event.target.value.trim() : null }))
                }
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              />
            </label>
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">Số chương</span>
              <input
                type="number"
                min={1}
                required
                value={formState.chapter}
                onChange={(event) => setFormState((prev) => ({ ...prev, chapter: Number(event.target.value) }))}
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              />
            </label>
          </div>
          <label className="space-y-2 text-sm">
            <span className="font-medium text-primary-foreground">Nội dung chương</span>
            <textarea
              required
              rows={10}
              value={formState.content}
              onChange={(event) => setFormState((prev) => ({ ...prev, content: event.target.value }))}
              className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
            />
          </label>
          <div className="flex justify-end gap-3">
            {editingChapterId && (
              <button
                type="button"
                onClick={resetForm}
                className="rounded-full border border-surface-muted px-5 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
              >
                Hủy
              </button>
            )}
            <button
              type="submit"
              disabled={createMutation.isPending || updateMutation.isPending}
              className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
            >
              {(createMutation.isPending || updateMutation.isPending) && (
                <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
              )}
              {editingChapterId ? "Cập nhật chương" : "Thêm chương"}
            </button>
          </div>
        </form>
      </section>

      <section className="space-y-4">
        <header className="flex items-center justify-between">
          <h3 className="text-lg font-semibold text-primary-foreground">Danh sách chương truyện</h3>
          <span className="text-xs uppercase tracking-wide text-surface-foreground/60">
            {selectedComicId ? `Truyện ID ${selectedComicId}` : "Chưa chọn truyện"}
          </span>
        </header>
        {selectedComicId === null ? (
          <p className="text-sm text-surface-foreground/60">Nhập ID truyện để xem và quản lý chương.</p>
        ) : chaptersQuery.isLoading ? (
          <p className="text-sm text-surface-foreground/60">Đang tải danh sách chương...</p>
        ) : chaptersQuery.isError ? (
          <p className="text-sm text-red-300">Không thể tải danh sách chương. Vui lòng kiểm tra lại ID truyện.</p>
        ) : chaptersQuery.data && chaptersQuery.data.length > 0 ? (
          <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
            {chaptersQuery.data.map((chapter) => (
              <article key={chapter.id} className="flex flex-col gap-3 rounded-2xl border border-surface-muted bg-surface/70 p-5">
                <div className="flex items-start justify-between gap-3">
                  <div>
                    <h4 className="text-lg font-semibold text-primary-foreground">Chương {chapter.chapter}</h4>
                    <p className="text-xs uppercase tracking-wide text-surface-foreground/60">ID: {chapter.id}</p>
                  </div>
                  <div className="flex gap-2">
                    <button
                      type="button"
                      onClick={() => {
                        setEditingChapterId(chapter.id);
                        setFormState({ comic_id: chapter.comic_id, chapter: chapter.chapter, content: chapter.content });
                      }}
                      className="inline-flex items-center gap-1 rounded-full border border-primary/40 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/10"
                    >
                      <Pencil className="h-3.5 w-3.5" />
                      Sửa
                    </button>
                    <button
                      type="button"
                      onClick={() => deleteMutation.mutate(chapter.id)}
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
                <p className="line-clamp-4 text-sm text-surface-foreground/70">{chapter.content}</p>
                <div className="flex items-center justify-between text-xs text-surface-foreground/60">
                  <span>Tạo lúc: {new Date(chapter.created_at).toLocaleString()}</span>
                  <span>Cập nhật: {new Date(chapter.updated_at).toLocaleString()}</span>
                </div>
              </article>
            ))}
          </div>
        ) : (
          <p className="text-sm text-surface-foreground/60">Truyện chưa có chương nào.</p>
        )}
      </section>
    </div>
  );
};

export default AdminChaptersPage;
