"use client";

import { useMemo, useState } from "react";
import { Pencil, Plus, RefreshCcw, Trash2 } from "lucide-react";

import type { ComicResponse } from "../../../types/comic";
import ConverterComicFormModal from "./ConverterComicFormModal";
import { ComicStatusLabel } from "@const/enum/comic-status";
import { formatRelativeTime } from "@helpers/format";
import { useConverterComicsQuery, useDeleteConverterComicMutation } from "@services/converter";

const DEFAULT_LIMIT = 20;

const ConverterComicsPage = () => {
  const [limit, setLimit] = useState(DEFAULT_LIMIT);
  const [offset, setOffset] = useState(0);
  const [modalState, setModalState] = useState<{ mode: "create" | "edit"; comic?: ComicResponse } | null>(null);
  const queryParams = useMemo(() => ({ limit, offset }), [limit, offset]);

  const { data, isLoading, isFetching, refetch } = useConverterComicsQuery(queryParams);
  const deleteMutation = useDeleteConverterComicMutation();

  const comics = data ?? [];
  const isBusy = isLoading || isFetching || deleteMutation.isPending;

  const closeModal = () => setModalState(null);
  const handleFormSuccess = async () => {
    await refetch();
    closeModal();
  };

  const handleDelete = async (id: string, name: string) => {
    if (!id) return;
    if (!window.confirm(`Xác nhận xóa truyện "${name}"?`)) {
      return;
    }

    try {
      await deleteMutation.mutateAsync(id);
      await refetch();
    } catch (error) {
      console.error("Delete comic failed", error);
    }
  };

  return (
    <section className="space-y-6">
      <header className="flex flex-col gap-3 rounded-2xl border border-surface-muted/60 bg-surface/80 p-4 shadow-sm md:flex-row md:items-center md:justify-between">
        <div>
          <p className="text-xs uppercase tracking-wide text-primary">Quản lý truyện</p>
          <h1 className="text-xl font-semibold text-primary-foreground">Danh sách truyện đã đăng</h1>
          <p className="text-sm text-surface-foreground/70">Theo dõi trạng thái duyệt và cập nhật nội dung cho từng truyện.</p>
        </div>
        <div className="flex flex-wrap items-center gap-3">
          <button
            type="button"
            className="inline-flex items-center gap-2 rounded-full border border-primary/50 px-4 py-2 text-sm font-medium text-primary transition hover:bg-primary/10"
            onClick={() => refetch()}
            disabled={isBusy}
          >
            <RefreshCcw className={`h-4 w-4 ${isFetching ? "animate-spin" : ""}`} />
            Làm mới
          </button>
          <button
            type="button"
            className="inline-flex items-center gap-2 rounded-full bg-primary px-4 py-2 text-sm font-semibold text-white shadow hover:bg-primary/90"
            onClick={() => setModalState({ mode: "create" })}
          >
            <Plus className="h-4 w-4" />
            Tạo truyện mới
          </button>
        </div>
      </header>

      <div className="rounded-2xl border border-surface-muted/60 bg-surface shadow-sm">
        <div className="flex items-center justify-between border-b border-surface-muted/60 px-4 py-3 text-sm text-surface-foreground/70">
          <div className="flex items-center gap-2">
            <span>Giới hạn mỗi lần:</span>
            <select
              className="rounded-lg border border-surface-muted/60 bg-transparent px-2 py-1"
              value={limit}
              onChange={(event) => {
                setLimit(Number(event.target.value));
                setOffset(0);
              }}
            >
              {[10, 20, 50].map((value) => (
                <option key={value} value={value}>
                  {value} truyện
                </option>
              ))}
            </select>
          </div>
          <span className="text-xs text-surface-foreground/60">
            {isFetching ? "Đang tải dữ liệu..." : `Hiển thị ${comics.length} truyện`}
          </span>
        </div>

        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-surface-muted/60 text-sm">
            <thead className="bg-surface-muted/30 text-xs uppercase tracking-wide text-surface-foreground/60">
              <tr>
                <th className="px-4 py-3 text-left">Truyện</th>
                <th className="px-4 py-3 text-left">Chương</th>
                <th className="px-4 py-3 text-left">Trạng thái</th>
                <th className="px-4 py-3 text-left">Cập nhật</th>
                <th className="px-4 py-3 text-left">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-surface-muted/50">
              {isLoading && (
                <tr>
                  <td colSpan={5} className="px-4 py-6 text-center text-surface-foreground/60">
                    Đang tải dữ liệu truyện...
                  </td>
                </tr>
              )}

              {!isLoading && comics.length === 0 && (
                <tr>
                  <td colSpan={5} className="px-4 py-8 text-center text-surface-foreground/60">
                    Bạn chưa có truyện nào. Hãy tạo truyện mới để bắt đầu.
                  </td>
                </tr>
              )}

              {comics.map((comic) => (
                <tr key={comic.id} className="hover:bg-primary/5">
                  <td className="px-4 py-3">
                    <div className="font-semibold text-primary-foreground">{comic.name}</div>
                    <div className="text-xs text-surface-foreground/70">{comic.slug}</div>
                  </td>
                  <td className="px-4 py-3 text-surface-foreground/70">{comic.chap_count}</td>
                  <td className="px-4 py-3">
                    <span className="inline-flex items-center rounded-full bg-primary/10 px-3 py-1 text-xs font-semibold text-primary">
                      {ComicStatusLabel[comic.status]}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-surface-foreground/70">{formatRelativeTime(comic.updated_at)}</td>
                  <td className="px-4 py-3">
                    <div className="flex flex-wrap gap-2">
                      <button
                        type="button"
                        className="inline-flex items-center gap-2 rounded-full border border-surface-muted/80 px-3 py-1.5 text-xs font-semibold text-primary-foreground transition hover:border-primary hover:text-primary"
                        onClick={() => setModalState({ mode: "edit", comic })}
                        disabled={deleteMutation.isPending}
                      >
                        <Pencil className="h-4 w-4" />
                        Sửa
                      </button>
                      <button
                        type="button"
                        className="inline-flex items-center gap-2 rounded-full border border-red-200 px-3 py-1.5 text-xs font-semibold text-red-500 transition hover:bg-red-50"
                        onClick={() => handleDelete(comic.id, comic.name)}
                        disabled={deleteMutation.isPending}
                      >
                        <Trash2 className="h-4 w-4" />
                        Xóa
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
      <ConverterComicFormModal
        open={Boolean(modalState)}
        mode={modalState?.mode ?? "create"}
        comic={modalState?.comic}
        onClose={closeModal}
        onSuccess={handleFormSuccess}
      />
    </section>
  );
};

export default ConverterComicsPage;
