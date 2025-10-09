"use client";

import { useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { AlertTriangle, KeyRound, ListRestart, RefreshCcw, ShieldCheck, Trash2, UserCircle2 } from "lucide-react";

import {
  fetchUserById,
  fetchUserRefreshTokens,
  fetchUsers,
  revokeAllUserRefreshTokens,
  revokeUserRefreshToken
} from "@services/admin";

const DEFAULT_LIMIT = 15;

const AdminUsersPage = () => {
  const queryClient = useQueryClient();
  const [offset, setOffset] = useState(0);
  const [selectedUserId, setSelectedUserId] = useState<number | null>(null);

  const usersQuery = useQuery({
    queryKey: ["admin-users", offset],
    queryFn: () => fetchUsers({ offset, limit: DEFAULT_LIMIT })
  });

  const selectedUserQuery = useQuery({
    queryKey: ["admin-user", selectedUserId],
    queryFn: () => fetchUserById(selectedUserId!),
    enabled: selectedUserId !== null
  });

  const refreshTokensQuery = useQuery({
    queryKey: ["admin-user-refresh-tokens", selectedUserId],
    queryFn: () => fetchUserRefreshTokens(selectedUserId!),
    enabled: selectedUserId !== null
  });

  const revokeTokenMutation = useMutation({
    mutationFn: ({ userId, tokenId }: { userId: number; tokenId: number }) => revokeUserRefreshToken(userId, tokenId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["admin-user-refresh-tokens", variables.userId] });
    }
  });

  const revokeAllMutation = useMutation({
    mutationFn: (userId: number) => revokeAllUserRefreshTokens(userId),
    onSuccess: (data, userId) => {
      queryClient.invalidateQueries({ queryKey: ["admin-user-refresh-tokens", userId] });
    }
  });

  const isUserSelected = useMemo(() => selectedUserId !== null, [selectedUserId]);

  return (
    <div className="space-y-10">
      <section className="space-y-4">
        <header className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-xs uppercase tracking-[0.4em] text-primary/70">Quản lý người dùng</p>
            <h2 className="text-2xl font-semibold text-primary-foreground">Danh sách tài khoản hệ thống</h2>
          </div>
          <button
            type="button"
            onClick={() => queryClient.invalidateQueries({ queryKey: ["admin-users", offset] })}
            className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
          >
            <RefreshCcw className="h-4 w-4" />
            Làm mới
          </button>
        </header>
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {usersQuery.isLoading && <p className="text-sm text-surface-foreground/60">Đang tải dữ liệu...</p>}
          {usersQuery.isError && (
            <p className="text-sm text-red-300">Không thể tải danh sách người dùng. Vui lòng thử lại.</p>
          )}
          {usersQuery.data?.map((user) => (
            <article
              key={user.id}
              className={`flex cursor-pointer flex-col gap-3 rounded-2xl border px-5 py-4 transition ${
                selectedUserId === user.id
                  ? "border-primary/60 bg-primary/15"
                  : "border-surface-muted bg-surface/70 hover:border-primary/40"
              }`}
              onClick={() => setSelectedUserId(user.id)}
            >
              <div className="flex items-start gap-3">
                <span className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-primary">
                  <UserCircle2 className="h-5 w-5" />
                </span>
                <div>
                  <h4 className="text-lg font-semibold text-primary-foreground">{user.full_name || user.name}</h4>
                  <p className="text-xs text-surface-foreground/60">{user.email}</p>
                </div>
              </div>
              <div className="flex items-center justify-between text-xs text-surface-foreground/60">
                <span>ID: {user.id}</span>
                <span>Tạo ngày {new Date(user.created_at).toLocaleDateString()}</span>
              </div>
            </article>
          ))}
        </div>
        <div className="flex items-center justify-between text-xs text-surface-foreground/60">
          <button
            type="button"
            onClick={() => setOffset((prev) => Math.max(prev - DEFAULT_LIMIT, 0))}
            disabled={offset === 0}
            className="rounded-full border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground disabled:opacity-60"
          >
            Trang trước
          </button>
          <span>
            {offset + 1} - {offset + DEFAULT_LIMIT}
          </span>
          <button
            type="button"
            onClick={() => setOffset((prev) => prev + DEFAULT_LIMIT)}
            className="rounded-full border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground"
          >
            Trang tiếp
          </button>
        </div>
      </section>

      <section className="grid gap-6 rounded-2xl border border-surface-muted bg-surface/70 p-6 md:grid-cols-[0.55fr_1.45fr]">
        <div className="space-y-3">
          <header className="flex items-center gap-2 text-primary-foreground">
            <ShieldCheck className="h-5 w-5" />
            <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Thông tin chi tiết</h3>
          </header>
          {selectedUserId === null ? (
            <p className="text-sm text-surface-foreground/60">Chọn một người dùng để xem hồ sơ.</p>
          ) : selectedUserQuery.isLoading ? (
            <p className="text-sm text-surface-foreground/60">Đang tải thông tin người dùng...</p>
          ) : selectedUserQuery.isError ? (
            <p className="text-sm text-red-300">Không thể tải thông tin người dùng.</p>
          ) : selectedUserQuery.data ? (
            <article className="space-y-3 rounded-2xl border border-surface-muted/70 bg-surface px-4 py-4 text-sm text-surface-foreground/80">
              <div>
                <p className="text-xs uppercase tracking-wide text-surface-foreground/60">Tên hiển thị</p>
                <p className="text-base font-semibold text-primary-foreground">{selectedUserQuery.data.full_name || selectedUserQuery.data.name}</p>
              </div>
              <div>
                <p className="text-xs uppercase tracking-wide text-surface-foreground/60">Email</p>
                <p>{selectedUserQuery.data.email}</p>
              </div>
              <div>
                <p className="text-xs uppercase tracking-wide text-surface-foreground/60">Ngày tạo</p>
                <p>{new Date(selectedUserQuery.data.created_at).toLocaleString()}</p>
              </div>
            </article>
          ) : null}
        </div>
        <div className="space-y-4">
          <header className="flex items-center justify-between">
            <div className="flex items-center gap-2 text-primary-foreground">
              <KeyRound className="h-5 w-5" />
              <h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Refresh Token đang hoạt động</h3>
            </div>
            {isUserSelected && (
              <button
                type="button"
                onClick={() => {
                  if (selectedUserId !== null) {
                    revokeAllMutation.mutate(selectedUserId);
                  }
                }}
                disabled={revokeAllMutation.isPending}
                className="inline-flex items-center gap-2 rounded-full border border-red-500/50 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-red-200 transition hover:bg-red-500/10 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {revokeAllMutation.isPending ? (
                  <ListRestart className="h-4 w-4 animate-spin" />
                ) : (
                  <ListRestart className="h-4 w-4" />
                )}
                Thu hồi toàn bộ
              </button>
            )}
          </header>
          <div className="rounded-2xl border border-surface-muted bg-surface/70 p-5">
            {selectedUserId === null ? (
              <p className="text-sm text-surface-foreground/60">Chọn người dùng để xem token.</p>
            ) : refreshTokensQuery.isLoading ? (
              <p className="text-sm text-surface-foreground/60">Đang tải danh sách token...</p>
            ) : refreshTokensQuery.isError ? (
              <p className="text-sm text-red-300">Không thể tải token của người dùng.</p>
            ) : refreshTokensQuery.data && refreshTokensQuery.data.length > 0 ? (
              <ul className="space-y-3 text-xs text-surface-foreground/70">
                {refreshTokensQuery.data.map((token) => (
                  <li
                    key={token.id}
                    className="rounded-xl border border-surface-muted/60 bg-surface px-4 py-3"
                  >
                    <p className="font-semibold text-primary-foreground">ID: {token.id}</p>
                    <p className="mt-1 break-all">Token: {token.token}</p>
                    <div className="mt-2 flex flex-wrap gap-3 text-xs text-surface-foreground/60">
                      <span>Hết hạn: {new Date(token.expires_at).toLocaleString()}</span>
                      <span>{token.is_active ? "Đang hoạt động" : "Đã khóa"}</span>
                      <span>Tạo lúc: {new Date(token.created_at).toLocaleString()}</span>
                    </div>
                    <button
                      type="button"
                      onClick={() =>
                        revokeTokenMutation.mutate({
                          userId: selectedUserId!,
                          tokenId: token.id
                        })
                      }
                      className="mt-3 inline-flex items-center gap-2 rounded-full border border-red-500/50 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-red-200 transition hover:bg-red-500/10"
                    >
                      {revokeTokenMutation.isPending && revokeTokenMutation.variables?.tokenId === token.id ? (
                        <RefreshCcw className="h-3.5 w-3.5 animate-spin" />
                      ) : (
                        <Trash2 className="h-3.5 w-3.5" />
                      )}
                      Thu hồi token
                    </button>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="text-sm text-surface-foreground/60">Người dùng hiện không có token nào.</p>
            )}
          </div>
          {revokeAllMutation.isIdle ? null : revokeAllMutation.isError ? (
            <p className="flex items-center gap-2 text-xs text-red-300">
              <AlertTriangle className="h-4 w-4" /> Không thể thu hồi toàn bộ token.
            </p>
          ) : revokeAllMutation.isSuccess ? (
            <p className="flex items-center gap-2 text-xs text-emerald-300">
              <ShieldCheck className="h-4 w-4" /> Đã thu hồi tất cả token cho người dùng này.
            </p>
          ) : null}
        </div>
      </section>
    </div>
  );
};

export default AdminUsersPage;
