import { logout } from "@services/auth.service";
import { Buffer } from "buffer";

export const ACCESS_TOKEN_COOKIE = "truyencv_access_token";
export const REFRESH_TOKEN_COOKIE = "truyencv_refresh_token";

const isBrowser = () => typeof window !== "undefined" && typeof document !== "undefined";

const buildCookieOptions = (maxAge: number) => {
	const options = ["path=/", `max-age=${maxAge}`, "SameSite=Strict"];

	if (isBrowser() && window.location.protocol === "https:") {
		options.push("Secure");
	}

	return options.join("; ");
};

const setCookie = (name: string, value: string, maxAge: number) => {
	if (!isBrowser()) {
		return;
	}

	document.cookie = `${name}=${encodeURIComponent(value)}; ${buildCookieOptions(maxAge)}`;
};

const deleteCookie = (name: string) => {
	if (!isBrowser()) {
		return;
	}

	const extraSecure = window.location.protocol === "https:" ? "; Secure" : "";
	document.cookie = `${name}=; path=/; max-age=0; SameSite=Strict${extraSecure}`;
};

const getCookie = (name: string): string => {
	if (isBrowser()) {
		const cookies = document.cookie ? document.cookie.split(";") : [];
		for (const rawCookie of cookies) {
			const [cookieName, ...rest] = rawCookie.trim().split("=");
			if (cookieName === name) {
				return decodeURIComponent(rest.join("="));
			}
		}
	}
	return "";
};

const getSVCookie = async (name: string): Promise<string> => {
	if (!isBrowser()) {
		const cookies = await (await import("next/headers")).cookies();
		return cookies.get(name)?.value ?? "";
	}
	return "";
};
const base64UrlDecode = (value: string) => {
	const normalized = value.replace(/-/g, "+").replace(/_/g, "/");
	const padding = normalized.length % 4 ? 4 - (normalized.length % 4) : 0;
	const padded = normalized.concat("=".repeat(padding));

	if (typeof window === "undefined") {
		return Buffer.from(padded, "base64").toString("utf-8");
	}

	try {
		return decodeURIComponent(
			atob(padded)
				.split("")
				.map((char) => `%${char.charCodeAt(0).toString(16).padStart(2, "0")}`)
				.join("")
		);
	} catch {
		return atob(padded);
	}
};

export const decodeJwtToken = (token: string): JWT | null => {
	try {
		const [, payload] = token.split(".");

		if (!payload) {
			return null;
		}

		const json = base64UrlDecode(payload);
		return JSON.parse(json) as JWT;
	} catch {
		return null;
	}
};

const normalizeClaim = (claim: unknown) => {
	if (!claim) {
		return [] as string[];
	}

	if (Array.isArray(claim)) {
		return claim.map((value) => value?.toString?.() ?? "").filter(Boolean);
	}

	if (typeof claim === "string") {
		return [claim];
	}

	return [] as string[];
};

export function getRoleFromJWT(jwt: JWT): string[] {
	return normalizeClaim(jwt["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? jwt.role);
}
const getRolesFromPayload = (payload: JWT | null) => {
	if (!payload) {
		return [] as string[];
	}

	return getRoleFromJWT(payload);
};

const getPermissionsFromPayload = (payload: JWT | null) => {
	if (!payload) {
		return [] as string[];
	}

	return normalizeClaim(payload.permissions);
};

export function setAuthTokens(accessToken: string, refreshToken: string): void;
export function setAuthTokens(accessToken: string, refreshToken: string, accessTokenExpiryMinutes: number, refreshTokenExpiryDays: number): void;
export function setAuthTokens(accessToken: string, refreshToken: string, accessTokenExpiryMinutes?: number, refreshTokenExpiryDays?: number): void {
	if (accessTokenExpiryMinutes === undefined || refreshTokenExpiryDays === undefined) {
		document.cookie = `${ACCESS_TOKEN_COOKIE}=${encodeURIComponent(accessToken)};`;
		document.cookie = `${REFRESH_TOKEN_COOKIE}=${encodeURIComponent(refreshToken)};`;
		return;
	}
	if (accessToken) {
		setCookie(ACCESS_TOKEN_COOKIE, accessToken, accessTokenExpiryMinutes * 60);
	}

	if (refreshToken) {
		setCookie(REFRESH_TOKEN_COOKIE, refreshToken, refreshTokenExpiryDays * 24 * 60 * 60);
	}
};

export const clearAuthTokens = async ({ from_logout = false } = {}) => {
	try {
		if (from_logout === false) await logout();
	} catch (error) {
		console.error("Error logging out:", error);
	}
	deleteCookie(ACCESS_TOKEN_COOKIE);
	deleteCookie(REFRESH_TOKEN_COOKIE);
};

export const getAccessToken = () => getCookie(ACCESS_TOKEN_COOKIE);

export const getRefreshToken = () => getCookie(REFRESH_TOKEN_COOKIE);

export const getSVAccessToken = async () => await getSVCookie(ACCESS_TOKEN_COOKIE);

export const getSVRefreshToken = async () => await getSVCookie(REFRESH_TOKEN_COOKIE);
export const getAccessTokenPayload = () => {
	const token = getAccessToken();
	return token ? decodeJwtToken(token) : null;
};

export const AuthStateFromJWT = (jwt: JWT | null): ServerAuthState => {
	return {
		isAuthenticated: jwt !== null,
		userId: jwt?.sub ?? null,
		name: jwt?.name ?? null,
		avatar: jwt?.avatar ?? null,
		email: jwt?.email ?? null,
		roles: getRoleFromJWT(jwt ?? {} as JWT),
		permissions: normalizeClaim(jwt?.permissions),
		payload: jwt
	} as ServerAuthState;
}
export const getAccessTokenRoles = () => getRolesFromPayload(getAccessTokenPayload());

export const tokenHasRole = (role: string) =>
	getAccessTokenRoles().some((existingRole) => existingRole.toLowerCase() === role.toLowerCase());

export const getAccessTokenPermissions = () => getPermissionsFromPayload(getAccessTokenPayload());

export const tokenHasPermission = (permission: string) =>
	getAccessTokenPermissions().some((existingPermission) => existingPermission.toLowerCase() === permission.toLowerCase());

export const authCookieNames = {
	access: ACCESS_TOKEN_COOKIE,
	refresh: REFRESH_TOKEN_COOKIE
};
