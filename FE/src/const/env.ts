export type AppEnvironment = {
  backendUrl: string;
  cdnUrl: string;
  fePort: number;
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
  backendUrl: fallbackUrl(process.env.NEXT_PUBLIC_BACKEND_URL),
  cdnUrl: fallbackUrl(process.env.NEXT_PUBLIC_CDN_URL),
  fePort: parsePort(process.env.NEXT_PUBLIC_FE_PORT)
};

export const isBrowser = typeof window !== "undefined";

export const withBaseUrl = (path: string) => {
  if (!path) {
    return appEnv.backendUrl;
  }

  if (/^https?:\/\//i.test(path)) {
    return path;
  }

  const trimmedPath = path.startsWith("/") ? path : `/${path}`;
  const base = appEnv.backendUrl.replace(/\/+$/, "");

  return `${base}${trimmedPath}`;
};
