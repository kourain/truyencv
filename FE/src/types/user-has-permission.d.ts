interface CreateUserHasPermissionRequest {
    permissions: number;
    user_id: string;
    assigned_by: string;
}

interface UpdateUserHasPermissionRequest {
    id: string;
    permissions: number;
    user_id: string;
    assigned_by: string;
}

interface UserHasPermissionResponse {
    id: string;
    permissions: number;
    user_id: string;
    assigned_by: string;
    created_at: string;
    updated_at: string;
    revoked_at: string | null;
    revoke_until: string | null;
    is_active: boolean;
}