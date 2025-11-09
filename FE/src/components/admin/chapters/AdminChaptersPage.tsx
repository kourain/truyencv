"use client";

import { FormEvent, useEffect, useMemo, useRef, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { BookOpenCheck, Pencil, PlusCircle, RefreshCcw, Search, Trash2, X } from "lucide-react";

import { createComicChapter, deleteComicChapter, fetchChaptersByComic, updateComicChapter } from "@services/admin";
import { fetchComics, fetchComicById } from "@services/admin/comic.service";
import type { ComicResponse } from "../../../types/comic";

const AdminChaptersPage = () => {
  const queryClient = useQueryClient();
  const [selectedComicId, setSelectedComicId] = useState<string | null>(null);
  const [editingChapterId, setEditingChapterId] = useState<string | null>(null);
  const [searchKeyword, setSearchKeyword] = useState("");
  const [showSearchResults, setShowSearchResults] = useState(false);
  const [selectedComic, setSelectedComic] = useState<ComicResponse | null>(null);
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

  // Search comics by name
  const comicsSearchQuery = useQuery({
    queryKey: ["admin-comics-search", searchKeyword],
    queryFn: () => fetchComics({ keyword: searchKeyword, limit: 10 }),
    enabled: searchKeyword.trim().length >= 2, // Only search when at least 2 characters
    staleTime: 30000 // Cache for 30 seconds
  });

  const searchResults = useMemo(() => {
    if (!comicsSearchQuery.data || !searchKeyword.trim()) return [];
    return comicsSearchQuery.data;
  }, [comicsSearchQuery.data, searchKeyword]);

  // Fetch comic info when ID is entered directly (not from search)
  const comicInfoQuery = useQuery({
    queryKey: ["admin-comic-info", selectedComicId],
    queryFn: () => fetchComicById(selectedComicId!),
    enabled:
      selectedComicId !== null &&
      (selectedComic === null || selectedComic.id !== selectedComicId), // Only fetch if not already selected from search
    staleTime: 60000 // Cache for 1 minute
  });

  // Update selectedComic when comic info is fetched from ID
  useEffect(() => {
    if (comicInfoQuery.data && (selectedComic === null || selectedComic.id !== selectedComicId)) {
      setSelectedComic(comicInfoQuery.data);
    }
  }, [comicInfoQuery.data, selectedComic, selectedComicId]);

  // Auto-set chapter number to max + 1 when comic is selected and chapters are loaded
  useEffect(() => {
    if (
      selectedComicId &&
      chaptersQuery.data &&
      editingChapterId === null // Only auto-set when creating new chapter, not editing
    ) {
      if (chaptersQuery.data.length > 0) {
        const maxChapter = Math.max(...chaptersQuery.data.map((ch) => ch.chapter));
        const nextChapter = maxChapter + 1;
        
        // Always update to next chapter when chapters are loaded (for new comic selection or after creating)
        setFormState((prev) => ({
          ...prev,
          chapter: nextChapter
        }));
      } else {
        // If no chapters exist, set to 1
        setFormState((prev) => ({
          ...prev,
          chapter: 1
        }));
      }
    }
  }, [selectedComicId, chaptersQuery.data, editingChapterId]);

  // Close dropdown when clicking outside
  const searchRef = useRef<HTMLDivElement>(null);
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (searchRef.current && !searchRef.current.contains(event.target as Node)) {
        setShowSearchResults(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  const resetForm = () => {
    setEditingChapterId(null);
    // Don't set chapter here - let useEffect auto-set it based on max chapter
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
    setSearchKeyword("");
    setShowSearchResults(false);
    // Don't clear selectedComic when entering ID directly - let it be fetched
    if (!comicIdValue) {
      setSelectedComic(null);
    }
  };

  const handleSelectComicFromSearch = (comic: ComicResponse) => {
    setSelectedComic(comic);
    handleSelectComic(comic.id);
  };

  const handleSearchInputChange = (value: string) => {
    setSearchKeyword(value);
    setShowSearchResults(value.trim().length >= 2);
    if (value.trim().length < 2) {
      setShowSearchResults(false);
    }
  };

  const handleClearSearch = () => {
    setSearchKeyword("");
    setShowSearchResults(false);
    setSelectedComic(null);
  };

  return (
    <div className="space-y-10">
      <section className="grid gap-6 rounded-2xl border border-surface-muted bg-surface/70 p-6 md:grid-cols-[0.45fr_1fr]">
        <div className="space-y-4">
          <header>
            <p className="text-xs uppercase tracking-[0.4em] text-primary/70">Chọn truyện</p>
            <h2 className="text-lg font-semibold text-primary-foreground">Tìm kiếm hoặc nhập ID truyện</h2>
          </header>
          
          {/* Search by name */}
          <div className="relative" ref={searchRef}>
            <div className="relative">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-surface-foreground/40" />
              <input
                type="text"
                placeholder="Tìm kiếm theo tên truyện..."
                value={searchKeyword}
                onChange={(event) => handleSearchInputChange(event.target.value)}
                onFocus={() => {
                  if (searchKeyword.trim().length >= 2) {
                    setShowSearchResults(true);
                  }
                }}
                className="w-full rounded-xl border border-surface-muted bg-surface pl-10 pr-10 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              />
              {searchKeyword && (
                <button
                  type="button"
                  onClick={handleClearSearch}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-surface-foreground/40 hover:text-surface-foreground/70"
                >
                  <X className="h-4 w-4" />
                </button>
              )}
            </div>
            
            {/* Search results dropdown */}
            {showSearchResults && (
              <div className="absolute z-10 mt-2 w-full rounded-xl border border-surface-muted bg-surface shadow-lg max-h-64 overflow-y-auto">
                {comicsSearchQuery.isLoading ? (
                  <div className="px-4 py-3 text-xs text-surface-foreground/60">Đang tìm kiếm...</div>
                ) : searchResults.length === 0 ? (
                  <div className="px-4 py-3 text-xs text-surface-foreground/60">Không tìm thấy truyện nào.</div>
                ) : (
                  searchResults.map((comic) => (
                    <button
                      key={comic.id}
                      type="button"
                      onClick={() => handleSelectComicFromSearch(comic)}
                      className="w-full px-4 py-3 text-left text-sm hover:bg-surface-muted/40 transition"
                    >
                      <div className="flex items-center gap-3">
                        {comic.cover_url && (
                          <img
                            src={comic.cover_url}
                            alt={comic.name}
                            className="h-10 w-8 rounded object-cover"
                          />
                        )}
                        <div className="flex-1 min-w-0">
                          <div className="font-semibold truncate">{comic.name}</div>
                          <div className="text-xs text-surface-foreground/60">
                            ID: {comic.id} • {comic.author}
                          </div>
                        </div>
                      </div>
                    </button>
                  ))
                )}
              </div>
            )}
          </div>

          {/* Or input ID directly */}
          <div className="relative">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-surface-muted"></div>
            </div>
            <div className="relative flex justify-center text-xs uppercase">
              <span className="bg-surface px-2 text-surface-foreground/60">Hoặc</span>
            </div>
          </div>
          
          <input
            type="text"
            inputMode="numeric"
            pattern="[0-9]*"
            placeholder="Nhập ID truyện trực tiếp (VD: 101)"
            value={selectedComicId ?? ""}
            onChange={(event) => handleSelectComic(event.target.value.trim() ? event.target.value.trim() : null)}
            className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
          />

          {/* Selected comic info */}
          {(selectedComic || (selectedComicId && comicInfoQuery.data)) && (
            <div className="rounded-xl border border-primary/30 bg-primary/10 p-3">
              <div className="flex items-center gap-3">
                {(selectedComic?.cover_url || comicInfoQuery.data?.cover_url) && (
                  <img
                    src={selectedComic?.cover_url || comicInfoQuery.data?.cover_url || ""}
                    alt={selectedComic?.name || comicInfoQuery.data?.name || ""}
                    className="h-12 w-9 rounded object-cover"
                  />
                )}
                <div className="flex-1 min-w-0">
                  <div className="font-semibold text-sm">
                    {selectedComic?.name || comicInfoQuery.data?.name || `Truyện ID: ${selectedComicId}`}
                  </div>
                  <div className="text-xs text-surface-foreground/60">
                    ID: {selectedComic?.id || comicInfoQuery.data?.id || selectedComicId} •{" "}
                    {selectedComic?.author || comicInfoQuery.data?.author || "Đang tải..."}
                  </div>
                </div>
              </div>
            </div>
          )}

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
          <div className="overflow-hidden rounded-lg border border-surface-muted/60 bg-surface/80 shadow">
            <table className="min-w-full text-sm">
              <thead className="bg-surface-muted/40 text-xs uppercase tracking-wide text-surface-foreground/60">
                <tr>
                  <th scope="col" className="px-4 py-3 text-left font-semibold">Chương (ID)</th>
                  <th scope="col" className="px-4 py-3 text-left font-semibold">Nội dung</th>
                  <th scope="col" className="px-4 py-3 text-left font-semibold">Tạo lúc</th>
                  <th scope="col" className="px-4 py-3 text-left font-semibold">Cập nhật</th>
                  <th scope="col" className="px-4 py-3 text-left font-semibold">Hành động</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-surface-muted/40 text-surface-foreground/80">
                {chaptersQuery.data.map((chapter) => {
                  const isEditing = editingChapterId === chapter.id;
                  const isDeleting = deleteMutation.isPending && deleteMutation.variables === chapter.id;

                  return (
                    <tr
                      key={chapter.id}
                      className={`transition ${
                        isEditing ? "bg-primary/15 text-primary-foreground" : "hover:bg-surface-muted/40"
                      }`}
                    >
                      <td className="px-4 py-3 text-xs text-surface-foreground/70">
                        <div className="flex flex-col gap-1">
                          <span className="font-semibold">#{chapter.chapter}</span>
                          <span className="text-surface-foreground/50 font-mono text-[10px]">ID: {chapter.id}</span>
                        </div>
                      </td>
                      <td className="px-4 py-3 text-xs text-surface-foreground/70">
                        <p className="line-clamp-3" title={chapter.content}>
                          {chapter.content}
                        </p>
                      </td>
                      <td className="px-4 py-3 text-xs text-surface-foreground/60">
                        {new Date(chapter.created_at).toLocaleString()}
                      </td>
                      <td className="px-4 py-3 text-xs text-surface-foreground/60">
                        {new Date(chapter.updated_at).toLocaleString()}
                      </td>
                      <td className="px-4 py-3">
                        <div className="flex flex-wrap items-center gap-2">
                          <button
                            type="button"
                            onClick={() => {
                              setEditingChapterId(chapter.id);
                              setFormState({ comic_id: chapter.comic_id, chapter: chapter.chapter, content: chapter.content });
                            }}
                            className="inline-flex items-center gap-2 rounded-md border border-primary/40 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/10"
                          >
                            <Pencil className="h-3.5 w-3.5" />
                            Sửa
                          </button>
                          <button
                            type="button"
                            onClick={() => deleteMutation.mutate(chapter.id)}
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
        ) : (
          <p className="text-sm text-surface-foreground/60">Truyện chưa có chương nào.</p>
        )}
      </section>
    </div>
  );
};

export default AdminChaptersPage;
