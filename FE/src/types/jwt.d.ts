interface JWT {
    sub: string;
    iat: string;
    jti: string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string[];
    exp: number;
    iss: string;
    aud: string;
}
function getRoleFromJWT(jwt: JWT): string[] {
    return jwt["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
}