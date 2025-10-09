export type AppEnvironment = {
  BACKEND_URL: string;
  CDN_URL: string;
};

const fallbackUrl = (value?: string | null) => {
  if (!value) {
    return "";
  }

  return value.trim().replace(/\/$/, "");
};

const parsePort = (value?: string | null) => {
  const port = Number(value ?? "");

  return Number.isFinite(port) && port > 0 ? port : 3000;
};

export const appEnv: AppEnvironment = {
  BACKEND_URL: fallbackUrl(process.env.NEXT_PUBLIC_BACKEND_URL),
  CDN_URL: fallbackUrl(process.env.NEXT_PUBLIC_CDN_URL),
};

export const isBrowser = typeof window !== "undefined";

export const withBaseUrl = (path: string) => {
  if (!path) {
    return appEnv.BACKEND_URL;
  }

  if (/^https?:\/\//i.test(path)) {
    return path;
  }

  const trimmedPath = path.startsWith("/") ? path : `/${path}`;
  const base = appEnv.BACKEND_URL.replace(/\/+$/, "");

  return `${base}${trimmedPath}`;
};
