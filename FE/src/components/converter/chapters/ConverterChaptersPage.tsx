"use client";

import { useState } from "react";
import { RefreshCcw, Search, Trash2 } from "lucide-react";

import { formatRelativeTime } from "@helpers/format";
import { useConverterChaptersQuery, useDeleteConverterChapterMutation } from "@services/converter";

const ConverterChaptersPage = () => {
  const [comicIdInput, setComicIdInput] = useState("");
  const [selectedComicId, setSelectedComicId] = useState<string | undefined>(undefined);

  const { data, isLoading, isFetching, refetch } = useConverterChaptersQuery(selectedComicId, { enabled: Boolean(selectedComicId) });
  const deleteMutation = useDeleteConverterChapterMutation();

  const handleLoadChapters = () => {
    if (!comicIdInput.trim()) {
      alert("Vui lòng nhập ID truyện");
      return;
    }
    setSelectedComicId(comicIdInput.trim());
  };

  const handleDelete = async (chapterId: string) => {
    if (!selectedComicId) return;
    if (!window.confirm("Bạn chắc chắn muốn xóa chương này?")) {
      return;
    }

    try {
      await deleteMutation.mutateAsync(chapterId);
      await refetch();
    } catch (error) {
      console.error("Delete chapter failed", error);
    }
  };

  const chapters = data ?? [];

  return (
    <section className="space-y-6">
      <header className="rounded-2xl border border-surface-muted/60 bg-surface p-4 shadow-sm">
        <p className="text-xs uppercase tracking-wide text-primary">Quản lý chương</p>
        <h1 className="text-xl font-semibold text-primary-foreground">Nhập ID truyện để xem danh sách chương</h1>
        <div className="mt-4 flex flex-col gap-3 md:flex-row md:items-center">
          <div className="flex flex-1 items-center gap-2 rounded-2xl border border-surface-muted/60 bg-surface-muted/20 px-4 py-2">
            <Search className="h-4 w-4 text-surface-foreground/60" />
            <input
              type="text"
              className="flex-1 bg-transparent text-sm outline-none"
              placeholder="Nhập ID truyện (snowflake)"
              value={comicIdInput}
              onChange={(event) => setComicIdInput(event.target.value)}
            />
          </div>
          <button
            type="button"
            className="rounded-full bg-primary px-4 py-2 text-sm font-semibold text-white shadow hover:bg-primary/90"
            onClick={handleLoadChapters}
          >
            Tải chương
          </button>
        </div>
      </header>

      {selectedComicId ? (
        <div className="rounded-2xl border border-surface-muted/60 bg-surface shadow-sm">
          <div className="flex items-center justify-between border-b border-surface-muted/60 px-4 py-3 text-sm text-surface-foreground/70">
            <span>
              {chapters.length > 0 ? `Có ${chapters.length} chương thuộc truyện ${selectedComicId}` : "Không tìm thấy chương nào"}
            </span>
            <button
              type="button"
              className="inline-flex items-center gap-2 text-xs font-semibold text-primary"
              onClick={() => refetch()}
              disabled={isFetching}
            >
              <RefreshCcw className={`h-4 w-4 ${isFetching ? "animate-spin" : ""}`} />
              Làm mới
            </button>
          </div>

          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-surface-muted/60 text-sm">
              <thead className="bg-surface-muted/30 text-xs uppercase tracking-wide text-surface-foreground/60">
                <tr>
                  <th className="px-4 py-3 text-left">Chương</th>
                  <th className="px-4 py-3 text-left">Nội dung</th>
                  <th className="px-4 py-3 text-left">Cập nhật</th>
                  <th className="px-4 py-3 text-left">Thao tác</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-surface-muted/50">
                {isLoading && (
                  <tr>
                    <td colSpan={4} className="px-4 py-6 text-center text-surface-foreground/60">
                      Đang tải danh sách chương...
                    </td>
                  </tr>
                )}

                {!isLoading && chapters.length === 0 && (
                  <tr>
                    <td colSpan={4} className="px-4 py-6 text-center text-surface-foreground/60">
                      Không có dữ liệu chương cho truyện này.
                    </td>
                  </tr>
                )}

                {chapters.map((chapter) => (
                  <tr key={chapter.id} className="hover:bg-primary/5">
                    <td className="px-4 py-3 font-semibold text-primary-foreground">Chương {chapter.chapter}</td>
                    <td className="px-4 py-3 text-surface-foreground/70">
                      <div className="line-clamp-2 text-xs">
                        {chapter.content?.slice(0, 120) || "(Không có nội dung)"}
                      </div>
                    </td>
                    <td className="px-4 py-3 text-surface-foreground/60">{formatRelativeTime(chapter.updated_at)}</td>
                    <td className="px-4 py-3">
                      <button
                        type="button"
                        className="inline-flex items-center gap-2 rounded-full border border-red-200 px-3 py-1.5 text-xs font-semibold text-red-500 transition hover:bg-red-50"
                        onClick={() => handleDelete(chapter.id)}
                        disabled={deleteMutation.isPending}
                      >
                        <Trash2 className="h-4 w-4" />
                        Xóa
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      ) : (
        <div className="rounded-2xl border border-dashed border-surface-muted/60 bg-surface/40 p-10 text-center text-sm text-surface-foreground/60">
          Nhập ID truyện và nhấn "Tải chương" để xem danh sách chương của bạn.
        </div>
      )}
    </section>
  );
};

export default ConverterChaptersPage;
