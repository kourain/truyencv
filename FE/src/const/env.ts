export type AppEnvironment = {
  BACKEND_URL: string;
  CDN_URL: string;
  JWT_SECRET: string;
  FE_USER: string[];
  FE_ADMIN: string[];
  FIREBASE_API_KEY: string;
  FIREBASE_AUTH_DOMAIN: string;
  FIREBASE_PROJECT_ID: string;
  FIREBASE_STORAGE_BUCKET: string;
  FIREBASE_MESSAGING_SENDER_ID: string;
  FIREBASE_APP_ID: string;
  FIREBASE_MEASUREMENT_ID: string;
};

const fallbackUrl = (value?: string | null) => {
  if (!value) {
    return "";
  }

  return value.trim().replace(/\/$/, "");
};

const fallbackValue = (value?: string | null) => value?.trim() ?? "";

export const appEnv: AppEnvironment = {
  BACKEND_URL: fallbackUrl(process.env.NEXT_PUBLIC_BACKEND_URL),
  CDN_URL: fallbackUrl(process.env.NEXT_PUBLIC_CDN_URL),
  JWT_SECRET: fallbackUrl(process.env.NEXT_PUBLIC_JWT_SECRET),
  FE_USER: fallbackUrl(process.env.NEXT_PUBLIC_FE_USER).replace(/http[s]:\/\//, "").split(","),
  FE_ADMIN: fallbackUrl(process.env.NEXT_PUBLIC_FE_ADMIN).replace(/http[s]:\/\//, "").split(","),
  FIREBASE_API_KEY: fallbackValue(process.env.NEXT_PUBLIC_FIREBASE_API_KEY),
  FIREBASE_AUTH_DOMAIN: fallbackValue(process.env.NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN),
  FIREBASE_PROJECT_ID: fallbackValue(process.env.NEXT_PUBLIC_FIREBASE_PROJECT_ID),
  FIREBASE_STORAGE_BUCKET: fallbackValue(process.env.NEXT_PUBLIC_FIREBASE_STORAGE_BUCKET),
  FIREBASE_MESSAGING_SENDER_ID: fallbackValue(process.env.NEXT_PUBLIC_FIREBASE_MESSAGING_SENDER_ID),
  FIREBASE_APP_ID: fallbackValue(process.env.NEXT_PUBLIC_FIREBASE_APP_ID),
  FIREBASE_MEASUREMENT_ID: fallbackValue(process.env.NEXT_PUBLIC_FIREBASE_MEASUREMENT_ID),
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
