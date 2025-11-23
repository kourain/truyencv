"use client";

import { formatRelativeTime } from "@helpers/format";

interface DiscussionsPanelProps {
  discussions?: ComicDetailDiscussionResponse[];
  isLoading?: boolean;
  comicId?: string;
  slug?: string;
}

import { useState } from "react";
import { useQueryClient, useMutation } from "@tanstack/react-query";
import { createUserComicComment } from "@services/user/comic-comment.service";
import { useToast } from "@components/providers/ToastProvider";

const DiscussionsPanel = ({ discussions, isLoading = false, comicId, slug }: DiscussionsPanelProps) => {
  const [message, setMessage] = useState("");
  const queryClient = useQueryClient();
  const Toast = useToast()
  const mutation = useMutation({
    mutationFn: (payload: CreateComicCommentRequest) => createUserComicComment(payload),
    onSuccess: () => {
      // refetch comic detail to get new discussions
      if (slug) queryClient.invalidateQueries({ queryKey: ["user-comic-detail", slug] });
      setMessage("");
    },
    onError: (error) => {
      Toast.pushToast({ title: "Lỗi", description: "Lỗi khi tạo bình luận", variant: "error" });
      console.log(error);
    },
  });
  if (isLoading) {
    return (
      <div className="grid gap-3">
        {Array.from({ length: 4 }).map((_, index) => (
          <div key={index} className="h-20 animate-pulse rounded-3xl bg-surface-muted/40" />
        ))}
      </div>
    );
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!comicId) return;
    if (!message.trim()) return;
    mutation.mutate({ comic_id: comicId, comment: message.trim() });
  };

  return (
    <div className="grid gap-3">
      <form onSubmit={handleSubmit} className="flex flex-col gap-3">
        <textarea
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          placeholder="Viết bình luận của bạn..."
          className="min-h-[80px] resize-none rounded-2xl border border-surface-muted/60 bg-surface p-4 text-sm"
        />
        <div className="flex justify-end">
          <button
            type="submit"
            disabled={mutation.isPending || !message.trim() || !comicId}
            className="rounded-full bg-primary px-4 py-2 text-sm font-semibold text-white disabled:opacity-50"
          >
            Gửi bình luận
          </button>
        </div>
      </form>
      {(!discussions?.length) ? (
        <p className="text-sm text-surface-foreground/60">Chưa có thảo luận nào cho truyện này.</p>
      ) : (
        discussions.map((discussion) => (
          <article
            key={discussion.id}
            className="rounded-3xl border border-surface-muted/60 bg-surface px-5 py-4"
          >
            <header className="mb-2 flex items-center justify-between gap-3">
              <div className="flex items-center gap-2 text-sm font-semibold text-primary-foreground">
                <span className="inline-flex h-8 w-8 items-center justify-center rounded-full bg-surface-muted/60 text-primary">
                  {discussion.user_display_name.slice(0, 1).toUpperCase()}
                </span>
                <span>{discussion.user_display_name}</span>
              </div>
              <span className="text-xs uppercase tracking-wide text-surface-foreground/60">
                {formatRelativeTime(discussion.created_at)}
              </span>
            </header>
            <p className="text-sm leading-relaxed text-surface-foreground/80">{discussion.message}</p>
          </article>
        )))}
    </div>
  );
};

export default DiscussionsPanel;
