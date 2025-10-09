import type { NextConfig } from "next";

const {
  BACKEND_URL = "",
  COMIC_CDN_URL = "",
  FE_PORT = "3000"
} = process.env;

const nextConfig: NextConfig = {
  reactStrictMode: true,
  experimental: {
    typedRoutes: true
  },
  env: {
    NEXT_PUBLIC_BACKEND_URL: BACKEND_URL,
    NEXT_PUBLIC_CDN_URL: COMIC_CDN_URL,
    NEXT_PUBLIC_FE_PORT: FE_PORT
  }
};

export default nextConfig;
