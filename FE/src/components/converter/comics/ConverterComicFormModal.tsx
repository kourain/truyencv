"use client";

import { FormEvent, useEffect, useMemo, useState } from "react";
import { X } from "lucide-react";

import type { ComicResponse } from "../../../types/comic";
import { ComicStatus, ComicStatusLabel } from "@const/enum/comic-status";
import {
  useCreateConverterComicMutation,
  useUpdateConverterComicMutation,
} from "@services/converter";

const DEFAULT_FORM_STATE = {
  name: "",
  author: "",
  description: "",
  embedded_from: "",
  embedded_from_url: "",
  cover_url: "",
  main_category_id: 1001,
  status: ComicStatus.Continuing,
  chap_count: 0,
  rate: 0,
};

type FormState = typeof DEFAULT_FORM_STATE;

type ConverterComicFormModalProps = {
  open: boolean;
  mode: "create" | "edit";
  comic?: ComicResponse;
  onClose: () => void;
  onSuccess?: () => void;
};

const ConverterComicFormModal = ({ open, mode, comic, onClose, onSuccess }: ConverterComicFormModalProps) => {
  const [formState, setFormState] = useState<FormState>({ ...DEFAULT_FORM_STATE });
  const createMutation = useCreateConverterComicMutation();
  const updateMutation = useUpdateConverterComicMutation();

  useEffect(() => {
    if (mode === "edit" && comic) {
      setFormState({
        name: comic.name,
        author: comic.author,
        description: comic.description,
        embedded_from: comic.embedded_from ?? "",
        embedded_from_url: comic.embedded_from_url ?? "",
        cover_url: comic.cover_url ?? "",
        main_category_id: DEFAULT_FORM_STATE.main_category_id,
        status: comic.status,
        chap_count: comic.chap_count,
        rate: comic.rate,
      });
      return;
    }
    setFormState({ ...DEFAULT_FORM_STATE });
  }, [mode, comic]);

  const statusOptions = useMemo(() => Object.values(ComicStatus).filter((value) => typeof value === "number") as ComicStatus[], []);
  const isSubmitting = createMutation.isPending || updateMutation.isPending;

  if (!open) {
    return null;
  }

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const mainCategory = Number.isNaN(formState.main_category_id) ? undefined : formState.main_category_id;

    const basePayload = {
      name: formState.name.trim(),
      author: formState.author.trim(),
      description: formState.description.trim(),
      embedded_from: formState.embedded_from.trim() || null,
      embedded_from_url: formState.embedded_from_url.trim() || null,
      cover_url: formState.cover_url.trim() || null,
      main_category_id: mainCategory,
      status: formState.status,
    };

    if (mode === "create") {
      createMutation.mutate(basePayload, {
        onSuccess: () => {
          onSuccess?.();
        },
      });
      return;
    }

    if (!comic) return;

    updateMutation.mutate(
      {
        id: comic.id,
        ...basePayload,
        embedded_from: basePayload.embedded_from,
        embedded_from_url: basePayload.embedded_from_url,
        cover_url: basePayload.cover_url,
        chap_count: formState.chap_count,
        rate: formState.rate,
      },
      {
        onSuccess: () => {
          onSuccess?.();
        },
      },
    );
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4 py-10">
      <div className="w-full max-w-2xl rounded-2xl border border-surface-muted/70 bg-surface shadow-2xl">
        <header className="flex items-center justify-between border-b border-surface-muted/50 px-6 py-4">
          <div>
            <p className="text-xs uppercase tracking-wide text-primary">{mode === "create" ? "Tạo truyện" : "Cập nhật truyện"}</p>
            <h2 className="text-lg font-semibold text-primary-foreground">
              {mode === "create" ? "Thêm truyện mới" : comic?.name ?? "Chỉnh sửa truyện"}
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
              <span className="font-medium text-primary-foreground">Tên truyện</span>
              <input
                required
                value={formState.name}
                onChange={(event) => setFormState((prev) => ({ ...prev, name: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted/60 bg-surface px-3 py-2"
                placeholder="Ví dụ: Vạn Cổ Chí Tôn"
              />
            </label>
            <label className="space-y-1 text-sm">
              <span className="font-medium text-primary-foreground">Tác giả</span>
              <input
                required
                value={formState.author}
                onChange={(event) => setFormState((prev) => ({ ...prev, author: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted/60 bg-surface px-3 py-2"
                placeholder="Tên tác giả"
              />
            </label>
          </div>
          <label className="space-y-1 text-sm">
            <span className="font-medium text-primary-foreground">Mô tả ngắn</span>
            <textarea
              required
              value={formState.description}
              onChange={(event) => setFormState((prev) => ({ ...prev, description: event.target.value }))}
              className="min-h-[120px] w-full rounded-xl border border-surface-muted/60 bg-surface px-3 py-2"
              placeholder="Giới thiệu nội dung chính"
            />
          </label>
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-1 text-sm">
              <span className="font-medium text-primary-foreground">Nguồn chuyển thể</span>
              <input
                value={formState.embedded_from}
                onChange={(event) => setFormState((prev) => ({ ...prev, embedded_from: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted/60 bg-surface px-3 py-2"
                placeholder="Tên web/nhóm convert"
              />
            </label>
            <label className="space-y-1 text-sm">
              <span className="font-medium text-primary-foreground">URL nguồn</span>
              <input
                value={formState.embedded_from_url}
                onChange={(event) => setFormState((prev) => ({ ...prev, embedded_from_url: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted/60 bg-surface px-3 py-2"
                placeholder="https://..."
              />
            </label>
          </div>
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-1 text-sm">
              <span className="font-medium text-primary-foreground">Ảnh bìa</span>
              <input
                value={formState.cover_url}
                onChange={(event) => setFormState((prev) => ({ ...prev, cover_url: event.target.value }))}
                className="w-full rounded-xl border border-surface-muted/60 bg-surface px-3 py-2"
                placeholder="URL ảnh bìa"
              />
            </label>
            <label className="space-y-1 text-sm">
              <span className="font-medium text-primary-foreground">Thể loại chính (ID)</span>
              <input
                type="number"
                min={1001}
                max={1999}
                value={Number.isNaN(formState.main_category_id) ? "" : formState.main_category_id}
                onChange={(event) =>
                  setFormState((prev) => ({ ...prev, main_category_id: Number(event.target.value) || 1001 }))
                }
                className="w-full rounded-xl border border-surface-muted/60 bg-surface px-3 py-2"
                placeholder="1001"
              />
            </label>
          </div>
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-1 text-sm">
              <span className="font-medium text-primary-foreground">Trạng thái</span>
              <select
                value={formState.status}
                onChange={(event) =>
                  setFormState((prev) => ({ ...prev, status: Number(event.target.value) as ComicStatus }))
                }
                className="w-full rounded-xl border border-surface-muted/60 bg-surface px-3 py-2"
              >
                {statusOptions.map((value) => (
                  <option key={value} value={value}>
                    {ComicStatusLabel[value as ComicStatus]}
                  </option>
                ))}
              </select>
            </label>
            {mode === "edit" ? (
              <div className="grid grid-cols-2 gap-4">
                <label className="space-y-1 text-sm">
                  <span className="font-medium text-primary-foreground">Tổng chương</span>
                  <input
                    type="number"
                    value={formState.chap_count}
                    readOnly
                    className="w-full rounded-xl border border-dashed border-surface-muted/60 bg-surface px-3 py-2 text-surface-foreground/60"
                  />
                </label>
                <label className="space-y-1 text-sm">
                  <span className="font-medium text-primary-foreground">Đánh giá</span>
                  <input
                    type="number"
                    step="0.1"
                    value={formState.rate}
                    readOnly
                    className="w-full rounded-xl border border-dashed border-surface-muted/60 bg-surface px-3 py-2 text-surface-foreground/60"
                  />
                </label>
              </div>
            ) : (
              <div />
            )}
          </div>
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
              disabled={isSubmitting}
            >
              {isSubmitting ? "Đang lưu..." : mode === "create" ? "Tạo truyện" : "Cập nhật"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ConverterComicFormModal;
