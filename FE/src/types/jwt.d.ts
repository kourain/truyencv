interface JWT {
    sub: string;
    iat: number;
    jti: string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string[] | string;
    role?: string[] | string;
    permissions?: string[] | string;
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