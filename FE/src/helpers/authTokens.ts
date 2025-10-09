import { Buffer } from "buffer";

const ACCESS_TOKEN_COOKIE = "truyencv_access_token";
const REFRESH_TOKEN_COOKIE = "truyencv_refresh_token";

const ACCESS_TOKEN_MAX_AGE = 7 * 60; // 7 phút
const REFRESH_TOKEN_MAX_AGE = 30 * 24 * 60 * 60; // 30 ngày

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

const getCookie = (name: string) => {
	if (!isBrowser()) {
		return undefined;
	}

	const cookies = document.cookie ? document.cookie.split(";") : [];

	for (const rawCookie of cookies) {
		const [cookieName, ...rest] = rawCookie.trim().split("=");

		if (cookieName === name) {
			return decodeURIComponent(rest.join("="));
		}
	}

	return undefined;
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
	} catch (error) {
		return atob(padded);
	}
};

const parseJwtToken = (token: string): JWT | null => {
	try {
		const [, payload] = token.split(".");

		if (!payload) {
			return null;
		}

		const json = base64UrlDecode(payload);
		return JSON.parse(json) as JWT;
	} catch (error) {
		return null;
	}
};

const getRolesFromPayload = (payload: JWT | null) => {
	if (!payload) {
		return [] as string[];
	}

	const roles = getRoleFromJWT(payload);
	return Array.isArray(roles) ? roles : [];
};

export const setAuthTokens = (accessToken: string, refreshToken: string) => {
	if (accessToken) {
		setCookie(ACCESS_TOKEN_COOKIE, accessToken, ACCESS_TOKEN_MAX_AGE);
	}

	if (refreshToken) {
		setCookie(REFRESH_TOKEN_COOKIE, refreshToken, REFRESH_TOKEN_MAX_AGE);
	}
};

export const clearAuthTokens = () => {
	deleteCookie(ACCESS_TOKEN_COOKIE);
	deleteCookie(REFRESH_TOKEN_COOKIE);
};

export const getAccessToken = () => getCookie(ACCESS_TOKEN_COOKIE);

export const getRefreshToken = () => getCookie(REFRESH_TOKEN_COOKIE);

export const hasValidTokens = () => Boolean(getAccessToken() && getRefreshToken());

export const getAccessTokenPayload = () => {
	const token = getAccessToken();
	return token ? parseJwtToken(token) : null;
};

export const getAccessTokenRoles = () => getRolesFromPayload(getAccessTokenPayload());

export const tokenHasRole = (role: string) =>
	getAccessTokenRoles().some((existingRole) => existingRole.toLowerCase() === role.toLowerCase());

export const authCookieNames = {
	access: ACCESS_TOKEN_COOKIE,
	refresh: REFRESH_TOKEN_COOKIE
};
