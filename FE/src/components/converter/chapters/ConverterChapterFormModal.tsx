"use client";

import { FormEvent, useEffect, useState } from "react";
import { X } from "lucide-react";

import { useCreateConverterChapterMutation, useUpdateConverterChapterMutation } from "@services/converter";

const DEFAULT_FORM_STATE = {
  chapter: 1,
  content: "",
};

type FormState = typeof DEFAULT_FORM_STATE;

type ConverterChapterFormModalProps = {
  open: boolean;
  mode: "create" | "edit";
  comicId?: string;
  chapter?: ComicChapterResponse;
  onClose: () => void;
  onSuccess?: () => void;
};

const ConverterChapterFormModal = ({ open, mode, comicId, chapter, onClose, onSuccess }: ConverterChapterFormModalProps) => {
  const [formState, setFormState] = useState<FormState>({ ...DEFAULT_FORM_STATE });
  const createMutation = useCreateConverterChapterMutation();
  const updateMutation = useUpdateConverterChapterMutation();

  useEffect(() => {
    if (mode === "edit" && chapter) {
      setFormState({
        chapter: chapter.chapter,
        content: chapter.content,
      });
      return;
    }
    setFormState({ ...DEFAULT_FORM_STATE });
  }, [mode, chapter]);

  if (!open) {
    return null;
  }

  const isSubmitting = createMutation.isPending || updateMutation.isPending;
  const chapterNumber = Number(formState.chapter) || 1;

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!comicId) return;

    const basePayload = {
      comic_id: comicId,
      chapter: chapterNumber,
      content: formState.content.trim(),
    };

    if (mode === "create") {
      createMutation.mutate(basePayload, {
        onSuccess: () => {
          onSuccess?.();
        },
      });
      return;
    }

    if (!chapter) return;

    updateMutation.mutate(
      {
        id: chapter.id,
        ...basePayload,
      },
      {
        onSuccess: () => {
          onSuccess?.();
        },
      },
    );
  };

  return (
    <div className="fixed inset-0 z-50 flex items-start justify-center bg-black/50 px-4 py-10">
      <div className="w-full max-w-3xl rounded-2xl border border-surface-muted/70 bg-surface shadow-2xl">
        <header className="flex items-center justify-between border-b border-surface-muted/50 px-6 py-4">
          <div>
            <p className="text-xs uppercase tracking-wide text-primary">{mode === "create" ? "Tạo chương" : "Cập nhật chương"}</p>
            <h2 className="text-lg font-semibold text-primary-foreground">
              {mode === "create" ? `Truyện ${comicId}` : chapter ? `Chương ${chapter.chapter}` : "Chỉnh sửa chương"}
            </h2>
          </div>
          <button
            type="button"
            onClick={onClose}
            className="inline-flex h-9 w-9 items-center justify-center rounded-full border border-surface-muted/60 text-surface-foreground/60 transition hover:border-primary hover:text-primary"
            aria-label="Đóng"
          >
            <X className="h-4 w-4" />
          </button>
        </header>
        <form onSubmit={handleSubmit} className="space-y-4 px-6 py-5">
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-1 text-sm">
              <span className="font-medium text-primary-foreground">ID truyện</span>
              <input
                value={comicId ?? ""}
                readOnly
                className="w-full rounded-xl border border-dashed border-surface-muted/60 bg-surface px-3 py-2 text-surface-foreground/60"
              />
            </label>
            <label className="space-y-1 text-sm">
              <span className="font-medium text-primary-foreground">Số chương</span>
              <input
                type="number"
                min={1}
                value={chapterNumber}
                onChange={(event) => setFormState((prev) => ({ ...prev, chapter: Number(event.target.value) || 1 }))}
                className="w-full rounded-xl border border-surface-muted/60 bg-surface px-3 py-2"
                required
              />
            </label>
          </div>
          <label className="space-y-1 text-sm">
            <span className="font-medium text-primary-foreground">Nội dung chương</span>
            <textarea
              required
              value={formState.content}
              onChange={(event) => setFormState((prev) => ({ ...prev, content: event.target.value }))}
              className="min-h-[240px] w-full rounded-xl border border-surface-muted/60 bg-surface px-3 py-2"
              placeholder="Dán nội dung sau khi chuyển đổi"
            />
          </label>
          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={onClose}
              className="rounded-full border border-surface-muted/70 px-5 py-2 text-sm font-semibold text-surface-foreground/70 hover:border-primary/40 hover:text-primary"
              disabled={isSubmitting}
            >
              Hủy
            </button>
            <button
              type="submit"
              className="rounded-full bg-primary px-5 py-2 text-sm font-semibold text-white shadow hover:bg-primary/90"
              disabled={isSubmitting || !comicId}
            >
              {isSubmitting ? "Đang lưu..." : mode === "create" ? "Tạo chương" : "Cập nhật"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ConverterChapterFormModal;
