"use client";

import { useState } from "react";
import { MessageSquareText, RefreshCcw, Search, Trash2 } from "lucide-react";

import { formatRelativeTime } from "@helpers/format";
import { useConverterCommentsQuery, useDeleteConverterCommentMutation } from "@services/converter";

const ConverterCommentsPage = () => {
  const [comicIdInput, setComicIdInput] = useState("");
  const [selectedComicId, setSelectedComicId] = useState<string | undefined>(undefined);

  const { data, isLoading, isFetching, refetch } = useConverterCommentsQuery(selectedComicId, { enabled: Boolean(selectedComicId) });
  const deleteMutation = useDeleteConverterCommentMutation();

  const handleFetchComments = () => {
    if (!comicIdInput.trim()) {
      alert("Vui lòng nhập ID truyện");
      return;
    }
    setSelectedComicId(comicIdInput.trim());
  };

  const handleDelete = async (comment: ComicCommentResponse) => {
    if (!window.confirm(`Xóa bình luận: "${comment.comment.slice(0, 30)}"?`)) {
      return;
    }

    try {
      await deleteMutation.mutateAsync(comment.id);
      await refetch();
    } catch (error) {
      console.error("Delete comment failed", error);
    }
  };

  const comments = data ?? [];
  const targetComic = selectedComicId ? `truyện ${selectedComicId}` : "";

  return (
    <section className="space-y-6">
      <header className="rounded-2xl border border-surface-muted/60 bg-surface p-4 shadow-sm">
        <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div>
            <p className="text-xs uppercase tracking-wide text-primary">Quản lý bình luận</p>
            <h1 className="text-xl font-semibold text-primary-foreground">Tra cứu bình luận theo truyện</h1>
            <p className="text-sm text-surface-foreground/70">Nhập ID truyện để xem và xử lý các bình luận tương ứng.</p>
          </div>
          <div className="flex flex-1 items-center gap-2 rounded-2xl border border-surface-muted/60 bg-surface-muted/20 px-4 py-2">
            <Search className="h-4 w-4 text-surface-foreground/60" />
            <input
              type="text"
              className="flex-1 bg-transparent text-sm outline-none"
              placeholder="Nhập ID truyện (snowflake)"
              value={comicIdInput}
              onChange={(event) => setComicIdInput(event.target.value)}
            />
            <button
              type="button"
              className="rounded-full bg-primary px-4 py-2 text-xs font-semibold text-white shadow hover:bg-primary/90"
              onClick={handleFetchComments}
            >
              Tải bình luận
            </button>
          </div>
        </div>
      </header>

      {selectedComicId ? (
        <div className="rounded-2xl border border-surface-muted/60 bg-surface shadow-sm">
          <div className="flex flex-wrap items-center justify-between gap-2 border-b border-surface-muted/60 px-4 py-3 text-sm text-surface-foreground/70">
            <span>{comments.length > 0 ? `${comments.length} bình luận thuộc ${targetComic}` : `Chưa có bình luận cho ${targetComic}`}</span>
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
                  <th className="px-4 py-3 text-left">Bình luận</th>
                  <th className="px-4 py-3 text-left">Đánh giá</th>
                  <th className="px-4 py-3 text-left">Thời gian</th>
                  <th className="px-4 py-3 text-left">Thao tác</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-surface-muted/50">
                {isLoading && (
                  <tr>
                    <td colSpan={4} className="px-4 py-6 text-center text-surface-foreground/60">
                      Đang tải bình luận...
                    </td>
                  </tr>
                )}

                {!isLoading && comments.length === 0 && (
                  <tr>
                    <td colSpan={4} className="px-4 py-8 text-center text-surface-foreground/60">
                      Chưa có bình luận nào cho truyện này.
                    </td>
                  </tr>
                )}

                {comments.map((comment) => (
                  <tr key={comment.id} className="hover:bg-primary/5">
                    <td className="px-4 py-3">
                      <div className="flex items-start gap-3">
                        <span className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-primary">
                          <MessageSquareText className="h-4 w-4" />
                        </span>
                        <div>
                          <p className="text-sm font-semibold text-primary-foreground">{comment.comment}</p>
                          <p className="text-xs text-surface-foreground/60">
                            ID bình luận: {comment.id} • Người dùng: {comment.user_id}
                          </p>
                          {comment.comic_chapter_id && (
                            <p className="text-xs text-surface-foreground/60">Chương: {comment.comic_chapter_id}</p>
                          )}
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-sm text-surface-foreground/70">
                      {comment.is_rate ? (
                        <span className="inline-flex items-center gap-1 rounded-full bg-amber-50 px-3 py-1 text-xs font-semibold text-amber-600">
                          ⭐ {comment.rate_star}
                        </span>
                      ) : (
                        <span className="text-xs text-surface-foreground/50">Không đánh giá</span>
                      )}
                    </td>
                    <td className="px-4 py-3 text-xs text-surface-foreground/60">
                      Tạo {formatRelativeTime(comment.created_at)}
                      <br />Cập nhật {formatRelativeTime(comment.updated_at)}
                    </td>
                    <td className="px-4 py-3">
                      <button
                        type="button"
                        className="inline-flex items-center gap-2 rounded-full border border-red-200 px-3 py-1.5 text-xs font-semibold text-red-500 transition hover:bg-red-50"
                        onClick={() => handleDelete(comment)}
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
          Nhập ID truyện và nhấn "Tải bình luận" để xem danh sách bình luận tương ứng.
        </div>
      )}
    </section>
  );
};

export default ConverterCommentsPage;
