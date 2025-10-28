interface JWT {
    sub: string;
    iat: number;
    jti: string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string[];
    role?: string[];
    permissions?: string[];
    name?: string | null;
    email?: string | null;
    avatar?: string | null;
    exp: number;
    iss: string;
    aud: string;
}
interface JWT_REFRESH_RESPONSE {
    access_token: string;
    refresh_token: string;
    access_token_minutes: number;
    refresh_token_days: number;
}
interface ServerAuthState {
  isAuthenticated: boolean;
  token: string | null;
  userId: string | null;
  name: string | null;
  avatar: string | null;
  email: string | null;
  roles: string[];
  permissions: string[];
  payload: JWT | null;
}
interface VerifiedAccessToken {
  userId: string | null;
  name: string | null;
  avatar: string | null;
  email: string | null;
  roles: string[];
  permissions: string[];
  payload: JWT;
}