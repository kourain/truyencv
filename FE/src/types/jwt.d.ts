interface JWT {
    sub: string;
    iat: number;
    jti: string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string[] | string;
    role?: string[] | string;
    Permissions?: string[] | string;
    exp: number;
    iss: string;
    aud: string;
}