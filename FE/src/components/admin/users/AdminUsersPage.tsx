"use client";

import { useEffect, useMemo, useState } from "react";
import clsx from "clsx";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { AlertTriangle, Check, Copy, KeyRound, ListRestart, RefreshCcw, ShieldCheck, Trash2, UserCircle2 } from "lucide-react";

import {
  fetchUserById,
  fetchUserRefreshTokens,
  fetchUsers,
  fetchUserRolesByUser,
  revokeAllUserRefreshTokens,
  revokeUserRefreshToken
} from "@services/admin";

const DEFAULT_LIMIT = 15;

const AdminUsersPage = () => {
  const queryClient = useQueryClient();
  const [offset, setOffset] = useState(0);
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);
  const [keyword, setKeyword] = useState("");

  const normalizedKeyword = useMemo(() => keyword.trim(), [keyword]);

  useEffect(() => {
    setOffset(0);
  }, [normalizedKeyword]);

  const usersQuery = useQuery({
    queryKey: ["admin-users", offset, normalizedKeyword],
    queryFn: () =>
      fetchUsers({
        offset,
        limit: DEFAULT_LIMIT,
        keyword: normalizedKeyword || undefined
      })
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

  const userRolesQuery = useQuery({
    queryKey: ["admin-user-roles", selectedUserId],
    queryFn: () => fetchUserRolesByUser(selectedUserId!),
    enabled: selectedUserId !== null
  });

  const revokeTokenMutation = useMutation({
    mutationFn: ({ userId, tokenId }: { userId: string; tokenId: string }) => revokeUserRefreshToken(userId, tokenId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["admin-user-refresh-tokens", variables.userId] });
    }
  });

  const revokeAllMutation = useMutation({
    mutationFn: (userId: string) => revokeAllUserRefreshTokens(userId),
    onSuccess: (_, userId) => {
      queryClient.invalidateQueries({ queryKey: ["admin-user-refresh-tokens", userId] });
    }
  });

  const isUserSelected = useMemo(() => selectedUserId !== null, [selectedUserId]);

  const [copiedTokenId, setCopiedTokenId] = useState<string | null>(null);

  const handleCopyToken = async (token: string, tokenId: string) => {
    try {
      await navigator.clipboard.writeText(token);
      setCopiedTokenId(tokenId);
      setTimeout(() => setCopiedTokenId(null), 2000);
    } catch (error) {
      console.error("Failed to copy token:", error);
    }
  };

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
            onClick={() => queryClient.invalidateQueries({ queryKey: ["admin-users"] })}
            className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
          >
            <RefreshCcw className="h-4 w-4" />
            Làm mới
          </button>
        </header>
        <div className="flex flex-col gap-3 rounded-2xl border border-surface-muted/70 bg-surface/60 p-4 sm:flex-row sm:items-center sm:justify-between">
          <label className="flex w-full items-center gap-3 text-sm text-surface-foreground/70">
            <span className="whitespace-nowrap text-xs font-semibold uppercase tracking-wide">Tìm kiếm</span>
            <input
              value={keyword}
              onChange={(event) => setKeyword(event.target.value)}
              placeholder="Email hoặc mã người dùng"
              className="flex-1 rounded-xl border border-surface-muted bg-surface px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
            />
          </label>
          {normalizedKeyword && (
            <button
              type="button"
              onClick={() => setKeyword("")}
              className="self-end rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground sm:self-center"
            >
              Xóa tìm kiếm
            </button>
          )}
        </div>
        <div className="overflow-hidden rounded-lg border border-surface-muted/60 bg-surface/80 shadow">
          <table className="min-w-full text-sm">
            <thead className="bg-surface-muted/50 text-xs uppercase tracking-wide text-surface-foreground/60">
              <tr>
                <th scope="col" className="px-4 py-3 text-left font-semibold">#</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Tài khoản</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Email</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Ngày tạo</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-surface-muted/40">
              {usersQuery.isLoading && (
                <tr>
                  <td colSpan={4} className="px-4 py-6 text-center text-xs text-surface-foreground/60">
                    Đang tải dữ liệu người dùng...
                  </td>
                </tr>
              )}
              {usersQuery.isError && (
                <tr>
                  <td colSpan={4} className="px-4 py-6 text-center text-xs text-red-300">
                    Không thể tải danh sách người dùng. Vui lòng thử lại.
                  </td>
                </tr>
              )}
              {!usersQuery.isLoading && !usersQuery.isError && usersQuery.data && usersQuery.data.length === 0 && (
                <tr>
                  <td colSpan={4} className="px-4 py-6 text-center text-xs text-surface-foreground/50">
                    Chưa có người dùng nào trong danh sách.
                  </td>
                </tr>
              )}
              {usersQuery.data?.map((user, index) => {
                const isActiveRow = selectedUserId === user.id;

                return (
                  <tr
                    key={user.id}
                    className={clsx(
                      "cursor-pointer transition",
                      isActiveRow
                        ? "bg-primary/15 text-primary-foreground"
                        : "hover:bg-surface-muted/40 text-surface-foreground/80"
                    )}
                    onClick={() => setSelectedUserId(user.id)}
                  >
                    <td className="px-4 py-3 align-middle text-xs text-surface-foreground/60">
                      {offset + index + 1}
                    </td>
                    <td className="px-4 py-3 align-middle">
                      <div className="flex items-center gap-3">
                        <span className="flex h-9 w-9 items-center justify-center rounded-md bg-primary/10 text-primary">
                          <UserCircle2 className="h-4 w-4" />
                        </span>
                        <div className="flex flex-col">
                          <span className="text-sm font-semibold">
                            {user.full_name || user.name}
                          </span>
                          <span className="text-xs text-surface-foreground/60">ID: {user.id}</span>
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-3 align-middle text-xs text-surface-foreground/70">
                      {user.email || "Chưa cập nhật"}
                    </td>
                    <td className="px-4 py-3 align-middle text-xs text-surface-foreground/70">
                      {new Date(user.created_at).toLocaleString()}
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
        <div className="flex items-center justify-between text-xs text-surface-foreground/60">
          <button
            type="button"
            onClick={() => setOffset((prev) => Math.max(prev - DEFAULT_LIMIT, 0))}
            disabled={offset === 0}
            className="rounded-md border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground disabled:opacity-60"
          >
            Trang trước
          </button>
          <span>
            {offset + 1} - {offset + DEFAULT_LIMIT}
          </span>
          <button
            type="button"
            onClick={() => setOffset((prev) => prev + DEFAULT_LIMIT)}
            className="rounded-md border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground"
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
                <p className="text-base font-semibold text-primary-foreground">
                  {selectedUserQuery.data.full_name || selectedUserQuery.data.name}
                </p>
              </div>
              <div>
                <p className="text-xs uppercase tracking-wide text-surface-foreground/60">Email</p>
                <p>{selectedUserQuery.data.email}</p>
              </div>
              <div>
                <p className="text-xs uppercase tracking-wide text-surface-foreground/60">Ngày tạo</p>
                <p>{new Date(selectedUserQuery.data.created_at).toLocaleString()}</p>
              </div>
              <div>
                <p className="text-xs uppercase tracking-wide text-surface-foreground/60">Vai trò hiện có</p>
                {selectedUserId === null ? null : userRolesQuery.isLoading ? (
                  <p className="mt-1 text-xs text-surface-foreground/60">Đang tải vai trò...</p>
                ) : userRolesQuery.isError ? (
                  <p className="mt-1 text-xs text-red-300">Không thể tải vai trò của người dùng.</p>
                ) : userRolesQuery.data && userRolesQuery.data.length > 0 ? (
                  <div className="mt-2 flex flex-wrap gap-2">
                    {userRolesQuery.data.map((role) => (
                      <span
                        key={role.id}
                        className="rounded-full border border-primary/40 bg-primary/10 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-primary"
                      >
                        {role.role_name}
                      </span>
                    ))}
                  </div>
                ) : (
                  <p className="mt-1 text-xs text-surface-foreground/60">Người dùng chưa được gán vai trò.</p>
                )}
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
                {refreshTokensQuery.data.map((token) => {
                  const isRevoking = revokeTokenMutation.isPending && revokeTokenMutation.variables?.tokenId === token.id;
                  const isCopied = copiedTokenId === token.id;
                  const tokenPreview = token.token.length > 60 ? `${token.token.substring(0, 60)}...` : token.token;

                  return (
                    <li key={token.id} className="rounded-xl border border-surface-muted/60 bg-surface px-4 py-3">
                      <div className="flex items-start justify-between gap-3">
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center gap-2">
                            <p className="font-semibold text-primary-foreground">ID: {token.id}</p>
                            <span
                              className={`rounded-full px-2 py-0.5 text-[10px] font-semibold uppercase ${
                                token.is_active
                                  ? "bg-emerald-500/20 text-emerald-300"
                                  : "bg-red-500/20 text-red-300"
                              }`}
                            >
                              {token.is_active ? "Hoạt động" : "Đã khóa"}
                            </span>
                          </div>
                          <div className="mt-2 space-y-1">
                            <div className="flex items-center gap-2">
                              <p className="break-all font-mono text-[10px] text-surface-foreground/60">
                                {tokenPreview}
                              </p>
                              <button
                                type="button"
                                onClick={() => handleCopyToken(token.token, token.id)}
                                className="flex-shrink-0 rounded-md border border-surface-muted/60 p-1.5 transition hover:bg-surface-muted/40"
                                title="Copy token"
                              >
                                {isCopied ? (
                                  <Check className="h-3.5 w-3.5 text-emerald-400" />
                                ) : (
                                  <Copy className="h-3.5 w-3.5 text-surface-foreground/60" />
                                )}
                              </button>
                            </div>
                            {token.token.length > 60 && (
                              <p className="text-[10px] text-surface-foreground/50">
                                Token đã được rút gọn. Click nút copy để xem đầy đủ.
                              </p>
                            )}
                          </div>
                          <div className="mt-2 flex flex-wrap gap-3 text-xs text-surface-foreground/60">
                            <span>Hết hạn: {new Date(token.expires_at).toLocaleString()}</span>
                            <span>Tạo lúc: {new Date(token.created_at).toLocaleString()}</span>
                          </div>
                        </div>
                        <button
                          type="button"
                          onClick={() =>
                            revokeTokenMutation.mutate({
                              userId: selectedUserId!,
                              tokenId: token.id
                            })
                          }
                          disabled={isRevoking || !token.is_active}
                          className="flex-shrink-0 inline-flex items-center gap-2 rounded-full border border-red-500/50 px-3 py-1.5 text-xs font-semibold uppercase tracking-wide text-red-200 transition hover:bg-red-500/10 disabled:cursor-not-allowed disabled:opacity-60"
                        >
                          {isRevoking ? (
                            <RefreshCcw className="h-3.5 w-3.5 animate-spin" />
                          ) : (
                            <Trash2 className="h-3.5 w-3.5" />
                          )}
                          Thu hồi
                        </button>
                      </div>
                    </li>
                  );
                })}
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
