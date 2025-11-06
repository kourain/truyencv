"use client";

import { FormEvent, useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { GraduationCap, PlusCircle, RefreshCcw, Shield, Trash2, Users2 } from "lucide-react";

import {
  createUserRole,
  deleteUserRole,
  fetchUserRoles,
  fetchUserRolesByUser,
  fetchUsersByRole,
  updateUserRole
} from "@services/admin";

const DEFAULT_LIMIT = 20;

type RoleFormState = {
  role_name: string;
  user_id: string | null;
};

type FilterState = {
  type: "user" | "role";
  key: string;
};

const initialForm: RoleFormState = {
  role_name: "",
  user_id: null
};

const initialFilter: FilterState = {
  type: "user",
  key: ""
};

const AdminUserRolesPage = () => {
  const queryClient = useQueryClient();
  const [offset, setOffset] = useState(0);
  const [editingRoleId, setEditingRoleId] = useState<string | null>(null);
  const [formState, setFormState] = useState<RoleFormState>(initialForm);
  const [filter, setFilter] = useState<FilterState>(initialFilter);

  const rolesQuery = useQuery({
    queryKey: ["admin-user-roles", offset],
    queryFn: () => fetchUserRoles({ offset, limit: DEFAULT_LIMIT })
  });

  const filterQuery = useQuery({
    queryKey: ["admin-user-roles-filter", filter.type, filter.key],
    queryFn: () => (filter.type === "user" ? fetchUserRolesByUser(filter.key) : fetchUsersByRole(filter.key)),
    enabled: filter.key.trim().length > 0
  });

  const invalidateRoles = () => {
    queryClient.invalidateQueries({ queryKey: ["admin-user-roles"] });
    queryClient.invalidateQueries({ queryKey: ["admin-user-roles-filter"] });
  };

  const createMutation = useMutation({
    mutationFn: () =>
      createUserRole({
        role_name: formState.role_name,
        user_id: formState.user_id!,
        assigned_by: formState.user_id!
      }),
    onSuccess: () => {
      invalidateRoles();
      resetForm();
    }
  });

  const updateMutation = useMutation({
    mutationFn: () =>
      updateUserRole({
        id: editingRoleId!,
        role_name: formState.role_name,
        user_id: formState.user_id!,
        assigned_by: formState.user_id!
      }),
    onSuccess: () => {
      invalidateRoles();
      resetForm();
    }
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteUserRole(id),
    onSuccess: () => invalidateRoles()
  });

  const resetForm = () => {
    setEditingRoleId(null);
    setFormState(initialForm);
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!formState.role_name.trim() || !formState.user_id) {
      return;
    }
    if (editingRoleId) {
      updateMutation.mutate();
    } else {
      createMutation.mutate();
    }
  };

  const isSubmitting = createMutation.isPending || updateMutation.isPending;
  const isFilterReady = useMemo(() => filter.key.trim().length > 0, [filter.key]);

  return (
    <div className="space-y-10">
      <section className="space-y-4">
        <header className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-xs uppercase tracking-[0.4em] text-primary/70">Gán quyền quản trị</p>
            <h2 className="text-2xl font-semibold text-primary-foreground">
              {editingRoleId ? "Cập nhật phân quyền" : "Thêm phân quyền mới"}
            </h2>
          </div>
          <div className="flex gap-2">
            <button
              type="button"
              onClick={() => queryClient.invalidateQueries({ queryKey: ["admin-user-roles", offset] })}
              className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
            >
              <RefreshCcw className="h-4 w-4" />
              Làm mới
            </button>
            <button
              type="button"
              onClick={resetForm}
              className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary/10 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/20"
            >
              <PlusCircle className="h-4 w-4" />
              Tạo mới
            </button>
          </div>
        </header>
        <form onSubmit={handleSubmit} className="grid gap-4 rounded-2xl border border-surface-muted bg-surface/70 p-6">
          <label className="space-y-2 text-sm">
            <span className="font-medium text-primary-foreground">Tên quyền</span>
            <input
              required
              value={formState.role_name}
              onChange={(event) => setFormState((prev) => ({ ...prev, role_name: event.target.value }))}
              className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              placeholder="VD: Admin, Moderator"
            />
          </label>
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-2 text-sm">
              <span className="font-medium text-primary-foreground">ID người dùng</span>
              <input
                type="text"
                inputMode="numeric"
                pattern="[0-9]*"
                required
                value={formState.user_id ?? ""}
                onChange={(event) =>
                  setFormState((prev) => ({ ...prev, user_id: event.target.value.trim() ? event.target.value.trim() : null }))
                }
                className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              />
            </label>
          </div>
          <div className="flex justify-end gap-3">
            {editingRoleId && (
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
              disabled={isSubmitting}
              className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
            >
              {isSubmitting && (
                <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
              )}
              {editingRoleId ? "Cập nhật" : "Phân quyền"}
            </button>
          </div>
        </form>
      </section>

      <section className="space-y-4">
        <header className="flex items-center justify-between">
          <h3 className="text-lg font-semibold text-primary-foreground">Danh sách phân quyền</h3>
          <div className="flex items-center gap-3 text-xs text-surface-foreground/60">
            <button
              type="button"
              onClick={() => setOffset((prev) => Math.max(prev - DEFAULT_LIMIT, 0))}
              className="rounded-md border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground disabled:opacity-60"
              disabled={offset === 0}
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
        </header>
        <div className="overflow-hidden rounded-lg border border-surface-muted/60 bg-surface/80 shadow">
          <table className="min-w-full text-sm">
            <thead className="bg-surface-muted/40 text-xs uppercase tracking-wide text-surface-foreground/60">
              <tr>
                <th scope="col" className="px-4 py-3 text-left font-semibold">#</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Role</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">User ID</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Gán bởi</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Tạo lúc</th>
                <th scope="col" className="px-4 py-3 text-left font-semibold">Hành động</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-surface-muted/40 text-surface-foreground/80">
              {rolesQuery.isLoading && (
                <tr>
                  <td colSpan={6} className="px-4 py-6 text-center text-xs text-surface-foreground/60">
                    Đang tải dữ liệu phân quyền...
                  </td>
                </tr>
              )}
              {rolesQuery.isError && (
                <tr>
                  <td colSpan={6} className="px-4 py-6 text-center text-xs text-red-300">
                    Không thể tải danh sách phân quyền.
                  </td>
                </tr>
              )}
              {!rolesQuery.isLoading && !rolesQuery.isError && rolesQuery.data && rolesQuery.data.length === 0 && (
                <tr>
                  <td colSpan={6} className="px-4 py-6 text-center text-xs text-surface-foreground/60">
                    Chưa có phân quyền nào.
                  </td>
                </tr>
              )}
              {rolesQuery.data?.map((role, index) => {
                const isEditing = editingRoleId === role.id;
                const isDeleting = deleteMutation.isPending && deleteMutation.variables === role.id;

                return (
                  <tr
                    key={role.id}
                    className={`transition ${
                      isEditing ? "bg-primary/15 text-primary-foreground" : "hover:bg-surface-muted/40"
                    }`}
                  >
                    <td className="px-4 py-3 text-xs text-surface-foreground/60">{offset + index + 1}</td>
                    <td className="px-4 py-3 font-semibold">{role.role_name}</td>
                    <td className="px-4 py-3 text-xs">#{role.user_id}</td>
                    <td className="px-4 py-3 text-xs">#{role.assigned_by}</td>
                    <td className="px-4 py-3 text-xs text-surface-foreground/60">
                      {new Date(role.created_at).toLocaleString()}
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex flex-wrap items-center gap-2">
                        <button
                          type="button"
                          onClick={() => {
                            setEditingRoleId(role.id);
                            setFormState({
                              role_name: role.role_name,
                              user_id: role.user_id
                            });
                          }}
                          className="inline-flex items-center gap-2 rounded-md border border-primary/40 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/10"
                        >
                          <Shield className="h-3.5 w-3.5" />
                          Sửa
                        </button>
                        <button
                          type="button"
                          onClick={() => deleteMutation.mutate(role.id)}
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
      </section>

      <section className="grid gap-6 rounded-2xl border border-surface-muted bg-surface/70 p-6 md:grid-cols-[0.45fr_1.55fr]">
        <form
          className="space-y-4"
          onSubmit={(event) => {
            event.preventDefault();
            if (!isFilterReady) {
              return;
            }
            queryClient.invalidateQueries({ queryKey: ["admin-user-roles-filter", filter.type, filter.key] });
          }}
        >
          <header>
            <p className="text-xs uppercase tracking-[0.35em] text-primary/70">Truy vấn nhanh</p>
            <h3 className="text-lg font-semibold text-primary-foreground">Lọc theo người dùng hoặc role</h3>
          </header>
          <label className="space-y-2 text-sm">
            <span className="font-medium text-primary-foreground">Kiểu lọc</span>
            <select
              value={filter.type}
              onChange={(event) => setFilter((prev) => ({ ...prev, type: event.target.value as FilterState["type"] }))}
              className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
            >
              <option value="user">Theo ID người dùng</option>
              <option value="role">Theo tên quyền</option>
            </select>
          </label>
          <label className="space-y-2 text-sm">
            <span className="font-medium text-primary-foreground">Giá trị</span>
            <input
              value={filter.key}
              onChange={(event) => setFilter((prev) => ({ ...prev, key: event.target.value }))}
              className="w-full rounded-xl border border-surface-muted bg-surface px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-primary/60"
              placeholder={filter.type === "user" ? "Nhập ID người dùng" : "Nhập tên quyền"}
            />
          </label>
          <button
            type="submit"
            disabled={!isFilterReady}
            className="inline-flex items-center gap-2 rounded-full border border-primary/50 bg-primary px-6 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:bg-primary/60"
          >
            <GraduationCap className="h-4 w-4" />
            Lọc dữ liệu
          </button>
        </form>
        <div className="rounded-2xl border border-surface-muted bg-surface/70 p-5">
          <header className="flex items-center gap-2 text-primary-foreground">
            <Users2 className="h-5 w-5" />
            <h4 className="text-sm font-semibold uppercase tracking-[0.3em]">Kết quả lọc</h4>
          </header>
          {filter.key.trim().length === 0 ? (
            <p className="mt-3 text-sm text-surface-foreground/60">Nhập giá trị để bắt đầu tra cứu.</p>
          ) : filterQuery.isLoading ? (
            <p className="mt-3 text-sm text-surface-foreground/60">Đang tải dữ liệu...</p>
          ) : filterQuery.isError ? (
            <p className="mt-3 text-sm text-red-300">Không thể tải dữ liệu theo điều kiện lọc.</p>
          ) : filterQuery.data && filterQuery.data.length > 0 ? (
            <div className="mt-3 overflow-hidden rounded-lg border border-surface-muted/60 bg-surface/80">
              <table className="min-w-full text-sm">
                <thead className="bg-surface-muted/40 text-xs uppercase tracking-wide text-surface-foreground/60">
                  <tr>
                    <th scope="col" className="px-4 py-2 text-left font-semibold">Role</th>
                    <th scope="col" className="px-4 py-2 text-left font-semibold">User ID</th>
                    <th scope="col" className="px-4 py-2 text-left font-semibold">Gán bởi</th>
                    <th scope="col" className="px-4 py-2 text-left font-semibold">Tạo lúc</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-surface-muted/40 text-surface-foreground/80">
                  {(filterQuery.data as UserHasRoleResponse[]).map((item) => (
                    <tr key={item.id}>
                      <td className="px-4 py-2 font-semibold text-primary-foreground">{item.role_name}</td>
                      <td className="px-4 py-2 text-xs text-surface-foreground/70">#{item.user_id}</td>
                      <td className="px-4 py-2 text-xs text-surface-foreground/70">#{item.assigned_by}</td>
                      <td className="px-4 py-2 text-xs text-surface-foreground/60">
                        {new Date(item.created_at).toLocaleString()}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <p className="mt-3 text-sm text-surface-foreground/60">Không có dữ liệu phù hợp.</p>
          )}
        </div>
      </section>
    </div>
  );
};

export default AdminUserRolesPage;
