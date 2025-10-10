export type AppEnvironment = {
  BACKEND_URL: string;
  CDN_URL: string;
  JWT_SECRET: string;
  FE_USER: string;
  FE_ADMIN: string;
};

const fallbackUrl = (value?: string | null) => {
  if (!value) {
    return "";
  }

  return value.trim().replace(/\/$/, "");
};

export const appEnv: AppEnvironment = {
  BACKEND_URL: fallbackUrl(process.env.NEXT_PUBLIC_BACKEND_URL),
  CDN_URL: fallbackUrl(process.env.NEXT_PUBLIC_CDN_URL),
  JWT_SECRET: fallbackUrl(process.env.NEXT_PUBLIC_JWT_SECRET),
  FE_USER: fallbackUrl(process.env.NEXT_PUBLIC_FE_USER).replace(/http[s]:\/\//, ""),
  FE_ADMIN: fallbackUrl(process.env.NEXT_PUBLIC_FE_ADMIN).replace(/http[s]:\/\//, ""),
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
