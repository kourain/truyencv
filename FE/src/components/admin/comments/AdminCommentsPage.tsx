"use client";

import { FormEvent, useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Filter, MessageSquare, RefreshCcw, ShieldAlert, Trash2 } from "lucide-react";

import { deleteComicComment, fetchCommentById, fetchComments } from "@services/admin";

type CommentFilterType = "comic" | "chapter" | "user" | "replies";

type CommentFilterState = {
  type: CommentFilterType;
  id: string | null;
};

const initialFilter: CommentFilterState = {
  type: "comic",
  id: null
};

const AdminCommentsPage = () => {
  const queryClient = useQueryClient();
  const [filter, setFilter] = useState<CommentFilterState>(initialFilter);
  const [focusedCommentId, setFocusedCommentId] = useState<string | null>(null);

  const commentsQuery = useQuery({
    queryKey: ["admin-comments", filter.type, filter.id],
    queryFn: () => fetchComments({ type: filter.type, id: filter.id! }),
    enabled: filter.id !== null
  });

  const singleCommentQuery = useQuery({
    queryKey: ["admin-comment", focusedCommentId],
    queryFn: () => fetchCommentById(focusedCommentId!),
    enabled: focusedCommentId !== null
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteComicComment(id),
    onSuccess: () => {
      if (filter.id !== null) {
        queryClient.invalidateQueries({ queryKey: ["admin-comments", filter.type, filter.id] });
      }
      if (focusedCommentId !== null) {
        queryClient.invalidateQueries({ queryKey: ["admin-comment", focusedCommentId] });
      }
    }
  });

  const isFilterReady = useMemo(() => filter.id !== null, [filter]);

  const handleFilterSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!isFilterReady) {
      return;
    }
    queryClient.invalidateQueries({ queryKey: ["admin-comments", filter.type, filter.id] });
  };

  return (
    <div className="space-y-10">
      <section className="grid gap-6 rounded-2xl border border-surface-muted bg-surface/70 p-6 md:grid-cols-[0.45fr_1.55fr]">
        <form onSubmit={handleFilterSubmit} className="space-y-4">
          <header>
            <p className="text-xs uppercase tracking-[0.4em] text-primary/70">Bộ lọc bình luận</p>
            <h2 className="text-lg font-semibold text-primary-foreground">Tìm theo truyện, chương, người dùng</h2>
          </header>
          <label className="space-y-2 text-sm">
            <span className="font-medium text-primary-foreground">Loại bộ lọc</span>
            <select
              value={filter.type}
              onChange={(event) => setFilter((prev) => ({ ...prev, type: event.target.value as CommentFilterType }))}
              className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
            >
              <option value="comic">Theo truyện</option>
              <option value="chapter">Theo chương</option>
              <option value="user">Theo người dùng</option>
              <option value="replies">Theo bình luận gốc</option>
            </select>
          </label>
          <label className="space-y-2 text-sm">
            <span className="font-medium text-primary-foreground">ID tương ứng</span>
            <input
              type="text"
              inputMode="numeric"
              pattern="[0-9]*"
              value={filter.id ?? ""}
              onChange={(event) => setFilter((prev) => ({ ...prev, id: event.target.value.trim() ? event.target.value.trim() : null }))}
              className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              placeholder={filter.type === "user" ? "ID người dùng" : "ID truyện/chương"}
            />
          </label>
          <button
            type="submit"
            disabled={!isFilterReady}
            className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
          >
            <Filter className="h-4 w-4" />
            Áp dụng bộ lọc
          </button>
        </form>
        <div className="space-y-3">
          <header className="flex items-center justify-between">
            <h3 className="text-lg font-semibold text-primary-foreground">Danh sách bình luận</h3>
            <button
              type="button"
              onClick={() => {
                if (filter.id !== null) {
                  queryClient.invalidateQueries({ queryKey: ["admin-comments", filter.type, filter.id] });
                }
              }}
              className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
            >
              <RefreshCcw className="h-4 w-4" />
              Làm mới
            </button>
          </header>
          <div className="rounded-2xl border border-surface-muted bg-surface/70 p-5">
            {!isFilterReady ? (
              <p className="text-sm text-surface-foreground/60">Nhập bộ lọc để tải bình luận.</p>
            ) : commentsQuery.isLoading ? (
              <p className="text-sm text-surface-foreground/60">Đang tải dữ liệu...</p>
            ) : commentsQuery.isError ? (
              <p className="text-sm text-red-300">Không thể tải danh sách bình luận. Vui lòng kiểm tra ID.</p>
            ) : commentsQuery.data && commentsQuery.data.length > 0 ? (
              <ul className="space-y-3 text-sm text-surface-foreground/80">
                {commentsQuery.data.map((comment) => (
                  <li
                    key={comment.id}
                    className="rounded-xl border border-surface-muted/60 bg-surface px-4 py-3 transition hover:border-primary/60"
                  >
                    <div className="flex items-start justify-between gap-3">
                      <div>
                        <p className="text-sm text-primary-foreground">{comment.comment}</p>
                        <p className="mt-1 text-xs text-surface-foreground/60">
                          #{comment.id} • Truyện {comment.comic_id}
                          {comment.comic_chapter_id ? ` • Chương ${comment.comic_chapter_id}` : ""} • Người dùng {comment.user_id}
                        </p>
                      </div>
                      <div className="flex gap-2">
                        <button
                          type="button"
                          onClick={() => setFocusedCommentId(comment.id)}
                          className="text-xs font-semibold uppercase tracking-wide text-primary transition hover:text-primary/80"
                        >
                          Chi tiết
                        </button>
                        <button
                          type="button"
                          onClick={() => deleteMutation.mutate(comment.id)}
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
                  </li>
                ))}
              </ul>
            ) : (
              <p className="text-sm text-surface-foreground/60">Không tìm thấy bình luận phù hợp.</p>
            )}
          </div>
        </div>
      </section>

      <section className="rounded-2xl border border-surface-muted bg-surface/70 p-6">
        <header className="flex items-center gap-2 text-primary-foreground">
          <MessageSquare className="h-5 w-5" />
          <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Chi tiết bình luận</h3>
        </header>
        {focusedCommentId === null ? (
          <p className="mt-4 text-sm text-surface-foreground/60">Chọn một bình luận để xem nội dung chi tiết.</p>
        ) : singleCommentQuery.isLoading ? (
          <p className="mt-4 text-sm text-surface-foreground/60">Đang tải dữ liệu...</p>
        ) : singleCommentQuery.isError ? (
          <p className="mt-4 text-sm text-red-300">Không thể tải bình luận.</p>
        ) : singleCommentQuery.data ? (
          <div className="mt-4 space-y-3 rounded-2xl border border-surface-muted/60 bg-surface px-5 py-4">
            <p className="text-base text-primary-foreground">{singleCommentQuery.data.comment}</p>
            <div className="flex flex-wrap gap-3 text-xs text-surface-foreground/60">
              <span>ID: {singleCommentQuery.data.id}</span>
              <span>Truyện: {singleCommentQuery.data.comic_id}</span>
              <span>Chương: {singleCommentQuery.data.comic_chapter_id ?? "Không"}</span>
              <span>Người dùng: {singleCommentQuery.data.user_id}</span>
              <span>Like: {singleCommentQuery.data.like}</span>
              <span>Đánh giá: {singleCommentQuery.data.is_rate ? `${singleCommentQuery.data.rate_star ?? 0}★` : "Không"}</span>
            </div>
            <div className="flex flex-wrap gap-3 text-xs text-surface-foreground/60">
              <span>Tạo lúc: {new Date(singleCommentQuery.data.created_at).toLocaleString()}</span>
              <span>Cập nhật: {new Date(singleCommentQuery.data.updated_at).toLocaleString()}</span>
            </div>
            <button
              type="button"
              onClick={() => setFocusedCommentId(null)}
              className="inline-flex items-center gap-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/60 transition hover:text-primary"
            >
              <ShieldAlert className="h-4 w-4" />
              Đóng chi tiết
            </button>
          </div>
        ) : (
          <p className="mt-4 text-sm text-surface-foreground/60">Không có dữ liệu bình luận.</p>
        )}
      </section>
    </div>
  );
};

export default AdminCommentsPage;
